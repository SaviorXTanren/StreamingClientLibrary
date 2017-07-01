using Mixer.Base.Model.Channel;
using Mixer.Base.Model.Chat;
using Mixer.Base.Model.Client;
using Mixer.Base.Model.User;
using Mixer.Base.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Mixer.Base.Clients
{
    public class ChatClient : WebSocketClientBase
    {
        public event EventHandler<ChatMessageEventModel> OnMessageOccurred;

        public event EventHandler<ChatUserEventModel> OnUserJoinOccurred;
        public event EventHandler<ChatUserEventModel> OnUserLeaveOccurred;
        public event EventHandler<ChatUserEventModel> OnUserUpdateOccurred;
        public event EventHandler<ChatUserEventModel> OnUserTimeoutOccurred;

        public event EventHandler<ChatPollEventModel> OnPollStartOccurred;
        public event EventHandler<ChatPollEventModel> OnPollEndOccurred;

        public event EventHandler<Guid> OnDeleteMessageOccurred;
        public event EventHandler<uint> OnPurgeMessageOccurred;
        public event EventHandler OnClearMessagesOccurred;

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

        public async Task SendMessage(string message)
        {
            MethodPacket packet = new MethodPacket()
            {
                method = "msg",
                arguments = new JArray() { message },
            };
            await this.Send(packet);
        }

        public async Task Whisper(string username, string message)
        {
            MethodPacket packet = new MethodPacket()
            {
                method = "whisper",
                arguments = new JArray() { username, message },
            };
            await this.Send(packet);
        }

        // TODO: Need to figure out how to get correct permissions for command to work
        public async Task StartVote(string question, IEnumerable<string> options, uint timeLimit)
        {
            MethodPacket packet = new MethodPacket()
            {
                method = "vote:start",
                arguments = new JArray() { question, options, timeLimit },
            };
            await this.Send(packet);
        }

        // TODO: Need to figure out how to get correct permissions for command to work
        public async Task ChooseVote(uint optionIndex)
        {
            MethodPacket packet = new MethodPacket()
            {
                method = "vote",
                arguments = new JArray() { optionIndex },
            };
            await this.Send(packet);
        }

        // TODO: Need to figure out how to get correct permissions for command to work
        public async Task TimeoutUser(string username, uint durationInSeconds)
        {
            MethodPacket packet = new MethodPacket()
            {
                method = "timeout",
                arguments = new JArray() { username, durationInSeconds },
            };
            await this.Send(packet);
        }

        // TODO: Need to figure out how to get correct permissions for command to work
        public async Task PurgeUser(string username)
        {
            MethodPacket packet = new MethodPacket()
            {
                method = "purge",
                arguments = new JArray() { username },
            };
            await this.Send(packet);
        }

        // TODO: Need to figure out how to get correct permissions for command to work
        public async Task DeleteMessage(Guid messageID)
        {
            MethodPacket packet = new MethodPacket()
            {
                method = "purge",
                arguments = new JArray() { messageID },
            };
            await this.Send(packet);
        }

        // TODO: Need to figure out how to get correct permissions for command to work
        public async Task ClearMessages()
        {
            MethodPacket packet = new MethodPacket()
            {
                method = "clearMessages",
                arguments = new JArray(),
            };
            await this.Send(packet);
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
            if (e.id == (this.CurrentPacketID - 1) && e.data.TryGetValue("authenticated", out value) && (bool)value)
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
                    if (this.OnClearMessagesOccurred != null) { this.OnClearMessagesOccurred(this, new EventArgs()); }
                    break;
            }
        }

        private void SendSpecificEvent<T>(EventPacket eventPacket, EventHandler<T> eventHandler)
        {
            if (eventHandler != null)
            {
                eventHandler(this, JsonConvert.DeserializeObject<T>(eventPacket.data.ToString()));
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
