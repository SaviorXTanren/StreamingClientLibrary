using Mixer.Base.Model.Channel;
using Mixer.Base.Model.Chat;
using Mixer.Base.Model.User;
using Mixer.Base.Util;
using Mixer.Base.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Mixer.Base
{
    public class ChatClient : WebSocketClientBase
    {
        public event EventHandler<ChatReplyPacket> ReplyOccurred;
        public event EventHandler<ChatEventPacket> EventOccurred;

        public event EventHandler<ChatMessageEventModel> MessageOccurred;

        public event EventHandler<ChatUserEventModel> UserJoinOccurred;
        public event EventHandler<ChatUserEventModel> UserLeaveOccurred;
        public event EventHandler<ChatUserEventModel> UserUpdateOccurred;
        public event EventHandler<ChatUserEventModel> UserTimeoutOccurred;

        public event EventHandler<ChatPollEventModel> PollStartOccurred;
        public event EventHandler<ChatPollEventModel> PollEndOccurred;

        public event EventHandler<Guid> DeleteMessageOccurred;
        public event EventHandler<uint> PurgeMessageOccurred;
        public event EventHandler ClearMessagesOccurred;

        public ChannelModel Channel { get; private set; }
        public UserModel User { get; private set; }

        private ChannelChatModel channelChat;

        public static async Task<ChatClient> CreateFromChannel(MixerClient client, ChannelModel channel)
        {
            Validator.ValidateVariable(client, "client");
            Validator.ValidateVariable(channel, "channel");

            UserModel user = await client.Users.GetCurrentUser();
            ChannelChatModel chat = await client.Chats.GetChat(channel);

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
            this.DisconnectOccurred -= ChatClient_DisconnectOccurred;

            int totalEndpoints = this.channelChat.endpoints.Count();
            Random random = new Random();
            int endpointToUse = random.Next() % totalEndpoints;

            this.EventOccurred += ConnectEventHandler;

            await this.ConnectInternal(this.channelChat.endpoints[endpointToUse]);

            await this.WaitForResponse(() => { return this.connectSuccessful; });

            this.EventOccurred -= ConnectEventHandler;

            if (this.connectSuccessful)
            {
                this.DisconnectOccurred += ChatClient_DisconnectOccurred;
            }

            return this.connectSuccessful;
        }

        public async Task<bool> Authenticate()
        {
            ChatMethodPacket packet = new ChatMethodPacket()
            {
                method = "auth",
                arguments = new JArray() { this.Channel.id.ToString(), this.User.id.ToString(), this.channelChat.authkey },
            };

            this.authenticateSuccessful = false;
            this.ReplyOccurred += AuthenticateEventHandler;

            await this.Send(packet, checkIfAuthenticated: false);

            await this.WaitForResponse(() => { return this.authenticateSuccessful; });

            this.ReplyOccurred -= AuthenticateEventHandler;

            return this.authenticateSuccessful;
        }

        public async Task SendMessage(string message)
        {
            ChatMethodPacket packet = new ChatMethodPacket()
            {
                method = "msg",
                arguments = new JArray() { message },
            };
            await this.Send(packet);
        }

        public async Task Whisper(string username, string message)
        {
            ChatMethodPacket packet = new ChatMethodPacket()
            {
                method = "whisper",
                arguments = new JArray() { username, message },
            };
            await this.Send(packet);
        }

        // TODO: Need to figure out how to get correct permissions for command to work
        public async Task StartVote(string question, IEnumerable<string> options, uint timeLimit)
        {
            ChatMethodPacket packet = new ChatMethodPacket()
            {
                method = "vote:start",
                arguments = new JArray() { question, options, timeLimit },
            };
            await this.Send(packet);
        }

        // TODO: Need to figure out how to get correct permissions for command to work
        public async Task ChooseVote(uint optionIndex)
        {
            ChatMethodPacket packet = new ChatMethodPacket()
            {
                method = "vote",
                arguments = new JArray() { optionIndex },
            };
            await this.Send(packet);
        }

        // TODO: Need to figure out how to get correct permissions for command to work
        public async Task TimeoutUser(string username, uint durationInSeconds)
        {
            ChatMethodPacket packet = new ChatMethodPacket()
            {
                method = "timeout",
                arguments = new JArray() { username, durationInSeconds },
            };
            await this.Send(packet);
        }

        // TODO: Need to figure out how to get correct permissions for command to work
        public async Task PurgeUser(string username)
        {
            ChatMethodPacket packet = new ChatMethodPacket()
            {
                method = "purge",
                arguments = new JArray() { username },
            };
            await this.Send(packet);
        }

        // TODO: Need to figure out how to get correct permissions for command to work
        public async Task DeleteMessage(Guid messageID)
        {
            ChatMethodPacket packet = new ChatMethodPacket()
            {
                method = "purge",
                arguments = new JArray() { messageID },
            };
            await this.Send(packet);
        }

        // TODO: Need to figure out how to get correct permissions for command to work
        public async Task ClearMessages()
        {
            ChatMethodPacket packet = new ChatMethodPacket()
            {
                method = "clearMessages",
                arguments = new JArray(),
            };
            await this.Send(packet);
        }

        protected override void Receive(string jsonBuffer)
        {
            ChatPacket packet = JsonConvert.DeserializeObject<ChatPacket>(jsonBuffer);
            if (packet.type.Equals("reply"))
            {
                ChatReplyPacket replyPacket = JsonConvert.DeserializeObject<ChatReplyPacket>(jsonBuffer);
                this.OnReplyOccurred(replyPacket);
            }
            else if (packet.type.Equals("event"))
            {
                ChatEventPacket eventPacket = JsonConvert.DeserializeObject<ChatEventPacket>(jsonBuffer);
                this.OnEventOccurred(eventPacket);
            }
        }

        private void ConnectEventHandler(object sender, ChatEventPacket e)
        {
            if (e.eventName.Equals("WelcomeEvent"))
            {
                this.connectSuccessful = true;
            }
        }

        private void AuthenticateEventHandler(object sender, ChatReplyPacket e)
        {
            JToken value;
            if (e.id == (this.CurrentPacketID - 1) && e.data.TryGetValue("authenticated", out value) && (bool)value)
            {
                this.authenticateSuccessful = true;
            }
        }

        private void OnReplyOccurred(ChatReplyPacket replyPacket)
        {
            if (this.ReplyOccurred != null)
            {
                this.ReplyOccurred(this, replyPacket);
            }
        }

        private void OnEventOccurred(ChatEventPacket eventPacket)
        {
            if (this.EventOccurred != null)
            {
                this.EventOccurred(this, eventPacket);
            }

            switch (eventPacket.eventName)
            {
                case "ChatMessage":
                    this.SendSpecificEvent(eventPacket, this.MessageOccurred);
                    break;

                case "UserJoin":
                    this.SendSpecificEvent(eventPacket, this.UserJoinOccurred);
                    break;
                case "UserLeave":
                    this.SendSpecificEvent(eventPacket, this.UserLeaveOccurred);
                    break;
                case "UserUpdate":
                    this.SendSpecificEvent(eventPacket, this.UserUpdateOccurred);
                    break;
                case "UserTimeout":
                    this.SendSpecificEvent(eventPacket, this.UserTimeoutOccurred);
                    break;

                case "PollStart":
                    this.SendSpecificEvent(eventPacket, this.PollStartOccurred);
                    break;
                case "PollEnd":
                    this.SendSpecificEvent(eventPacket, this.PollEndOccurred);
                    break;

                case "DeleteMessage":
                    this.SendSpecificEvent(eventPacket, this.DeleteMessageOccurred);
                    break;
                case "PurgeMessage":
                    this.SendSpecificEvent(eventPacket, this.PurgeMessageOccurred);
                    break;
                case "ClearMessages":
                    if (this.ClearMessagesOccurred != null) { this.ClearMessagesOccurred(this, new EventArgs()); }
                    break;
            }
        }

        private void SendSpecificEvent<T>(ChatEventPacket eventPacket, EventHandler<T> eventHandler)
        {
            if (eventHandler != null)
            {
                eventHandler(this, JsonConvert.DeserializeObject<T>(eventPacket.data.ToString()));
            }
        }

        private async void ChatClient_DisconnectOccurred(object sender, WebSocketCloseStatus e)
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
