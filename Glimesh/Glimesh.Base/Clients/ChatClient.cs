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
    /// Web Socket client for interacting with Chat service.
    /// </summary>
    public class ChatClient : ClientWebSocketBase
    {
        private const string CHAT_CONNECTION_URL_BASE = "wss://glimesh.tv/api/socket/websocket?vsn=2.0.0";
        private const string TOKEN_CHAT_CONNECTION_URL = CHAT_CONNECTION_URL_BASE + "&token={0}";
        private const string CLIENT_ID_CHAT_CONNECTION_URL = CHAT_CONNECTION_URL_BASE + "client_id={0}";

        /// <summary>
        /// Invoked when a chat message is received.
        /// </summary>
        public event EventHandler<ChatMessagePacketModel> OnMessageReceived;

        private GlimeshConnection connection;
        private string connectionUrl;

        private CancellationTokenSource backgroundPingCancellationTokenSource;

        private Dictionary<string, ChatResponsePacketModel> replyIDListeners = new Dictionary<string, ChatResponsePacketModel>();

        private HashSet<string> chatSubscriptions = new HashSet<string>();

        /// <summary>
        /// Connects to chat using the user's acquired OAuth token.
        /// </summary>
        /// <returns>The chat client</returns>
        public static async Task<ChatClient> CreateWithToken(GlimeshConnection connection)
        {
            OAuthTokenModel oauthToken = await connection.GetOAuthToken();
            return new ChatClient(connection, string.Format(TOKEN_CHAT_CONNECTION_URL, oauthToken.accessToken));
        }

        /// <summary>
        /// Connects to chat using the user's acquired OAuth token.
        /// </summary>
        /// <returns>The chat client</returns>
        public static async Task<ChatClient> CreateWithClientID(GlimeshConnection connection)
        {
            OAuthTokenModel oauthToken = await connection.GetOAuthToken();
            return new ChatClient(connection, string.Format(CLIENT_ID_CHAT_CONNECTION_URL, oauthToken.clientID));
        }

        /// <summary>
        /// Creates a new instance of the ChatClient class.
        /// </summary>
        /// <param name="connection">The current connection</param>
        /// <param name="connectionUrl">The URL to connect with</param>
        private ChatClient(GlimeshConnection connection, string connectionUrl)
        {
            this.connection = connection;
            this.connectionUrl = connectionUrl;
        }

        /// <summary>
        /// Connects to the chat servers.
        /// </summary>
        /// <returns>Whether the connection was successful</returns>
        public async Task<bool> Connect()
        {
            if (await this.Connect(this.connectionUrl))
            {
                ChatResponsePacketModel response = await this.SendAndListen(new ChatConnectPacketModel());
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
        public async Task<bool> Join(string channelID)
        {
            Validator.ValidateString(channelID, "channelID");

            ChatResponsePacketModel response = await this.SendAndListen(new ChatJoinPacketModel(channelID));
            if (response != null && response.IsPayloadStatusOk)
            {
                JToken subscription = response.Payload.SelectToken("response.subscriptionId");
                if (subscription != null)
                {
                    this.chatSubscriptions.Add(subscription.ToString());
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Sends a heartbeat to the chat servers.
        /// </summary>
        /// <returns>An awaitable Task</returns>
        public async Task<bool> SendHeartbeat()
        {
            ChatResponsePacketModel response = await this.SendAndListen(new ChatHeartbeatPacketModel());
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

            ChatResponsePacketModel response = await this.SendAndListen(new ChatSendMessagePacketModel(channelID, message));
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

            ChatResponsePacketModel response = await this.SendAndListen(new ChatShortTimeoutUserPacketModel(channelID, userID));
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

            ChatResponsePacketModel response = await this.SendAndListen(new ChatLongTimeoutUserPacketModel(channelID, userID));
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

            ChatResponsePacketModel response = await this.SendAndListen(new ChatBanUserPacketModel(channelID, userID));
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

            ChatResponsePacketModel response = await this.SendAndListen(new ChatUnbanUserPacketModel(channelID, userID));
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
                ChatResponsePacketModel packet = new ChatResponsePacketModel(packetMessage);

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
                    this.OnMessageReceived?.Invoke(this, message);
                }
            }
            return Task.FromResult(0);
        }

        private async Task Send(ChatPacketModelBase packet)
        {
            await this.Send(packet.ToSerializedChatPacketArray());
        }

        /// <summary>
        /// Sends a packet to the server and listens for a reply.
        /// </summary>
        /// <param name="packet">The packet to send</param>
        /// <returns>An awaitable task with the reply packet</returns>
        private async Task<ChatResponsePacketModel> SendAndListen(ChatPacketModelBase packet)
        {
            ChatResponsePacketModel replyPacket = null;
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
