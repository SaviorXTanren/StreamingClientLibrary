using Mixer.Base.Model.Channel;
using Mixer.Base.Model.Chat;
using Mixer.Base.Model.Client;
using Mixer.Base.Model.User;
using Mixer.Base.Util;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mixer.Base.Clients
{
    /// <summary>
    /// The real-time client for Chat interactions.
    /// </summary>
    public class ChatClient : MixerWebSocketClientBase
    {
        /// <summary>
        /// This event is triggered when a new chat message arrives.
        /// </summary>
        public event EventHandler<ChatMessageEventModel> OnMessageOccurred;

        /// <summary>
        /// This event is triggered when a new chat message arrives.
        /// </summary>
        public event EventHandler<ChatUserEventModel> OnUserJoinOccurred;

        /// <summary>
        /// This event is triggered when a user leaves chat.
        /// </summary>
        public event EventHandler<ChatUserEventModel> OnUserLeaveOccurred;

        /// <summary>
        /// This event is triggered when a user in chat is updated.
        /// </summary>
        public event EventHandler<ChatUserEventModel> OnUserUpdateOccurred;

        /// <summary>
        /// This event is triggered when a user in chat is timed out.
        /// </summary>
        public event EventHandler<ChatUserEventModel> OnUserTimeoutOccurred;

        /// <summary>
        /// This event is triggered when a chat poll starts.
        /// </summary>
        public event EventHandler<ChatPollEventModel> OnPollStartOccurred;

        /// <summary>
        /// This event is triggered when a chat poll ends.
        /// </summary>
        public event EventHandler<ChatPollEventModel> OnPollEndOccurred;

        /// <summary>
        /// This event is triggered when a chat message is deleted.
        /// </summary>
        public event EventHandler<ChatDeleteMessageEventModel> OnDeleteMessageOccurred;

        /// <summary>
        /// This event is triggered when a chat message is purged.
        /// </summary>
        public event EventHandler<ChatPurgeMessageEventModel> OnPurgeMessageOccurred;

        /// <summary>
        /// This event is triggered when chat is cleared.
        /// </summary>
        public event EventHandler<ChatClearMessagesEventModel> OnClearMessagesOccurred;

        /// <summary>
        /// The channel that for this chat.
        /// </summary>
        public ChannelModel Channel { get; private set; }

        /// <summary>
        /// The user this chat belongs to.
        /// </summary>
        public UserModel User { get; private set; }

        private ChannelChatModel channelChat;

        /// <summary>
        /// Creates a chat client for the specified connection and channel.
        /// </summary>
        /// <param name="connection">The connection to use for client creation</param>
        /// <param name="channel">The channel to connect to</param>
        /// <returns>The chat client for the specified channel</returns>
        public static async Task<ChatClient> CreateFromChannel(MixerConnection connection, ChannelModel channel)
        {
            Validator.ValidateVariable(connection, "connection");
            Validator.ValidateVariable(channel, "channel");

            UserModel user = await connection.Users.GetCurrentUser();
            ChannelChatModel chat = await connection.Chats.GetChat(channel);

            return new ChatClient(channel, user, chat);
        }

        private ChatClient(ChannelModel channel, UserModel user, ChannelChatModel channelChat)
        {
            Validator.ValidateVariable(channel, "channel");
            Validator.ValidateVariable(user, "user");
            Validator.ValidateVariable(channelChat, "channelChat");

            this.Channel = channel;
            this.User = user;
            this.channelChat = channelChat;
        }

        /// <summary>
        /// Connects to the channel Chat service.
        /// </summary>
        /// <returns>Whether the operation succeeded</returns>
        public async Task<bool> Connect()
        {
            Random random = new Random();
            int endpointToUse = random.Next(this.channelChat.endpoints.Count());
            return await this.Connect(this.channelChat.endpoints[endpointToUse]);
        }

        /// <summary>
        /// Connects to the channel Chat service.
        /// </summary>
        /// <param name="endpoint">The endpoint to connect to</param>
        /// <returns>Whether the operation succeeded</returns>
        public override async Task<bool> Connect(string endpoint)
        {
            this.OnEventOccurred -= ChatClient_OnEventOccurred;

            this.OnEventOccurred += ConnectEventHandler;

            await base.Connect(endpoint);

            await this.WaitForResponse(() => { return this.Connected; });

            this.OnEventOccurred -= ConnectEventHandler;

            if (this.Connected)
            {
                this.OnEventOccurred += ChatClient_OnEventOccurred;
            }

            return this.Connected;
        }

        /// <summary>
        /// Authenticates the currently logged in user with the channel's chat.
        /// </summary>
        /// <returns>Whether the operation succeeded</returns>
        public async Task<bool> Authenticate()
        {
            MethodPacket packet = new MethodPacket()
            {
                method = "auth",
                arguments = new JArray() { this.Channel.id.ToString(), this.User.id.ToString(), this.channelChat.authkey },
            };

            this.Authenticated = false;

            ReplyPacket reply = await this.SendAndListen(packet, checkIfAuthenticated: false);
            if (reply != null && reply.dataObject.TryGetValue("authenticated", out JToken value))
            {
                this.Authenticated = (bool)value;
            }

            return this.Authenticated;
        }

        /// <summary>
        /// Pings the Mixer Chat service
        /// </summary>
        /// <returns>Whether the service can be contacted or not</returns>
        public async Task<bool> Ping()
        {
            return this.VerifyNoErrors(await this.SendAndListen(new MethodPacket("ping")));
        }

        /// <summary>
        /// Sends a message.
        /// </summary>
        /// <param name="message">The message to send</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        public async Task SendMessage(string message)
        {
            await this.Send(this.CreateSendMessagePacket(message));
        }

        /// <summary>
        /// Sends a message.
        /// </summary>
        /// <param name="message">The message to send</param>
        /// <returns>The event of the message</returns>
        public async Task<ChatMessageEventModel> SendMessageWithResponse(string message)
        {
            return await this.SendAndListen<ChatMessageEventModel>(this.CreateSendMessagePacket(message));
        }

        private MethodPacket CreateSendMessagePacket(string message)
        {
            Validator.ValidateString(message, "message");
            return new MethodArgPacket("msg", new JArray() { message });
        }

        /// <summary>
        /// Sends a whisper to the specified username.
        /// </summary>
        /// <param name="username">The username to whisper</param>
        /// <param name="message">The message to send</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        public async Task Whisper(string username, string message)
        {
            await this.Send(this.BuildWhisperPacket(username, message));
        }

        /// <summary>
        /// Sends a whisper to the specified username.
        /// </summary>
        /// <param name="username">The username to whisper</param>
        /// <param name="message">The message to send</param>
        /// <returns>The event of the whisper</returns>
        public async Task<ChatMessageEventModel> WhisperWithResponse(string username, string message)
        {
            return await this.SendAndListen<ChatMessageEventModel>(this.BuildWhisperPacket(username, message));
        }

        private MethodPacket BuildWhisperPacket(string username, string message)
        {
            Validator.ValidateString(username, "username");
            Validator.ValidateString(message, "message");
            return new MethodArgPacket("whisper", new JArray() { username, message });
        }

        /// <summary>
        /// Starts a vote.
        /// </summary>
        /// <param name="question">The question to ask</param>
        /// <param name="options">The available choices to select from</param>
        /// <param name="durationInSeconds">The duration in seconds</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        public async Task StartVote(string question, IEnumerable<string> options, uint durationInSeconds)
        {
            await this.Send(this.BuildStartVotePacket(question, options, durationInSeconds));
        }

        /// <summary>
        /// Starts a vote.
        /// </summary>
        /// <param name="question">The question to ask</param>
        /// <param name="options">The available choices to select from</param>
        /// <param name="durationInSeconds">The duration in seconds</param>
        /// <returns>Whether the operation succeeded</returns>
        public async Task<bool> StartVoteWithResponse(string question, IEnumerable<string> options, uint durationInSeconds)
        {
            return this.VerifyDataExists(await this.SendAndListen(this.BuildStartVotePacket(question, options, durationInSeconds)));
        }

        private MethodPacket BuildStartVotePacket(string question, IEnumerable<string> options, uint durationInSeconds)
        {
            Validator.ValidateString(question, "question");
            Validator.ValidateList(options, "options");
            return new MethodArgPacket("vote:start", new JArray() { question, new JArray() { options }, durationInSeconds });
        }

        /// <summary>
        /// Selects an choice for the current vote
        /// </summary>
        /// <param name="optionIndex">The index of the choice</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        public async Task ChooseVote(uint optionIndex)
        {
            await this.Send(this.BuildChooseVotePacket(optionIndex));
        }

        /// <summary>
        /// Selects an choice for the current vote
        /// </summary>
        /// <param name="optionIndex">The index of the choice</param>
        /// <returns>Whether the operation succeeded</returns>
        public async Task<bool> ChooseVoteWithResponse(uint optionIndex)
        {
            return this.VerifyDataExists(await this.SendAndListen(this.BuildChooseVotePacket(optionIndex)));
        }

        private MethodPacket BuildChooseVotePacket(uint optionIndex)
        {
            return new MethodArgPacket("vote:choose", new JArray() { optionIndex });
        }

        /// <summary>
        /// Times out the user for the specified duration.
        /// </summary>
        /// <param name="username">The username to time out</param>
        /// <param name="durationInSeconds">The duration of the time out</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        public async Task TimeoutUser(string username, uint durationInSeconds)
        {
            await this.Send(this.BuildTimeoutUserPacket(username, durationInSeconds));
        }

        /// <summary>
        /// Times out the user for the specified duration.
        /// </summary>
        /// <param name="username">The username to time out</param>
        /// <param name="durationInSeconds">The duration of the time out</param>
        /// <returns>Whether the operation succeeded</returns>
        public async Task<bool> TimeoutUserWithResponse(string username, uint durationInSeconds)
        {
            return this.VerifyDataExists(await this.SendAndListen(this.BuildTimeoutUserPacket(username, durationInSeconds)));
        }

        private MethodPacket BuildTimeoutUserPacket(string username, uint durationInSeconds)
        {
            Validator.ValidateString(username, "username");
            return new MethodArgPacket("timeout", new JArray() { username, durationInSeconds.ToString() });
        }

        /// <summary>
        /// Purges all messages in chat from the specified user.
        /// </summary>
        /// <param name="username">The username to purge</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        public async Task PurgeUser(string username)
        {
            await this.Send(this.BuildPurgeUserPacket(username));
        }

        /// <summary>
        /// Purges all messages in chat from the specified user.
        /// </summary>
        /// <param name="username">The username to purge</param>
        /// <returns>Whether the operation succeeded</returns>
        public async Task<bool> PurgeUserWithResponse(string username)
        {
            return this.VerifyNoErrors(await this.SendAndListen(this.BuildPurgeUserPacket(username)));
        }

        private MethodPacket BuildPurgeUserPacket(string username)
        {
            Validator.ValidateString(username, "username");
            return new MethodArgPacket("purge", new JArray() { username });
        }

        /// <summary>
        /// Deletes the specified message.
        /// </summary>
        /// <param name="messageID">The id of the message to delete</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        public async Task DeleteMessage(Guid messageID)
        {
            await this.Send(this.BuildDeleteMessagePacket(messageID));
        }

        /// <summary>
        /// Deletes the specified message.
        /// </summary>
        /// <param name="messageID">The id of the message to delete</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        public async Task<bool> DeleteMessageWithResponse(Guid messageID)
        {
            return this.VerifyDataExists(await this.SendAndListen(this.BuildDeleteMessagePacket(messageID)));
        }

        private MethodPacket BuildDeleteMessagePacket(Guid messageID)
        {
            return new MethodArgPacket("deleteMessage", new JArray() { messageID });
        }

        /// <summary>
        /// Clears all messages from chat.
        /// </summary>
        /// <returns>The task object representing the asynchronous operation</returns>
        public async Task ClearMessages()
        {
            await this.Send(this.BuildClearMessagesPacket());
        }

        /// <summary>
        /// Clears all messages from chat.
        /// </summary>
        /// <returns>Whether the operation succeeded</returns>
        public async Task<bool> ClearMessagesWithResponse()
        {
            return this.VerifyDataExists(await this.SendAndListen(this.BuildClearMessagesPacket()));
        }

        private MethodPacket BuildClearMessagesPacket()
        {
            return new MethodArgPacket("clearMessages", new JArray() { });
        }

        private void ConnectEventHandler(object sender, EventPacket e)
        {
            if (e.eventName.Equals("WelcomeEvent"))
            {
                this.Connected = true;
            }
        }

        private void ChatClient_OnEventOccurred(object sender, EventPacket eventPacket)
        {
            switch (eventPacket.eventName)
            {
                case "ChatMessage":
                    this.SendSpecificEvent(eventPacket, this.OnMessageOccurred);
                    break;

                case "UserJoin":
                    this.SendSpecificEvent(eventPacket, this.OnUserJoinOccurred);
                    break;
                case "UserLeave":
                    this.SendSpecificEvent(eventPacket, this.OnUserLeaveOccurred);
                    break;
                case "UserUpdate":
                    this.SendSpecificEvent(eventPacket, this.OnUserUpdateOccurred);
                    break;
                case "UserTimeout":
                    this.SendSpecificEvent(eventPacket, this.OnUserTimeoutOccurred);
                    break;

                case "PollStart":
                    this.SendSpecificEvent(eventPacket, this.OnPollStartOccurred);
                    break;
                case "PollEnd":
                    this.SendSpecificEvent(eventPacket, this.OnPollEndOccurred);
                    break;

                case "DeleteMessage":
                    this.SendSpecificEvent(eventPacket, this.OnDeleteMessageOccurred);
                    break;
                case "PurgeMessage":
                    this.SendSpecificEvent(eventPacket, this.OnPurgeMessageOccurred);
                    break;
                case "ClearMessages":
                    this.SendSpecificEvent(eventPacket, this.OnClearMessagesOccurred);
                    break;
            }
        }
    }
}
