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
    /// <summary>
    /// Mixer-specific implementation of a web socket client.
    /// </summary>
    public class MixerWebSocketClientBase : ClientWebSocketBase
    {
        /// <summary>
        /// Whether the client is connected.
        /// </summary>
        public bool Connected { get; protected set; }
        /// <summary>
        /// Whether the client is authenticated.
        /// </summary>
        public bool Authenticated { get; protected set; }

        /// <summary>
        /// Invoked when a packet is sent.
        /// </summary>
        public event EventHandler<WebSocketPacket> OnPacketSentOccurred;
        /// <summary>
        /// Invoked when a packet is received.
        /// </summary>
        public event EventHandler<WebSocketPacket> OnPacketReceivedOccurred;

        /// <summary>
        /// Invoked when a method occurs.
        /// </summary>
        public event EventHandler<MethodPacket> OnMethodOccurred;
        /// <summary>
        /// Invoked when a reply occurs.
        /// </summary>
        public event EventHandler<ReplyPacket> OnReplyOccurred;
        /// <summary>
        /// Invoked when an event occurs.
        /// </summary>
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

        /// <summary>
        /// Sends a packet to the server.
        /// </summary>
        /// <param name="packet">The packet to send</param>
        /// <param name="checkIfAuthenticated">Whether to check if the client is authenciated</param>
        /// <returns>An awaitable task with the packet ID</returns>
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

        /// <summary>
        /// Sends a packet to the server and listens for a reply.
        /// </summary>
        /// <param name="packet">The packet to send</param>
        /// <param name="checkIfAuthenticated">Whether to check if the client is authenciated</param>
        /// <returns>An awaitable task with the reply packet</returns>
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

        /// <summary>
        /// Sends a packet to the server and listens for a reply.
        /// </summary>
        /// <param name="packet">The packet to send</param>
        /// <param name="checkIfAuthenticated">Whether to check if the client is authenciated</param>
        /// <returns>An awaitable task with the reply data</returns>
        protected async Task<T> SendAndListen<T>(WebSocketPacket packet, bool checkIfAuthenticated = true)
        {
            ReplyPacket reply = await this.SendAndListen(packet);
            return this.GetSpecificReplyResultValue<T>(reply);
        }

        /// <summary>
        /// Invokes the matching event for the specified method packet.
        /// </summary>
        /// <typeparam name="T">The type of data</typeparam>
        /// <param name="methodPacket">The method packet</param>
        /// <param name="eventHandler">The event handler</param>
        protected void InvokeMethodPacketEvent<T>(MethodPacket methodPacket, EventHandler<T> eventHandler)
        {
            this.SendSpecificPacket(JsonConvert.DeserializeObject<T>(methodPacket.parameters.ToString()), eventHandler);
        }

        /// <summary>
        /// Invokes the matching event for the specified event packet.
        /// </summary>
        /// <typeparam name="T">The type of data</typeparam>
        /// <param name="eventPacket">The event packet</param>
        /// <param name="eventHandler">The event handler</param>
        protected void InvokeEventPacketEvent<T>(EventPacket eventPacket, EventHandler<T> eventHandler)
        {
            this.SendSpecificPacket(JsonConvert.DeserializeObject<T>(eventPacket.data.ToString()), eventHandler);
        }

        /// <summary>
        /// Invokes the event for the specified packet.
        /// </summary>
        /// <typeparam name="T">The type of data</typeparam>
        /// <param name="packet">The packet</param>
        /// <param name="eventHandler">The event handler</param>
        protected void SendSpecificPacket<T>(T packet, EventHandler<T> eventHandler)
        {
            eventHandler?.Invoke(this, packet);
        }

        /// <summary>
        /// Processes a JSON packet received from the server.
        /// </summary>
        /// <param name="packet">The packet JSON</param>
        /// <returns>An awaitable Task</returns>
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
                    if (this.replyIDListeners.ContainsKey(replyPacket.id))
                    {
                        this.replyIDListeners[replyPacket.id] = replyPacket;
                    }
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

        /// <summary>
        /// Verifies if data exists in the reply packet.
        /// </summary>
        /// <param name="replyPacket">The reply packet to check</param>
        /// <returns>Whether there is data in the reply packet</returns>
        protected bool VerifyDataExists(ReplyPacket replyPacket)
        {
            return (replyPacket != null && replyPacket.data != null && !string.IsNullOrEmpty(replyPacket.data.ToString()));
        }

        /// <summary>
        /// Verifies if there are no errors in the reply packet.
        /// </summary>
        /// <param name="replyPacket">The reply packet to check</param>
        /// <returns>Whether there are no errors in the reply packet</returns>
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

        /// <summary>
        /// Gets the specified type data from the reply packet.
        /// </summary>
        /// <typeparam name="T">The type to get</typeparam>
        /// <param name="replyPacket">The reply packet</param>
        /// <returns>The typed data</returns>
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
