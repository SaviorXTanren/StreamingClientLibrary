using StreamingClient.Base.Model.OAuth;
using StreamingClient.Base.Util;
using StreamingClient.Base.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Twitch.Base.Models.Clients.PubSub;

namespace Twitch.Base.Clients
{
    /// <summary>
    /// Web Socket client for interacting with PubSub service.
    /// </summary>
    public class PubSubClient : ClientWebSocketBase
    {
        /// <summary>
        /// The default PubSub connection url.
        /// </summary>
        public const string PUBSUB_CONNECTION_URL = "wss://pubsub-edge.twitch.tv";

        /// <summary>
        /// Invoked when a pong packet is received.
        /// </summary>
        public event EventHandler OnPongReceived;

        /// <summary>
        /// Invoked when a reconnect packet is received.
        /// </summary>
        public event EventHandler OnReconnectReceived;

        /// <summary>
        /// Invoked when a response packet is received.
        /// </summary>
        public event EventHandler<PubSubResponsePacketModel> OnResponseReceived;
        /// <summary>
        /// Invoked when a message packet is received.
        /// </summary>
        public event EventHandler<PubSubMessagePacketModel> OnMessageReceived;

        private TwitchConnection connection;

        /// <summary>
        /// Creates a new instance of the PubSubClient class.
        /// </summary>
        /// <param name="connection">The current connection</param>
        public PubSubClient(TwitchConnection connection)
        {
            this.connection = connection;
        }

        /// <summary>
        /// Connects to the default PubSub connection.
        /// </summary>
        /// <returns>An awaitable Task</returns>
        public async Task Connect()
        {
            await base.Connect(PubSubClient.PUBSUB_CONNECTION_URL);
        }

        /// <summary>
        /// Sends a listen packet for a specified set of topics.
        /// </summary>
        /// <param name="topics">The topics to listen for</param>
        /// <returns>An awaitable Task</returns>
        public async Task Listen(IEnumerable<PubSubListenTopicModel> topics)
        {
            OAuthTokenModel oauthToken = await this.connection.GetOAuthToken();
            await this.Send(new PubSubPacketModel("LISTEN", new { topics = topics.Select(t => t.ToString()).ToList(), auth_token = oauthToken.accessToken }));
        }

        /// <summary>
        /// Sends a ping packet.
        /// </summary>
        /// <returns>An awaitable Task</returns>
        public async Task Ping()
        {
            await this.Send(JSONSerializerHelper.SerializeToString(new { type = "PING" }));
        }

        /// <summary>
        /// Processes the received text packet.
        /// </summary>
        /// <param name="packetText">The receive text packet</param>
        /// <returns>An awaitable task</returns>
        protected override Task ProcessReceivedPacket(string packetText)
        {
            PubSubPacketModel packet = JSONSerializerHelper.DeserializeFromString<PubSubPacketModel>(packetText);
            if (packet != null)
            {
                switch (packet.type)
                {
                    case "RECONNECT":
                        this.OnReconnectReceived?.Invoke(this, new EventArgs());
                        break;
                    case "RESPONSE":
                        this.OnResponseReceived?.Invoke(this, JSONSerializerHelper.DeserializeFromString<PubSubResponsePacketModel>(packetText));
                        break;
                    case "MESSAGE":
                        PubSubMessagePacketModel message = JSONSerializerHelper.DeserializeFromString<PubSubMessagePacketModel>(packetText);
                        this.OnMessageReceived?.Invoke(this, message);
                        break;
                    case "PONG":
                        this.OnPongReceived?.Invoke(this, new EventArgs());
                        break;
                }
            }
            return Task.FromResult(0);
        }

        private async Task Send(PubSubPacketModel packet)
        {
            await this.Send(JSONSerializerHelper.SerializeToString(packet));
        }
    }
}
