using Mixer.Base.Model.Client;
using Mixer.Base.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Mixer.Base.Clients
{
    public class MixerWebSocketClientBase : WebSocketClientBase
    {
        public bool Connected { get; protected set; }
        public bool Authenticated { get; protected set; }

        public event EventHandler<WebSocketPacket> OnPacketSentOccurred;
        public event EventHandler<WebSocketPacket> OnPacketReceivedOccurred;

        public event EventHandler<MethodPacket> OnMethodOccurred;
        public event EventHandler<ReplyPacket> OnReplyOccurred;
        public event EventHandler<EventPacket> OnEventOccurred;

        private SemaphoreSlim packetIDSemaphore = new SemaphoreSlim(1);
        private int randomPacketIDSeed = (int)DateTime.Now.Ticks;
        private Dictionary<uint, ReplyPacket> replyIDListeners = new Dictionary<uint, ReplyPacket>();

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

            await this.AssignPacketID(packet);

            string packetJson = JsonConvert.SerializeObject(packet);

            await this.Send(packetJson);

            if (this.OnPacketSentOccurred != null)
            {
                this.OnPacketSentOccurred(this, packet);
            }

            return packet.id;
        }

        protected async Task<ReplyPacket> SendAndListen(WebSocketPacket packet, bool checkIfAuthenticated = true)
        {
            ReplyPacket replyPacket = null;

            await this.AssignPacketID(packet);
            this.replyIDListeners[packet.id] = null;

            await this.Send(packet, checkIfAuthenticated);

            await this.WaitForResponse(() =>
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
            if (eventHandler != null)
            {
                eventHandler(this, packet);
            }
        }

        protected override Task ProcessReceivedPacket(string packetJSON)
        {
            dynamic jsonObject = JsonConvert.DeserializeObject(packetJSON);

            List<WebSocketPacket> packets = new List<WebSocketPacket>();
            if (jsonObject.Type == JTokenType.Array)
            {
                JArray array = JArray.Parse(packetJSON);
                foreach (JToken token in array.Children())
                {
                    packets.Add(token.ToObject<WebSocketPacket>());
                }
            }
            else
            {
                packets.Add(JsonConvert.DeserializeObject<WebSocketPacket>(packetJSON));
            }

            foreach (WebSocketPacket packet in packets)
            {
                if (this.OnPacketReceivedOccurred != null)
                {
                    this.OnPacketReceivedOccurred(this, packet);
                }

                if (packet.type.Equals("method"))
                {
                    MethodPacket methodPacket = JsonConvert.DeserializeObject<MethodPacket>(packetJSON);
                    this.SendSpecificPacket(methodPacket, this.OnMethodOccurred);
                }
                else if (packet.type.Equals("reply"))
                {
                    ReplyPacket replyPacket = JsonConvert.DeserializeObject<ReplyPacket>(packetJSON);
                    this.AddReplyPacketForListeners(replyPacket);
                    this.SendSpecificPacket(replyPacket, this.OnReplyOccurred);
                }
                else if (packet.type.Equals("event"))
                {
                    EventPacket eventPacket = JsonConvert.DeserializeObject<EventPacket>(packetJSON);
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
                else if (replyPacket.dataObject != null)
                {
                    return JsonConvert.DeserializeObject<T>(replyPacket.dataObject.ToString());
                }
            }
            return default(T);
        }

        private async Task AssignPacketID(WebSocketPacket packet)
        {
            if (packet.id == 0)
            {
                await this.packetIDSemaphore.WaitAsync();

                this.randomPacketIDSeed -= 1000;
                Random random = new Random(this.randomPacketIDSeed);
                packet.id = (uint)random.Next(100, int.MaxValue);

                this.packetIDSemaphore.Release();
            }
        }
    }
}
