using Mixer.Base.Model.Channel;
using Mixer.Base.Model.Chat;
using Mixer.Base.Model.Client;
using Mixer.Base.Model.User;
using Mixer.Base.Util;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Mixer.Base.Clients
{
    /// <summary>
    /// The real-time client for Chat interactions.
    /// </summary>
    public class ChatClient : WebSocketClientBase
    {
        public event EventHandler<ChatMessageEventModel> OnMessageOccurred;

        public event EventHandler<ChatUserEventModel> OnUserJoinOccurred;
        public event EventHandler<ChatUserEventModel> OnUserLeaveOccurred;
        public event EventHandler<ChatUserEventModel> OnUserUpdateOccurred;
        public event EventHandler<ChatUserEventModel> OnUserTimeoutOccurred;

        public event EventHandler<ChatPollEventModel> OnPollStartOccurred;
        public event EventHandler<ChatPollEventModel> OnPollEndOccurred;

        public event EventHandler<ChatDeleteMessageEventModel> OnDeleteMessageOccurred;
        public event EventHandler<ChatPurgeMessageEventModel> OnPurgeMessageOccurred;
        public event EventHandler<ChatClearMessagesEventModel> OnClearMessagesOccurred;

        public ChannelModel Channel { get; private set; }
        public UserModel User { get; private set; }

        private ChannelChatModel channelChat;

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

        public async Task<bool> Connect()
        {
            this.OnDisconnectOccurred -= ChatClient_OnDisconnectOccurred;
            this.OnEventOccurred -= ChatClient_OnEventOccurred;

            int totalEndpoints = this.channelChat.endpoints.Count();
            Random random = new Random();
            int endpointToUse = random.Next() % totalEndpoints;

            this.OnEventOccurred += ConnectEventHandler;

            await this.ConnectInternal(this.channelChat.endpoints[endpointToUse]);

            await this.WaitForResponse(() => { return this.connectSuccessful; });

            this.OnEventOccurred -= ConnectEventHandler;

            if (this.connectSuccessful)
            {
                this.OnDisconnectOccurred += ChatClient_OnDisconnectOccurred;
                this.OnEventOccurred += ChatClient_OnEventOccurred;
            }

            return this.connectSuccessful;
        }

        public async Task<bool> Authenticate()
        {
            MethodPacket packet = new MethodPacket()
            {
                method = "auth",
                arguments = new JArray() { this.Channel.id.ToString(), this.User.id.ToString(), this.channelChat.authkey },
            };

            this.authenticateSuccessful = false;
            this.OnReplyOccurred += AuthenticateEventHandler;

            await this.Send(packet, checkIfAuthenticated: false);

            await this.WaitForResponse(() => { return this.authenticateSuccessful; });

            this.OnReplyOccurred -= AuthenticateEventHandler;

            return this.authenticateSuccessful;
        }

        public async Task<ChatMessageEventModel> SendMessage(string message)
        {
            MethodPacket packet = new MethodPacket()
            {
                method = "msg",
                arguments = new JArray() { message },
            };
            ReplyPacket reply = await this.SendAndListen(packet);
            return this.GetSpecificReplyResultValue<ChatMessageEventModel>(reply);    
        }

        public async Task<ChatMessageEventModel> Whisper(string username, string message)
        {
            MethodPacket packet = new MethodPacket()
            {
                method = "whisper",
                arguments = new JArray() { username, message },
            };
            ReplyPacket reply = await this.SendAndListen(packet);
            return this.GetSpecificReplyResultValue<ChatMessageEventModel>(reply);
        }

        // TODO: Need to figure out how to get correct permissions for command to work
        public async Task<bool> StartVote(string question, IEnumerable<string> options, uint timeLimit)
        {
            MethodPacket packet = new MethodPacket()
            {
                method = "vote:start",
                arguments = new JArray() { question, new JArray() { options }, timeLimit },
            };
            ReplyPacket reply = await this.SendAndListen(packet);
            return this.VerifyDataExists(reply);
        }

        public async Task<bool> ChooseVote(uint optionIndex)
        {
            MethodPacket packet = new MethodPacket()
            {
                method = "vote:choose",
                arguments = new JArray() { optionIndex },
            };
            ReplyPacket reply = await this.SendAndListen(packet);
            return this.VerifyDataExists(reply);
        }

        public async Task<bool> TimeoutUser(string username, uint durationInSeconds)
        {
            MethodPacket packet = new MethodPacket()
            {
                method = "timeout",
                arguments = new JArray() { username, durationInSeconds.ToString() },
            };
            ReplyPacket reply = await this.SendAndListen(packet);
            return this.VerifyDataExists(reply);
        }

        public async Task<bool> PurgeUser(string username)
        {
            MethodPacket packet = new MethodPacket()
            {
                method = "purge",
                arguments = new JArray() { username },
            };
            ReplyPacket reply = await this.SendAndListen(packet);
            return this.VerifyNoErrors(reply);
        }

        public async Task<bool> DeleteMessage(Guid messageID)
        {
            MethodPacket packet = new MethodPacket()
            {
                method = "deleteMessage",
                arguments = new JArray() { messageID },
            };
            ReplyPacket reply = await this.SendAndListen(packet);
            return this.VerifyDataExists(reply);
        }

        public async Task<bool> ClearMessages()
        {
            MethodPacket packet = new MethodPacket()
            {
                method = "clearMessages",
                arguments = new JArray(),
            };
            ReplyPacket reply = await this.SendAndListen(packet);
            return this.VerifyDataExists(reply);
        }

        private void ConnectEventHandler(object sender, EventPacket e)
        {
            if (e.eventName.Equals("WelcomeEvent"))
            {
                this.connectSuccessful = true;
            }
        }

        private void AuthenticateEventHandler(object sender, ReplyPacket e)
        {
            JToken value;
            if (e.id == (this.CurrentPacketID - 1) && e.dataObject.TryGetValue("authenticated", out value) && (bool)value)
            {
                this.authenticateSuccessful = true;
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

        private async void ChatClient_OnDisconnectOccurred(object sender, WebSocketCloseStatus e)
        {
            this.connectSuccessful = false;
            this.authenticateSuccessful = false;
            if (await this.Connect())
            {
                await this.Authenticate();
            }
        }
    }
}
