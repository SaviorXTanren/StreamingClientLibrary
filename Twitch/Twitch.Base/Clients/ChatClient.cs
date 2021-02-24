using StreamingClient.Base.Model.OAuth;
using StreamingClient.Base.Util;
using StreamingClient.Base.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twitch.Base.Models.Clients.Chat;
using NewAPI = Twitch.Base.Models.NewAPI;
using V5 = Twitch.Base.Models.V5;

namespace Twitch.Base.Clients
{
    // Twitch IRC logic from: https://github.com/danefairbanks/TwitchChat/blob/master/TwitchChat/Code/IrcClient.cs
    // Command documentation: https://github.com/TwitchLib/TwitchLib.Client/tree/1dc45145654ac467a7568c88770c0df258a2bedb/TwitchLib.Client/Extensions

    /// <summary>
    /// IRC client for interacting with Chat service.
    /// </summary>
    public class ChatClient : ClientWebSocketBase
    {
        /// <summary>
        /// IRC message state.
        /// </summary>
        private enum MessageState
        {
            Start,
            TagKey,
            TagValue,
            Prefix,
            Command,
            StartParameter,
            Parameter,
            Trailing,
            EndLine
        }

        /// <summary>
        /// The default Chat connection url.
        /// </summary>
        public const string CHAT_CONNECTION_URL = "wss://irc-ws.chat.twitch.tv:443";

        private const string PING_COMMAND_ID = "PING";
        private const string RECONNECT_COMMAND_ID = "RECONNECT";

        /// <summary>
        /// Invoked when a Chat packet is received.
        /// </summary>
        public event EventHandler<ChatRawPacketModel> OnPacketReceived;

        /// <summary>
        /// Invoked when a ping is received.
        /// </summary>
        public event EventHandler OnPingReceived;

        /// <summary>
        /// Invoked when a reconnect is requested from the server.
        /// </summary>
        public event EventHandler OnReconnectRequestedReceived;

        /// <summary>
        /// Invoked when a room state is received.
        /// </summary>
        public event EventHandler<ChatRoomStatePacketModel> OnRoomStateReceived;

        /// <summary>
        /// Invoked when a user list is received.
        /// </summary>
        public event EventHandler<ChatUsersListPacketModel> OnUserListReceived;

        /// <summary>
        /// Invoked when a user join is received.
        /// </summary>
        public event EventHandler<ChatUserJoinPacketModel> OnUserJoinReceived;

        /// <summary>
        /// Invoked when a user leave is received.
        /// </summary>
        public event EventHandler<ChatUserLeavePacketModel> OnUserLeaveReceived;

        /// <summary>
        /// Invoked when a user state is received;
        /// </summary>
        public event EventHandler<ChatUserStatePacketModel> OnUserStateReceived;

        /// <summary>
        /// Invoked when a message is received.
        /// </summary>
        public event EventHandler<ChatMessagePacketModel> OnMessageReceived;

        /// <summary>
        /// Invoked when a whisper is received.
        /// </summary>
        public event EventHandler<ChatWhisperMessagePacketModel> OnWhisperMessageReceived;

        /// <summary>
        /// Invoked when a chat clear is received.
        /// </summary>
        public event EventHandler<ChatClearChatPacketModel> OnChatClearReceived;

        /// <summary>
        /// Invoked when a message clear is received.
        /// </summary>
        public event EventHandler<ChatClearMessagePacketModel> OnClearMessageReceived;

        /// <summary>
        /// Invoked when a host target is received.
        /// </summary>
        public event EventHandler<ChatHostTargetPacketModel> OnHostTargetReceived;

        /// <summary>
        /// Invoked when a notice is received.
        /// </summary>
        public event EventHandler<ChatNoticePacketModel> OnNoticeReceived;

        /// <summary>
        /// Invoked when a user notice is received.
        /// </summary>
        public event EventHandler<ChatUserNoticePacketModel> OnUserNoticeReceived;

        /// <summary>
        /// Invoked when a global user state is received.
        /// </summary>
        public event EventHandler<ChatGlobalUserStatePacketModel> OnGlobalUserStateReceived;

        private TwitchConnection connection;

        /// <summary>
        /// Creates a new instance of the ChatClient class.
        /// </summary>
        /// <param name="connection">The current connection</param>
        public ChatClient(TwitchConnection connection)
        {
            this.connection = connection;
        }

        /// <summary>
        /// Connects to the default ChatClient connection.
        /// </summary>
        /// <returns>An awaitable Task</returns>
        public async Task Connect()
        {
            await base.Connect(ChatClient.CHAT_CONNECTION_URL);

            OAuthTokenModel oauthToken = await this.connection.GetOAuthToken();
            await this.Send("PASS oauth:" + oauthToken.accessToken);

            NewAPI.Users.UserModel user = await this.connection.NewAPI.Users.GetCurrentUser();
            await this.Send("NICK " + user.login.ToLower());
        }

        /// <summary>
        /// Sends a pong packet.
        /// </summary>
        /// <returns>An awaitable Task</returns>
        public async Task Pong()
        {
            await this.Send("PONG :tmi.twitch.tv");
        }

        /// <summary>
        /// Joins the specified broadcaster's channel.
        /// </summary>
        /// <param name="broadcaster">The broadcaster's channel to join</param>
        /// <returns>An awaitable Task</returns>
        public async Task Join(NewAPI.Users.UserModel broadcaster)
        {
            Validator.ValidateVariable(broadcaster, "broadcaster");
            await this.Send("JOIN #" + broadcaster.login);
        }

        /// <summary>
        /// Joins the specified broadcaster's channel.
        /// </summary>
        /// <param name="broadcaster">The broadcaster's channel to join</param>
        /// <returns>An awaitable Task</returns>
        public async Task Join(V5.Users.UserModel broadcaster)
        {
            Validator.ValidateVariable(broadcaster, "broadcaster");
            await this.Send("JOIN #" + broadcaster.name);
        }

        /// <summary>
        /// Joins the specified broadcaster's channel in the specified room.
        /// </summary>
        /// <param name="broadcaster">The broadcaster's channel to join</param>
        /// <param name="room">The room to join</param>
        /// <returns>An awaitable Task</returns>
        public async Task Join(V5.Users.UserModel broadcaster, V5.Chat.ChatRoomModel room)
        {
            Validator.ValidateVariable(broadcaster, "broadcaster");
            Validator.ValidateVariable(room, "room");
            await this.Send(string.Format("JOIN #chatrooms:{0}:{1}" + broadcaster.id, room.id));
        }

        /// <summary>
        /// Leaves the specified broadcaster's channel.
        /// </summary>
        /// <param name="broadcaster">The broadcaster's channel to leave</param>
        /// <returns>An awaitable Task</returns>
        public async Task Leave(NewAPI.Users.UserModel broadcaster)
        {
            Validator.ValidateVariable(broadcaster, "broadcaster");
            await this.Send("PART #" + broadcaster.login);
        }

        /// <summary>
        /// Leaves the specified broadcaster's channel.
        /// </summary>
        /// <param name="broadcaster">The broadcaster's channel to leave</param>
        /// <returns>An awaitable Task</returns>
        public async Task Leave(V5.Users.UserModel broadcaster)
        {
            Validator.ValidateVariable(broadcaster, "broadcaster");
            await this.Send("PART #" + broadcaster.name);
        }

        /// <summary>
        /// Sends a message to the specified broadcaster's channel.
        /// </summary>
        /// <param name="broadcaster">The broadcaster's channel to send the message to</param>
        /// <param name="message">The message to send</param>
        /// <returns>An awaitable Task</returns>
        public async Task SendMessage(NewAPI.Users.UserModel broadcaster, string message)
        {
            Validator.ValidateVariable(broadcaster, "broadcaster");
            Validator.ValidateString(message, "message");
            await this.Send(string.Format("PRIVMSG #{0} :{1}", broadcaster.login, message));
        }

        /// <summary>
        /// Sends a message to the specified broadcaster's channel.
        /// </summary>
        /// <param name="broadcaster">The broadcaster's channel to send the message to</param>
        /// <param name="message">The message to send</param>
        /// <returns>An awaitable Task</returns>
        public async Task SendMessage(V5.Users.UserModel broadcaster, string message)
        {
            Validator.ValidateVariable(broadcaster, "broadcaster");
            Validator.ValidateString(message, "message");
            await this.Send(string.Format("PRIVMSG #{0} :{1}", broadcaster.name, message));
        }

        /// <summary>
        /// Sends a message to the specified broadcaster's channel.
        /// </summary>
        /// <param name="broadcaster">The broadcaster's channel to send the message to</param>
        /// <param name="user">The user to whisper</param>
        /// <param name="message">The message to send</param>
        /// <returns>An awaitable Task</returns>
        public async Task SendWhisperMessage(NewAPI.Users.UserModel broadcaster, NewAPI.Users.UserModel user, string message)
        {
            Validator.ValidateVariable(broadcaster, "broadcaster");
            Validator.ValidateVariable(user, "user");
            Validator.ValidateString(message, "message");
            await this.Send(string.Format("PRIVMSG #{0} :.w {1} {2}", broadcaster.login, user.login, message));
        }

        /// <summary>
        /// Sends a message to the specified broadcaster's channel.
        /// </summary>
        /// <param name="broadcaster">The broadcaster's channel to send the message to</param>
        /// <param name="user">The user to whisper</param>
        /// <param name="message">The message to send</param>
        /// <returns>An awaitable Task</returns>
        public async Task SendWhisperMessage(V5.Users.UserModel broadcaster, V5.Users.UserModel user, string message)
        {
            Validator.ValidateVariable(broadcaster, "broadcaster");
            Validator.ValidateVariable(user, "user");
            Validator.ValidateString(message, "message");
            await this.Send(string.Format("PRIVMSG #{0} :.w {1} {2}", broadcaster.name, user.name, message));
        }

        /// <summary>
        /// Deletes the specified message.
        /// </summary>
        /// <param name="broadcaster">The broadcaster's channel to use</param>
        /// <param name="messageID">The ID of the message to clear</param>
        /// <returns>An awaitable Task</returns>
        public async Task DeleteMessage(NewAPI.Users.UserModel broadcaster, string messageID)
        {
            Validator.ValidateVariable(broadcaster, "broadcaster");
            Validator.ValidateString(messageID, "messageID");
            await this.Send(string.Format("PRIVMSG #{0} :/delete {1}", broadcaster.login, messageID));
        }

        /// <summary>
        /// Deletes the specified message.
        /// </summary>
        /// <param name="broadcaster">The broadcaster's channel to use</param>
        /// <param name="messageID">The ID of the message to clear</param>
        /// <returns>An awaitable Task</returns>
        public async Task DeleteMessage(V5.Users.UserModel broadcaster, string messageID)
        {
            Validator.ValidateVariable(broadcaster, "broadcaster");
            Validator.ValidateString(messageID, "messageID");
            await this.Send(string.Format("PRIVMSG #{0} :/delete {1}", broadcaster.name, messageID));
        }

        /// <summary>
        /// Purges a user's messages.
        /// </summary>
        /// <param name="broadcaster">The broadcaster's channel to use</param>
        /// <param name="targetUser">The target user to purge</param>
        /// <param name="lengthInSeconds">The length in seconds to time out for</param>
        /// <returns>An awaitable Task</returns>
        public async Task TimeoutUser(NewAPI.Users.UserModel broadcaster, NewAPI.Users.UserModel targetUser, int lengthInSeconds)
        {
            Validator.ValidateVariable(broadcaster, "broadcaster");
            Validator.ValidateVariable(targetUser, "targetUser");
            await this.Send(string.Format("PRIVMSG #{0} :.timeout {1} {2}", broadcaster.login, targetUser.login, lengthInSeconds));
        }

        /// <summary>
        /// Purges a user's messages.
        /// </summary>
        /// <param name="broadcaster">The broadcaster's channel to use</param>
        /// <param name="targetUser">The target user to purge</param>
        /// <param name="lengthInSeconds">The length in seconds to time out for</param>
        /// <returns>An awaitable Task</returns>
        public async Task TimeoutUser(V5.Users.UserModel broadcaster, V5.Users.UserModel targetUser, int lengthInSeconds)
        {
            Validator.ValidateVariable(broadcaster, "broadcaster");
            Validator.ValidateVariable(targetUser, "targetUser");
            await this.Send(string.Format("PRIVMSG #{0} :.timeout {1} {2}", broadcaster.name, targetUser.name, lengthInSeconds));
        }

        /// <summary>
        /// Clears all messages from chat.
        /// </summary>
        /// <param name="broadcaster">The broadcaster's channel to use</param>
        /// <returns>An awaitable Task</returns>
        public async Task ClearChat(NewAPI.Users.UserModel broadcaster)
        {
            Validator.ValidateVariable(broadcaster, "broadcaster");
            await this.Send(string.Format("PRIVMSG #{0} :.clear", broadcaster.login));
        }

        /// <summary>
        /// Clears all messages from chat.
        /// </summary>
        /// <param name="broadcaster">The broadcaster's channel to use</param>
        /// <returns>An awaitable Task</returns>
        public async Task ClearChat(V5.Users.UserModel broadcaster)
        {
            Validator.ValidateVariable(broadcaster, "broadcaster");
            await this.Send(string.Format("PRIVMSG #{0} :.clear", broadcaster.name));
        }

        /// <summary>
        /// Mods the specified user in specified broadcaster's channel.
        /// </summary>
        /// <param name="broadcaster">The broadcaster's channel</param>
        /// <param name="user">The user to mod</param>
        /// <returns>An awaitable Task</returns>
        public async Task ModUser(NewAPI.Users.UserModel broadcaster, NewAPI.Users.UserModel user)
        {
            Validator.ValidateVariable(broadcaster, "broadcaster");
            Validator.ValidateVariable(user, "user");
            await this.Send(string.Format("PRIVMSG #{0} :.mod {1}", broadcaster.login, user.login));
        }

        /// <summary>
        /// Mods the specified user in specified broadcaster's channel.
        /// </summary>
        /// <param name="broadcaster">The broadcaster's channel</param>
        /// <param name="user">The user to mod</param>
        /// <returns>An awaitable Task</returns>
        public async Task ModUser(V5.Users.UserModel broadcaster, V5.Users.UserModel user)
        {
            Validator.ValidateVariable(broadcaster, "broadcaster");
            Validator.ValidateVariable(user, "user");
            await this.Send(string.Format("PRIVMSG #{0} :.mod {1}", broadcaster.name, user.name));
        }

        /// <summary>
        /// Unmods the specified user in specified broadcaster's channel.
        /// </summary>
        /// <param name="broadcaster">The broadcaster's channel</param>
        /// <param name="user">The user to unmod</param>
        /// <returns>An awaitable Task</returns>
        public async Task UnmodUser(NewAPI.Users.UserModel broadcaster, NewAPI.Users.UserModel user)
        {
            Validator.ValidateVariable(broadcaster, "broadcaster");
            Validator.ValidateVariable(user, "user");
            await this.Send(string.Format("PRIVMSG #{0} :.unmod {1}", broadcaster.login, user.login));
        }

        /// <summary>
        /// Unmods the specified user in specified broadcaster's channel.
        /// </summary>
        /// <param name="broadcaster">The broadcaster's channel</param>
        /// <param name="user">The user to unmod</param>
        /// <returns>An awaitable Task</returns>
        public async Task UnmodUser(V5.Users.UserModel broadcaster, V5.Users.UserModel user)
        {
            Validator.ValidateVariable(broadcaster, "broadcaster");
            Validator.ValidateVariable(user, "user");
            await this.Send(string.Format("PRIVMSG #{0} :.unmod {1}", broadcaster.name, user.name));
        }

        /// <summary>
        /// Bans the specified user in specified broadcaster's channel.
        /// </summary>
        /// <param name="broadcaster">The broadcaster's channel</param>
        /// <param name="user">The user to ban</param>
        /// <returns>An awaitable Task</returns>
        public async Task BanUser(NewAPI.Users.UserModel broadcaster, NewAPI.Users.UserModel user)
        {
            Validator.ValidateVariable(broadcaster, "broadcaster");
            Validator.ValidateVariable(user, "user");
            await this.Send(string.Format("PRIVMSG #{0} :.ban {1}", broadcaster.login, user.login));
        }

        /// <summary>
        /// Bans the specified user in specified broadcaster's channel.
        /// </summary>
        /// <param name="broadcaster">The broadcaster's channel</param>
        /// <param name="user">The user to ban</param>
        /// <returns>An awaitable Task</returns>
        public async Task BanUser(V5.Users.UserModel broadcaster, V5.Users.UserModel user)
        {
            Validator.ValidateVariable(broadcaster, "broadcaster");
            Validator.ValidateVariable(user, "user");
            await this.Send(string.Format("PRIVMSG #{0} :.ban {1}", broadcaster.name, user.name));
        }

        /// <summary>
        /// Bans the specified user in specified broadcaster's channel.
        /// </summary>
        /// <param name="broadcaster">The broadcaster's channel</param>
        /// <param name="user">The user to ban</param>
        /// <returns>An awaitable Task</returns>
        public async Task UnbanUser(NewAPI.Users.UserModel broadcaster, NewAPI.Users.UserModel user)
        {
            Validator.ValidateVariable(broadcaster, "broadcaster");
            Validator.ValidateVariable(user, "user");
            await this.Send(string.Format("PRIVMSG #{0} :.unban {1}", broadcaster.login, user.login));
        }

        /// <summary>
        /// Bans the specified user in specified broadcaster's channel.
        /// </summary>
        /// <param name="broadcaster">The broadcaster's channel</param>
        /// <param name="user">The user to ban</param>
        /// <returns>An awaitable Task</returns>
        public async Task UnbanUser(V5.Users.UserModel broadcaster, V5.Users.UserModel user)
        {
            Validator.ValidateVariable(broadcaster, "broadcaster");
            Validator.ValidateVariable(user, "user");
            await this.Send(string.Format("PRIVMSG #{0} :.unban {1}", broadcaster.name, user.name));
        }

        /// <summary>
        /// Runs a commercial for the specified time length in specified broadcaster's channel.
        /// </summary>
        /// <param name="broadcaster">The broadcaster's channel</param>
        /// <param name="lengthInSeconds">The length of the commercial</param>
        /// <returns>An awaitable Task</returns>
        public async Task RunCommercial(NewAPI.Users.UserModel broadcaster, int lengthInSeconds)
        {
            Validator.ValidateVariable(broadcaster, "broadcaster");
            await this.Send(string.Format("PRIVMSG #{0} :.commercial {1}", broadcaster.login, lengthInSeconds));
        }

        /// <summary>
        /// Runs a commercial for the specified time length in specified broadcaster's channel.
        /// </summary>
        /// <param name="broadcaster">The broadcaster's channel</param>
        /// <param name="lengthInSeconds">The length of the commercial</param>
        /// <returns>An awaitable Task</returns>
        public async Task RunCommercial(V5.Users.UserModel broadcaster, int lengthInSeconds)
        {
            Validator.ValidateVariable(broadcaster, "broadcaster");
            await this.Send(string.Format("PRIVMSG #{0} :.commercial {1}", broadcaster.name, lengthInSeconds));
        }

        /// <summary>
        /// Adds membership state event data. By default, we do not send this data to clients without this capability.
        /// </summary>
        /// <returns>An awaitable Task</returns>
        public async Task AddMembershipCapability()
        {
            await this.Send("CAP REQ :twitch.tv/membership");
        }

        /// <summary>
        /// Adds IRC V3 message tags to several commands, if enabled with the commands capability.
        /// </summary>
        /// <returns>An awaitable Task</returns>
        public async Task AddTagsCapability()
        {
            await this.Send("CAP REQ :twitch.tv/tags");
        }

        /// <summary>
        /// Enables several Twitch-specific commands.
        /// </summary>
        /// <returns>An awaitable Task</returns>
        public async Task AddCommandsCapability()
        {
            await this.Send("CAP REQ :twitch.tv/commands");
        }

        /// <summary>
        /// Processes the received text packet.
        /// </summary>
        /// <param name="packetMessage">The receive text packet</param>
        /// <returns>An awaitable task</returns>
        protected override Task ProcessReceivedPacket(string packetMessage)
        {
            foreach (string packetChunk in packetMessage.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                ChatRawPacketModel packet = this.ProcessPacketChunk(packetChunk);

                this.OnPacketReceived?.Invoke(this, packet);

                switch (packet.Command)
                {
                    case PING_COMMAND_ID:
                        this.OnPingReceived?.Invoke(this, new EventArgs());
                        break;
                    case RECONNECT_COMMAND_ID:
                        this.OnReconnectRequestedReceived?.Invoke(this, new EventArgs());
                        break;
                    case ChatRoomStatePacketModel.CommandID:
                        this.OnRoomStateReceived?.Invoke(this, new ChatRoomStatePacketModel(packet));
                        break;
                    case ChatUsersListPacketModel.CommandID:
                        this.OnUserListReceived?.Invoke(this, new ChatUsersListPacketModel(packet));
                        break;
                    case ChatUserJoinPacketModel.CommandID:
                        this.OnUserJoinReceived?.Invoke(this, new ChatUserJoinPacketModel(packet));
                        break;
                    case ChatUserLeavePacketModel.CommandID:
                        this.OnUserLeaveReceived?.Invoke(this, new ChatUserLeavePacketModel(packet));
                        break;
                    case ChatUserStatePacketModel.CommandID:
                        this.OnUserStateReceived?.Invoke(this, new ChatUserStatePacketModel(packet));
                        break;
                    case ChatMessagePacketModel.CommandID:
                        this.OnMessageReceived?.Invoke(this, new ChatMessagePacketModel(packet));
                        break;
                    case ChatWhisperMessagePacketModel.CommandID:
                        this.OnWhisperMessageReceived?.Invoke(this, new ChatWhisperMessagePacketModel(packet));
                        break;
                    case ChatClearChatPacketModel.CommandID:
                        this.OnChatClearReceived?.Invoke(this, new ChatClearChatPacketModel(packet));
                        break;
                    case ChatClearMessagePacketModel.CommandID:
                        this.OnClearMessageReceived?.Invoke(this, new ChatClearMessagePacketModel(packet));
                        break;
                    case ChatHostTargetPacketModel.CommandID:
                        this.OnHostTargetReceived?.Invoke(this, new ChatHostTargetPacketModel(packet));
                        break;
                    case ChatNoticePacketModel.CommandID:
                        this.OnNoticeReceived?.Invoke(this, new ChatNoticePacketModel(packet));
                        break;
                    case ChatUserNoticePacketModel.CommandID:
                        this.OnUserNoticeReceived?.Invoke(this, new ChatUserNoticePacketModel(packet));
                        break;
                    case ChatGlobalUserStatePacketModel.CommandID:
                        this.OnGlobalUserStateReceived?.Invoke(this, new ChatGlobalUserStatePacketModel(packet));
                        break;
                }
            }
            return Task.FromResult(0);
        }

        private async Task<IEnumerable<ChatRawPacketModel>> SendAndListen(Func<Task> sendFunction, IEnumerable<string> validPacketCommands, string endPacketCommand, int secondsToWait = 5)
        {
            List<ChatRawPacketModel> results = new List<ChatRawPacketModel>();
            bool stillListening = true;

            EventHandler<ChatRawPacketModel> listener = delegate (object sender, ChatRawPacketModel packet)
            {
                if (stillListening)
                {
                    if (validPacketCommands.Contains(packet.Command))
                    {
                        results.Add(packet);
                    }

                    if (packet.Command.Equals(endPacketCommand))
                    {
                        stillListening = false;
                    }
                }
            };

            this.OnPacketReceived += listener;

            await sendFunction();

            await this.WaitForSuccess(() => !stillListening, secondsToWait);

            this.OnPacketReceived -= listener;

            return results;
        }

        private ChatRawPacketModel ProcessPacketChunk(string packetMessage)
        {
            MessageState state = MessageState.Start;
            Dictionary<string, string> tags = new Dictionary<string, string>();
            StringBuilder key = new StringBuilder();
            StringBuilder value = new StringBuilder();
            StringBuilder prefix = new StringBuilder();
            StringBuilder command = new StringBuilder();
            StringBuilder parameter = new StringBuilder();
            List<string> parameters = new List<string>();

            for (int i = 0; i < packetMessage.Length; i++)
            {
                char c = packetMessage[i];
                switch (state)
                {
                    case MessageState.Start:
                        switch (c)
                        {
                            case '@':
                                state = MessageState.TagKey;
                                break;
                            case ':':
                                state = MessageState.Prefix;
                                break;
                            case ' ':
                                break;
                            case '\r':
                                state = MessageState.EndLine;
                                break;
                            default:
                                state = MessageState.Command;
                                if (char.IsLetterOrDigit(c))
                                {
                                    command.Append(c);
                                }
                                else
                                {
                                    throw new FormatException(string.Format("Unexpected character in command {0}.", (int)c));
                                }
                                break;
                        }
                        break;
                    case MessageState.TagKey:
                        switch (c)
                        {
                            case ' ':
                                state = MessageState.Start;
                                tags.Add(key.ToString(), null);
                                key.Clear();
                                break;
                            case ';':
                                state = MessageState.TagKey;
                                tags.Add(key.ToString(), null);
                                key.Clear();
                                break;
                            case '=':
                                state = MessageState.TagValue;
                                break;
                            case '\r':
                                state = MessageState.EndLine;
                                key.Clear();
                                break;
                            default:
                                if (char.IsLetterOrDigit(c) || c == '-' || c == '.' || c == '/')
                                {
                                    key.Append(c);
                                }
                                else
                                {
                                    throw new FormatException(string.Format("Unexpected character {0} found.", (int)c));
                                }
                                break;
                        }
                        break;
                    case MessageState.TagValue:
                        switch (c)
                        {
                            case ' ':
                                state = MessageState.Start;
                                tags.Add(key.ToString(), value.ToString());
                                key.Clear();
                                value.Clear();
                                break;
                            case ';':
                                state = MessageState.TagKey;
                                tags.Add(key.ToString(), value.ToString());
                                key.Clear();
                                value.Clear();
                                break;
                            case '\\':
                                if (i == (packetMessage.Length - 1))
                                {
                                    throw new EndOfStreamException("Unexpected end of stream during escape sequence.");
                                }

                                char x = packetMessage[i + 1];
                                switch (x)
                                {
                                    case ':':
                                        value.Append(';');
                                        break;
                                    case 's':
                                        value.Append(' ');
                                        break;
                                    case '\\':
                                        value.Append('\\');
                                        break;
                                    case 'r':
                                        value.Append('\r');
                                        break;
                                    case 'n':
                                        value.Append('\n');
                                        break;
                                    default:
                                        throw new FormatException(string.Format("Unexpected escape sequence {0}.", (int)c));
                                }
                                i++;
                                break;
                            case '\r':
                            case '\n':
                            case '\0':
                                throw new FormatException("Unexpected character in escaped value.");
                            default:
                                value.Append(c);
                                break;
                        }
                        break;
                    case MessageState.Prefix:
                        switch (c)
                        {
                            case ' ':
                                state = MessageState.Start;
                                break;
                            case '\r':
                            case '\n':
                            case '\0':
                                throw new FormatException("Unexpected character in prefix.");
                            default:
                                prefix.Append(c);
                                break;
                        }
                        break;
                    case MessageState.Command:
                        switch (c)
                        {
                            case ' ':
                                state = MessageState.StartParameter;
                                break;
                            default:
                                if (char.IsLetterOrDigit(c))
                                {
                                    command.Append(c);
                                }
                                else
                                {
                                    throw new FormatException(string.Format("Unexpected character in command {0}.", (int)c));
                                }
                                break;
                        }
                        break;
                    case MessageState.StartParameter:
                        switch (c)
                        {
                            case ' ':
                                break;
                            case ':':
                                state = MessageState.Trailing;
                                break;
                            case '\r':
                                state = MessageState.EndLine;
                                break;
                            case '\n':
                            case '\0':
                                throw new FormatException(string.Format("Unexpected character in parameter list {0}.", (int)c));
                            default:
                                state = MessageState.Parameter;
                                parameter.Append(c);
                                break;
                        }
                        break;
                    case MessageState.Parameter:
                        switch (c)
                        {
                            case ' ':
                                state = MessageState.StartParameter;
                                if (parameter.Length > 0)
                                {
                                    parameters.Add(parameter.ToString());
                                }
                                parameter.Clear();
                                break;
                            case '\r':
                                state = MessageState.EndLine;
                                break;
                            case '\n':
                            case '\0':
                                throw new FormatException("Unexpected character in parameter list.");
                            default:
                                parameter.Append(c);
                                break;
                        }
                        break;
                    case MessageState.Trailing:
                        switch (c)
                        {
                            case '\r':
                                state = MessageState.EndLine;
                                break;
                            case '\n':
                            case '\0':
                                throw new FormatException("Unexpected character in trailing parameter.");
                            default:
                                parameter.Append(c);
                                break;
                        }
                        break;
                    case MessageState.EndLine:
                        break;
                }
            }

            var lastParam = parameter.ToString();
            if (!string.IsNullOrWhiteSpace(lastParam))
            {
                parameters.Add(lastParam);
            }

            return new ChatRawPacketModel
            {
                RawText = packetMessage,
                Tags = tags,
                Prefix = prefix.ToString(),
                Command = command.ToString(),
                Parameters = parameters
            };
        }
    }
}
