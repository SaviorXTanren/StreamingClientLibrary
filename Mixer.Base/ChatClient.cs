using Mixer.Base.Model.Channel;
using Mixer.Base.Model.Chat;
using Mixer.Base.Model.User;
using Mixer.Base.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mixer.Base
{
    public class ChatClient : IDisposable
    {
        public event EventHandler<ChatReplyPacket> ReplyOccurred;
        public event EventHandler<ChatEventPacket> EventOccurred;
        public event EventHandler<WebSocketCloseStatus> DisconnectOccurred;

        public event EventHandler<ChatMessageEventModel> MessageOccurred;

        public event EventHandler<ChatUserEventModel> UserJoinOccurred;
        public event EventHandler<ChatUserEventModel> UserLeaveOccurred;
        public event EventHandler<ChatUserEventModel> UserUpdateOccurred;
        public event EventHandler<ChatUserEventModel> UserTimeoutOccurred;

        public event EventHandler<ChatPollEventModel> PollStartOccurred;
        public event EventHandler<ChatPollEventModel> PollEndOccurred;

        public event EventHandler<Guid> DeleteMessageOccurred;
        public event EventHandler<uint> PurgeMessageOccurred;
        public event EventHandler<uint> ClearMessagesOccurred;

        public ChannelModel Channel { get; private set; }
        public UserModel User { get; private set; }

        internal uint CurrentPacketID { get; private set; }

        private ChannelChatModel channelChat;

        private ClientWebSocket webSocket = new ClientWebSocket();
        private UTF8Encoding encoder = new UTF8Encoding();

        private bool connectSuccessful = false;
        private bool authenticateSuccessful = false;

        private int bufferSize = 4096 * 20;

        public static async Task<ChatClient> CreateFromChannel(MixerClient client, ChannelModel channel)
        {
            Validator.ValidateVariable(client, "client");
            Validator.ValidateVariable(channel, "channel");

            UserModel user = await client.Users.GetCurrentUser();
            ChannelChatModel chat = await client.Chats.GetChat(channel);

            return new ChatClient(channel, user, chat);
        }

        public ChatClient(ChannelModel channel, UserModel user, ChannelChatModel channelChat)
        {
            Validator.ValidateVariable(channel, "channel");
            Validator.ValidateVariable(user, "user");
            Validator.ValidateVariable(channelChat, "channelChat");

            this.Channel = channel;
            this.User = user;
            this.channelChat = channelChat;

            this.CurrentPacketID = 0;
        }

        public async Task<bool> Connect()
        {
            int totalEndpoints = this.channelChat.endpoints.Count();
            Random random = new Random();
            int endpointToUse = random.Next() % totalEndpoints;

            this.connectSuccessful = false;
            this.EventOccurred += ConnectEventHandler;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            this.Receive();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            await this.webSocket.ConnectAsync(new Uri(this.channelChat.endpoints[endpointToUse]), CancellationToken.None);

            for (int i = 0; i < 10 && !this.connectSuccessful; i++)
            {
                await Task.Delay(500);
            }

            this.EventOccurred -= ConnectEventHandler;

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

            await this.Send(packet);

            for (int i = 0; i < 10 && !this.authenticateSuccessful; i++)
            {
                await Task.Delay(500);
            }

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

        public async Task Whisper(UserModel user, string message)
        {
            ChatMethodPacket packet = new ChatMethodPacket()
            {
                method = "whisper",
                arguments = new JArray() { user.username, message },
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
        public async Task TimeoutUser(UserModel user, uint durationInSeconds)
        {
            ChatMethodPacket packet = new ChatMethodPacket()
            {
                method = "timeout",
                arguments = new JArray() { user.username, durationInSeconds },
            };
            await this.Send(packet);
        }

        // TODO: Need to figure out how to get correct permissions for command to work
        public async Task PurgeUser(UserModel user)
        {
            ChatMethodPacket packet = new ChatMethodPacket()
            {
                method = "purge",
                arguments = new JArray() { user.username },
            };
            await this.Send(packet);
        }

        // TODO: Need to figure out how to get correct permissions for command to work
        public async Task DeleteMessage(ChatMessageEventModel message)
        {
            ChatMethodPacket packet = new ChatMethodPacket()
            {
                method = "purge",
                arguments = new JArray() { message.id },
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

        public async Task Disconnect()
        {
            await this.webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
            this.Dispose();
        }

        public void Dispose()
        {
            if (this.webSocket != null)
            {
                this.webSocket.Dispose();
            }
        }

        private async Task Send(ChatMethodPacket packet)
        {
            packet.id = this.CurrentPacketID;

            string packetJson = JsonConvert.SerializeObject(packet);
            byte[] buffer = this.encoder.GetBytes(packetJson);

            await this.webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);

            this.CurrentPacketID++;
        }

        private async Task Receive()
        {
            await Task.Delay(100);
            while (this.webSocket != null)
            {
                if (this.webSocket.State == WebSocketState.Open)
                {
                    byte[] buffer = new byte[this.bufferSize];
                    WebSocketReceiveResult result = await this.webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result != null)
                    {
                        if (result.CloseStatus == null || result.CloseStatus != WebSocketCloseStatus.Empty)
                        {
                            string jsonBuffer = this.encoder.GetString(buffer);
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
                        else
                        {
                            this.OnDisconnectOccurred(result);
                        }
                    }
                }
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
                    this.SendSpecificEvent(eventPacket, this.ClearMessagesOccurred);
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

        private void OnDisconnectOccurred(WebSocketReceiveResult result)
        {
            if (this.EventOccurred != null)
            {
                this.DisconnectOccurred(this, result.CloseStatus.GetValueOrDefault());
            }
        }
    }
}
