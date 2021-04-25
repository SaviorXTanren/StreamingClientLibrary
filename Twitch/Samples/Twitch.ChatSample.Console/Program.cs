using StreamingClient.Base.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Twitch.Base;
using Twitch.Base.Clients;
using Twitch.Base.Models.Clients.Chat;
using Twitch.Base.Models.NewAPI.Users;

namespace Twitch.ChatSample.Console
{
    public class Program
    {
        public const string clientID = "xm067k6ffrsvt8jjngyc9qnaelt7oo";
        public const string clientSecret = "jtzezlc6iuc18vh9dktywdgdgtu44b";

        public static readonly List<OAuthClientScopeEnum> scopes = new List<OAuthClientScopeEnum>()
        {
            OAuthClientScopeEnum.channel_commercial,
            OAuthClientScopeEnum.channel_editor,
            OAuthClientScopeEnum.channel_read,
            OAuthClientScopeEnum.channel_subscriptions,

            OAuthClientScopeEnum.user_read,

            OAuthClientScopeEnum.bits__read,
            OAuthClientScopeEnum.channel__moderate,
            OAuthClientScopeEnum.chat__edit,
            OAuthClientScopeEnum.chat__read,
            OAuthClientScopeEnum.user__edit,
            OAuthClientScopeEnum.whispers__read,
            OAuthClientScopeEnum.whispers__edit,
        };

        private static TwitchConnection connection;
        private static UserModel user;
        private static ChatClient chat;

        private static SemaphoreSlim semaphore = new SemaphoreSlim(1);

        private static List<string> initialUserList = new List<string>();

        public static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                try
                {
                    Logger.SetLogLevel(LogLevel.Debug);
                    Logger.LogOccurred += Logger_LogOccurred;

                    using (StreamWriter writer = new StreamWriter(File.Open("Packets.txt", FileMode.Create)))
                    {
                        await writer.FlushAsync();
                    }

                    System.Console.WriteLine("Connecting to Twitch...");

                    connection = await TwitchConnection.ConnectViaLocalhostOAuthBrowser(clientID, clientSecret, scopes);
                    if (connection != null)
                    {
                        System.Console.WriteLine("Twitch connection successful!");

                        user = await connection.NewAPI.Users.GetCurrentUser();
                        if (user != null)
                        {
                            System.Console.WriteLine("Logged in as: " + user.display_name);

                            System.Console.WriteLine("Connecting to Chat...");

                            chat = new ChatClient(connection);

                            chat.OnDisconnectOccurred += Chat_OnDisconnectOccurred;
                            chat.OnSentOccurred += Chat_OnSentOccurred;
                            chat.OnPacketReceived += Chat_OnPacketReceived;

                            chat.OnPingReceived += Chat_OnPingReceived;
                            chat.OnGlobalUserStateReceived += Chat_OnGlobalUserStateReceived;
                            chat.OnUserListReceived += Chat_OnUserListReceived;
                            chat.OnUserJoinReceived += Chat_OnUserJoinReceived;
                            chat.OnUserLeaveReceived += Chat_OnUserLeaveReceived;
                            chat.OnMessageReceived += Chat_OnMessageReceived;
                            chat.OnUserStateReceived += Chat_OnUserStateReceived;
                            chat.OnUserNoticeReceived += Chat_OnUserNoticeReceived;

                            await chat.Connect();

                            await Task.Delay(1000);

                            await chat.AddCommandsCapability();
                            await chat.AddTagsCapability();
                            await chat.AddMembershipCapability();

                            await Task.Delay(1000);

                            await chat.Join(user);

                            await Task.Delay(2000);

                            System.Console.WriteLine(string.Format("There are {0} users currently in chat", initialUserList.Count()));

                            await chat.SendMessage(user, "Hello World!");

                            while (true)
                            {
                                string line = System.Console.ReadLine();
                                await chat.SendMessage(user, line);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.ToString());
                }
            }).Wait();

            System.Console.ReadLine();
        }

        private static void Chat_OnGlobalUserStateReceived(object sender, ChatGlobalUserStatePacketModel packet)
        {
            System.Console.WriteLine(string.Format("Connected as: {0} {1}", packet.UserID, packet.UserDisplayName));
        }

        private static void Chat_OnUserListReceived(object sender, ChatUsersListPacketModel packet)
        {
            initialUserList.AddRange(packet.UserLogins);
        }

        private static void Chat_OnUserJoinReceived(object sender, ChatUserJoinPacketModel packet)
        {
            System.Console.WriteLine(string.Format("User Joined: {0}", packet.UserLogin));
        }

        private static void Chat_OnUserLeaveReceived(object sender, ChatUserLeavePacketModel packet)
        {
            System.Console.WriteLine(string.Format("User Left: {0}", packet.UserLogin));
        }

        private static void Chat_OnMessageReceived(object sender, ChatMessagePacketModel packet)
        {
            System.Console.WriteLine(string.Format("{0}: {1}", packet.UserDisplayName, packet.Message));
        }

        private static void Chat_OnUserStateReceived(object sender, ChatUserStatePacketModel packet)
        {
            System.Console.WriteLine(string.Format("{0}: {1} {2}", packet.UserDisplayName, packet.UserBadges, packet.Color));
        }

        private static void Chat_OnUserNoticeReceived(object sender, ChatUserNoticePacketModel packet)
        {
            System.Console.WriteLine(string.Format("USER NOTICE: {0} {1}", packet.UserDisplayName, packet.SystemMessage));
        }

        private static async void Chat_OnPingReceived(object sender, EventArgs e)
        {
            await chat.Pong();
        }

        private static void Chat_OnDisconnectOccurred(object sender, System.Net.WebSockets.WebSocketCloseStatus e)
        {
            System.Console.WriteLine("DISCONNECTED");
        }

        private static void Chat_OnSentOccurred(object sender, string packet)
        {
            System.Console.WriteLine("SEND: " + packet);
        }

        private static async void Chat_OnPacketReceived(object sender, Base.Models.Clients.Chat.ChatRawPacketModel packet)
        {
            if (!packet.Command.Equals("PING") && !packet.Command.Equals(ChatMessagePacketModel.CommandID) && !packet.Command.Equals(ChatUserJoinPacketModel.CommandID)
                 && !packet.Command.Equals(ChatUserLeavePacketModel.CommandID))
            {
                System.Console.WriteLine("PACKET: " + packet.Command);

                await semaphore.WaitAndRelease(async () =>
                {
                    using (StreamWriter writer = new StreamWriter(File.Open("Packets.txt", FileMode.Append)))
                    {
                        await writer.WriteLineAsync(JSONSerializerHelper.SerializeToString(packet));
                        await writer.WriteLineAsync();
                        await writer.FlushAsync();
                    }
                });
            }
        }

        private static void Logger_LogOccurred(object sender, Log log)
        {
            System.Console.WriteLine(string.Format("LOG: {0} - {1}", log.Level, log.Message));
        }
    }
}
