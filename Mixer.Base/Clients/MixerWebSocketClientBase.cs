using Mixer.Base.Model.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Mixer.Base.Clients
{
    public abstract class MixerWebSocketClientBase : WebSocketClientBase
    {
        public bool Connected { get; protected set; }
        public bool Authenticated { get; protected set; }

        public event EventHandler<MethodPacket> OnMethodOccurred;
        public event EventHandler<ReplyPacket> OnReplyOccurred;
        public event EventHandler<EventPacket> OnEventOccurred;

        protected override async Task<uint> Send(WebSocketPacket packet, bool checkIfAuthenticated = true)
        {
            if (!this.Connected)
            {
                throw new InvalidOperationException("Client is not connected");
            }

            if (checkIfAuthenticated && !this.Authenticated)
            {
                throw new InvalidOperationException("Client is not authenticated");
            }

            return await base.Send(packet, checkIfAuthenticated);
        }

        protected void SendSpecificMethod<T>(MethodPacket methodPacket, EventHandler<T> eventHandler)
        {
            if (eventHandler != null)
            {
                eventHandler(this, JsonConvert.DeserializeObject<T>(methodPacket.parameters.ToString()));
            }
        }

        protected void SendSpecificEvent<T>(EventPacket eventPacket, EventHandler<T> eventHandler)
        {
            if (eventHandler != null)
            {
                eventHandler(this, JsonConvert.DeserializeObject<T>(eventPacket.data.ToString()));
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

        protected override void DisconnectOccurred(WebSocketCloseStatus? result)
        {
            this.Connected = this.Authenticated = false;
            base.DisconnectOccurred(result);
        }
    }
}
