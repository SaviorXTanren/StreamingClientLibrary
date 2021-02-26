using Glimesh.Base.Models.Clients;
using Glimesh.Base.Models.Clients.Channel;
using Glimesh.Base.Models.Clients.Chat;
using Newtonsoft.Json.Linq;
using StreamingClient.Base.Model.OAuth;
using StreamingClient.Base.Util;
using StreamingClient.Base.Web;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Glimesh.Base.Clients
{
    /// <summary>
    /// Web Socket client for interacting with Chat &amp; Event services.
    /// </summary>
    public class ChatEventClient : ClientWebSocketBase
    {
        private const string CONNECTION_URL_BASE = "wss://glimesh.tv/api/socket/websocket?vsn=2.0.0";
        private const string TOKEN_CONNECTION_URL = CONNECTION_URL_BASE + "&token={0}";
        private const string CLIENT_ID_CONNECTION_URL = CONNECTION_URL_BASE + "client_id={0}";

        /// <summary>
        /// Invoked when a chat message is received.
        /// </summary>
        public event EventHandler<ChatMessagePacketModel> OnChatMessageReceived;

        /// <summary>
        /// Invoked when a channel is updated.
        /// </summary>
        public event EventHandler<ChannelUpdatePacketModel> OnChannelUpdated;

        private GlimeshConnection connection;
        private string connectionUrl;

        private CancellationTokenSource backgroundPingCancellationTokenSource;

        private Dictionary<string, ClientResponsePacketModel> replyIDListeners = new Dictionary<string, ClientResponsePacketModel>();

        private HashSet<string> chatSubscriptions = new HashSet<string>();
        private HashSet<string> channelSubscriptions = new HashSet<string>();

        /// <summary>
        /// Creates a client using the user's acquired OAuth token.
        /// </summary>
        /// <returns>The chat client</returns>
        public static async Task<ChatEventClient> CreateWithToken(GlimeshConnection connection)
        {
            OAuthTokenModel oauthToken = await connection.GetOAuthToken();
            return new ChatEventClient(connection, string.Format(TOKEN_CONNECTION_URL, oauthToken.accessToken));
        }

        /// <summary>
        /// Creates a client using the user's acquired OAuth token.
        /// </summary>
        /// <returns>The chat client</returns>
        public static async Task<ChatEventClient> CreateWithClientID(GlimeshConnection connection)
        {
            OAuthTokenModel oauthToken = await connection.GetOAuthToken();
            return new ChatEventClient(connection, string.Format(CLIENT_ID_CONNECTION_URL, oauthToken.clientID));
        }

        /// <summary>
        /// Creates a new instance of the ChatEventClient class.
        /// </summary>
        /// <param name="connection">The current connection</param>
        /// <param name="connectionUrl">The URL to connect with</param>
        private ChatEventClient(GlimeshConnection connection, string connectionUrl)
        {
            this.connection = connection;
            this.connectionUrl = connectionUrl;
        }

        /// <summary>
        /// Connects to the web socket servers.
        /// </summary>
        /// <returns>Whether the connection was successful</returns>
        public async Task<bool> Connect()
        {
            if (await this.Connect(this.connectionUrl))
            {
                ClientResponsePacketModel response = await this.SendAndListen(new ClientConnectPacketModel());
                if (response != null && response.IsPayloadStatusOk)
                {
                    this.backgroundPingCancellationTokenSource = new CancellationTokenSource();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    Task.Run(() => this.BackgroundPing(this.backgroundPingCancellationTokenSource.Token), this.backgroundPingCancellationTokenSource.Token);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Disconnects the web socket.
        /// </summary>
        /// <returns>A task for the closing of the web socket</returns>
        public async Task Disconnect()
        {
            if (this.backgroundPingCancellationTokenSource != null)
            {
                this.backgroundPingCancellationTokenSource.Cancel();
                this.backgroundPingCancellationTokenSource = null;
            }
            await base.Disconnect();
        }

        /// <summary>
        /// Joins the specified channel's chat.
        /// </summary>
        /// <param name="channelID">The ID of the channel to join</param>
        /// <returns>Whether the connection was successful</returns>
        public async Task<bool> JoinChannelChat(string channelID)
        {
            Validator.ValidateString(channelID, "channelID");

            // Join Chat
            ClientResponsePacketModel chatJoinResponse = await this.SendAndListen(new ChatJoinPacketModel(channelID));
            if (chatJoinResponse == null || !chatJoinResponse.IsPayloadStatusOk)
            {
                return false;
            }

            JToken chatJoinSubscription = chatJoinResponse.Payload.SelectToken("response.subscriptionId");
            if (chatJoinSubscription == null)
            {
                return false;
            }
            this.chatSubscriptions.Add(chatJoinSubscription.ToString());

            return true;
        }

        /// <summary>
        /// Joins the specified channel's events.
        /// </summary>
        /// <param name="channelID">The ID of the channel to join</param>
        /// <returns>Whether the connection was successful</returns>
        public async Task<bool> JoinChannelEvents(string channelID)
        {
            Validator.ValidateString(channelID, "channelID");

            // Join Channel Events
            ClientResponsePacketModel channelJoinResponse = await this.SendAndListen(new ChannelJoinPacketModel(channelID));
            if (channelJoinResponse == null || !channelJoinResponse.IsPayloadStatusOk)
            {
                return false;
            }

            JToken channelJoinSubscription = channelJoinResponse.Payload.SelectToken("response.subscriptionId");
            if (channelJoinSubscription == null)
            {
                return false;
            }
            this.channelSubscriptions.Add(channelJoinSubscription.ToString());

            return true;
        }

        /// <summary>
        /// Sends a heartbeat to the web socket servers.
        /// </summary>
        /// <returns>An awaitable Task</returns>
        public async Task<bool> SendHeartbeat()
        {
            ClientResponsePacketModel response = await this.SendAndListen(new ClientHeartbeatPacketModel());
            return response != null && response.IsPayloadStatusOk;
        }

        /// <summary>
        /// Sends a plain-text message the specified channel's chat.
        /// </summary>
        /// <param name="channelID">The ID of the channel to send the message to</param>
        /// <param name="message">The plain-text message to send</param>
        /// <returns>Whether the message was successful</returns>
        public async Task<bool> SendMessage(string channelID, string message)
        {
            Validator.ValidateString(channelID, "channelID");
            Validator.ValidateString(message, "message");

            ClientResponsePacketModel response = await this.SendAndListen(new ChatSendMessagePacketModel(channelID, message));
            return response != null && response.IsPayloadStatusOk;
        }

        /// <summary>
        /// Times out a user for a short period of time (5 minutes) for the specified channel.
        /// </summary>
        /// <param name="channelID">The ID of the channel to send the message to</param>
        /// <param name="userID">The ID of the user to timeout</param>
        /// <returns>Whether the message was successful</returns>
        public async Task<bool> ShortTimeoutUser(string channelID, string userID)
        {
            Validator.ValidateString(channelID, "channelID");
            Validator.ValidateString(userID, "userID");

            ClientResponsePacketModel response = await this.SendAndListen(new ChatShortTimeoutUserPacketModel(channelID, userID));
            return response != null && response.IsPayloadStatusOk;
        }

        /// <summary>
        /// Times out a user for a long period of time (15 minutes) for the specified channel.
        /// </summary>
        /// <param name="channelID">The ID of the channel to send the message to</param>
        /// <param name="userID">The ID of the user to timeout</param>
        /// <returns>Whether the message was successful</returns>
        public async Task<bool> LongTimeoutUser(string channelID, string userID)
        {
            Validator.ValidateString(channelID, "channelID");
            Validator.ValidateString(userID, "userID");

            ClientResponsePacketModel response = await this.SendAndListen(new ChatLongTimeoutUserPacketModel(channelID, userID));
            return response != null && response.IsPayloadStatusOk;
        }

        /// <summary>
        /// Bans a user from the specified channel.
        /// </summary>
        /// <param name="channelID">The ID of the channel to send the message to</param>
        /// <param name="userID">The ID of the user to ban</param>
        /// <returns>Whether the message was successful</returns>
        public async Task<bool> BanUser(string channelID, string userID)
        {
            Validator.ValidateString(channelID, "channelID");
            Validator.ValidateString(userID, "userID");

            ClientResponsePacketModel response = await this.SendAndListen(new ChatBanUserPacketModel(channelID, userID));
            return response != null && response.IsPayloadStatusOk;
        }

        /// <summary>
        /// Unbans a user from the specified channel.
        /// </summary>
        /// <param name="channelID">The ID of the channel to send the message to</param>
        /// <param name="userID">The ID of the user to unban</param>
        /// <returns>Whether the message was successful</returns>
        public async Task<bool> UnbanUser(string channelID, string userID)
        {
            Validator.ValidateString(channelID, "channelID");
            Validator.ValidateString(userID, "userID");

            ClientResponsePacketModel response = await this.SendAndListen(new ChatUnbanUserPacketModel(channelID, userID));
            return response != null && response.IsPayloadStatusOk;
        }

        /// <summary>
        /// Processes the received text packet.
        /// </summary>
        /// <param name="packetMessage">The receive text packet</param>
        /// <returns>An awaitable task</returns>
        protected override Task ProcessReceivedPacket(string packetMessage)
        {
            if (!string.IsNullOrEmpty(packetMessage))
            {
                ClientResponsePacketModel packet = new ClientResponsePacketModel(packetMessage);

                if (packet.IsReplyEvent)
                {
                    if (this.replyIDListeners.ContainsKey(packet.NormalRef))
                    {
                        this.replyIDListeners[packet.NormalRef] = packet;
                    }
                }

                if (this.chatSubscriptions.Contains(packet.Topic))
                {
                    ChatMessagePacketModel message = new ChatMessagePacketModel(packetMessage);
                    this.OnChatMessageReceived?.Invoke(this, message);
                }
                else if (this.channelSubscriptions.Contains(packet.Topic))
                {
                    ChannelUpdatePacketModel channelUpdate = new ChannelUpdatePacketModel(packetMessage);
                    this.OnChannelUpdated?.Invoke(this, channelUpdate);
                }
            }
            return Task.FromResult(0);
        }

        private async Task Send(ClientPacketModelBase packet)
        {
            await this.Send(packet.ToSerializedPacketArray());
        }

        private async Task<ClientResponsePacketModel> SendAndListen(ClientPacketModelBase packet)
        {
            ClientResponsePacketModel replyPacket = null;
            this.replyIDListeners[packet.NormalRef] = null;
            await this.Send(packet);

            await this.WaitForSuccess(() =>
            {
                if (this.replyIDListeners.ContainsKey(packet.NormalRef) && this.replyIDListeners[packet.NormalRef] != null)
                {
                    replyPacket = this.replyIDListeners[packet.NormalRef];
                    return true;
                }
                return false;
            }, secondsToWait: 5);

            this.replyIDListeners.Remove(packet.NormalRef);
            return replyPacket;
        }

        private async Task BackgroundPing(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(15000);

                    if (!await this.SendHeartbeat())
                    {
                        await this.Disconnect();
                        return;
                    }
                }
                catch (ThreadAbortException) { return; }
                catch (OperationCanceledException) { return; }
                catch (Exception ex) { Logger.Log(ex); }
            }
        }
    }
}
