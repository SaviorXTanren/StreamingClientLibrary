using Mixer.Base.Model.Client;
using Mixer.Base.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StreamingClient.Base.Util;
using StreamingClient.Base.Web;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Mixer.Base.Clients
{
    public class MixerWebSocketClientBase : ClientWebSocketBase
    {
        public bool Connected { get; protected set; }
        public bool Authenticated { get; protected set; }

        public event EventHandler<WebSocketPacket> OnPacketSentOccurred;
        public event EventHandler<WebSocketPacket> OnPacketReceivedOccurred;

        public event EventHandler<MethodPacket> OnMethodOccurred;
        public event EventHandler<ReplyPacket> OnReplyOccurred;
        public event EventHandler<EventPacket> OnEventOccurred;

        private int currentPacketId = 0;
        private readonly Dictionary<uint, ReplyPacket> replyIDListeners = new Dictionary<uint, ReplyPacket>();

        /// <summary>
        /// Disconnects the web socket.
        /// </summary>
        /// <param name="closeStatus">Optional status to send to partner web socket as to why the web socket is being closed</param>
        /// <returns>A task for the closing of the web socket</returns>
        public override Task Disconnect(WebSocketCloseStatus closeStatus = WebSocketCloseStatus.NormalClosure)
        {
            this.Connected = this.Authenticated = false;
            return base.Disconnect(closeStatus);
        }

        protected virtual async Task<uint> Send(WebSocketPacket packet, bool checkIfAuthenticated = true)
        {
            if (!this.Connected)
            {
                throw new InvalidOperationException("Client is not connected");
            }

            if (checkIfAuthenticated && !this.Authenticated)
            {
                throw new InvalidOperationException("Client is not authenticated");
            }

            this.AssignPacketID(packet);

            string packetJson = JsonConvert.SerializeObject(packet);

            await this.Send(packetJson);

            this.OnPacketSentOccurred?.Invoke(this, packet);

            return packet.id;
        }

        protected async Task<ReplyPacket> SendAndListen(WebSocketPacket packet, bool checkIfAuthenticated = true)
        {
            ReplyPacket replyPacket = null;

            this.AssignPacketID(packet);
            this.replyIDListeners[packet.id] = null;

            await this.Send(packet, checkIfAuthenticated);

            await this.WaitForSuccess(() =>
            {
                if (this.replyIDListeners.ContainsKey(packet.id) && this.replyIDListeners[packet.id] != null)
                {
                    replyPacket = this.replyIDListeners[packet.id];
                    return true;
                }
                return false;
            });

            this.replyIDListeners.Remove(packet.id);

            return replyPacket;
        }

        protected async Task<T> SendAndListen<T>(WebSocketPacket packet, bool checkIfAuthenticated = true)
        {
            ReplyPacket reply = await this.SendAndListen(packet);
            return this.GetSpecificReplyResultValue<T>(reply);
        }

        protected void SendSpecificMethod<T>(MethodPacket methodPacket, EventHandler<T> eventHandler)
        {
            this.SendSpecificPacket(JsonConvert.DeserializeObject<T>(methodPacket.parameters.ToString()), eventHandler);
        }

        protected void SendSpecificEvent<T>(EventPacket eventPacket, EventHandler<T> eventHandler)
        {
            this.SendSpecificPacket(JsonConvert.DeserializeObject<T>(eventPacket.data.ToString()), eventHandler);
        }

        protected void SendSpecificPacket<T>(T packet, EventHandler<T> eventHandler)
        {
            eventHandler?.Invoke(this, packet);
        }

        protected override Task ProcessReceivedPacket(string packet)
        {
            List<JToken> packetJTokens = new List<JToken>();

            JToken packetJToken = JToken.Parse(packet);
            if (packetJToken is JArray)
            {
                foreach (JToken t in (JArray)packetJToken)
                {
                    packetJTokens.Add(t);
                }
            }
            else
            {
                packetJTokens.Add(packetJToken);
            }

            foreach (JToken token in packetJTokens)
            {
                WebSocketPacket webSocketPacket = token.ToObject<WebSocketPacket>();
                string data = JSONSerializerHelper.SerializeToString(token);

                this.OnPacketReceivedOccurred?.Invoke(this, webSocketPacket);

                if (webSocketPacket.type.Equals("method"))
                {
                    MethodPacket methodPacket = JsonConvert.DeserializeObject<MethodPacket>(data);
                    this.SendSpecificPacket(methodPacket, this.OnMethodOccurred);
                }
                else if (webSocketPacket.type.Equals("reply"))
                {
                    ReplyPacket replyPacket = JsonConvert.DeserializeObject<ReplyPacket>(data);
                    this.AddReplyPacketForListeners(replyPacket);
                    this.SendSpecificPacket(replyPacket, this.OnReplyOccurred);
                }
                else if (webSocketPacket.type.Equals("event"))
                {
                    EventPacket eventPacket = JsonConvert.DeserializeObject<EventPacket>(data);
                    this.SendSpecificPacket(eventPacket, this.OnEventOccurred);
                }
            }

            return Task.FromResult(0);
        }

        protected void AddReplyPacketForListeners(ReplyPacket packet)
        {
            if (this.replyIDListeners.ContainsKey(packet.id))
            {
                this.replyIDListeners[packet.id] = packet;
            }
        }

        protected bool VerifyDataExists(ReplyPacket replyPacket)
        {
            return (replyPacket != null && replyPacket.data != null && !string.IsNullOrEmpty(replyPacket.data.ToString()));
        }

        protected bool VerifyNoErrors(ReplyPacket replyPacket)
        {
            if (replyPacket == null)
            {
                return false;
            }
            if (replyPacket.errorObject != null)
            {
                throw new ReplyPacketException(JsonConvert.DeserializeObject<ReplyErrorModel>(replyPacket.error.ToString()));
            }
            return true;
        }

        protected T GetSpecificReplyResultValue<T>(ReplyPacket replyPacket)
        {
            this.VerifyNoErrors(replyPacket);

            if (replyPacket != null)
            {
                if (replyPacket.resultObject != null)
                {
                    return JsonConvert.DeserializeObject<T>(replyPacket.resultObject.ToString());
                }
                else if (replyPacket.data != null && replyPacket.data.Type == JTokenType.Array)
                {
                    return JsonConvert.DeserializeObject<T>(replyPacket.data.ToString());
                }
                else if (replyPacket.dataObject != null)
                {
                    return JsonConvert.DeserializeObject<T>(replyPacket.dataObject.ToString());
                }
            }
            return default(T);
        }

        private void AssignPacketID(WebSocketPacket packet)
        {
            // This while loop is to protected from the packet ID wrapping around.
            // This is highly unlikely as it would require more than 4 billion packets to be sent.
            while (packet.id == 0)
            {
                packet.id = (uint)Interlocked.Increment(ref currentPacketId);
            }
        }
    }
}
