using StreamingClient.Base.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trovo.Base;
using Trovo.Base.Clients;
using Trovo.Base.Models.Channels;
using Trovo.Base.Models.Chat;
using Trovo.Base.Models.Users;

namespace Trovo.ChatSample.Console
{
    public class Program
    {
        private static string clientID = "8FMjuk785AX4FMyrwPTU3B8vYvgHWN33";
        public static readonly List<OAuthClientScopeEnum> scopes = new List<OAuthClientScopeEnum>()
        {
            OAuthClientScopeEnum.chat_connect,
            OAuthClientScopeEnum.chat_send_self,
            OAuthClientScopeEnum.send_to_my_channel,
            OAuthClientScopeEnum.manage_messages,

            OAuthClientScopeEnum.channel_details_self,
            OAuthClientScopeEnum.channel_update_self,
            OAuthClientScopeEnum.channel_subscriptions,

            OAuthClientScopeEnum.user_details_self,
        };

        private static TrovoConnection connection;

        private static ChatClient chat;

        public static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                try
                {
                    Logger.SetLogLevel(LogLevel.Debug);
                    Logger.LogOccurred += Logger_LogOccurred;

                    System.Console.WriteLine("Connecting to Trovo...");

                    connection = await TrovoConnection.ConnectViaLocalhostOAuthBrowser(clientID, scopes);
                    if (connection != null)
                    {
                        System.Console.WriteLine("Trovo connection successful!");

                        PrivateUserModel user = await connection.Users.GetCurrentUser();
                        if (user != null)
                        {
                            System.Console.WriteLine("Current User: " + user.userName);

                            PrivateChannelModel channel = await connection.Channels.GetCurrentChannel();
                            if (channel != null)
                            {
                                System.Console.WriteLine("Channel Title: " + channel.live_title);

                                chat = new ChatClient(connection);
                                chat.OnChatMessageReceived += Chat_OnChatMessageReceived;

                                System.Console.WriteLine("Connecting to chat...");
                                if (await chat.Connect(await connection.Chat.GetToken()))
                                {
                                    System.Console.WriteLine("Successfully connected to chat!");

                                    await chat.SendMessage("Hello World!");

                                    ChatViewersRolesModel viewers = await connection.Chat.GetViewers(channel.channel_id, 1000);

                                    while (true)
                                    {
                                        string line = System.Console.ReadLine();
                                        await chat.SendMessage(line);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
            }).Wait();

            System.Console.ReadLine();
        }

        private static void Chat_OnChatMessageReceived(object sender, ChatMessageContainerModel message)
        {
            List<string> rawText = new List<string>();
            foreach (ChatMessageModel m in message.chats)
            {
                rawText.Add(m.content);
            }
            System.Console.WriteLine(string.Format("{0}: {1}", message.chats.FirstOrDefault()?.nick_name, string.Join(' ', rawText)));
        }

        private static void Logger_LogOccurred(object sender, Log log)
        {
            System.Console.WriteLine(string.Format("LOG: {0} - {1}", log.Level, log.Message));
        }
    }
}
