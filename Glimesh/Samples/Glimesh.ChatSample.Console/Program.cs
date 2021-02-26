using Glimesh.Base;
using Glimesh.Base.Clients;
using Glimesh.Base.Models.Channels;
using Glimesh.Base.Models.Clients.Channel;
using Glimesh.Base.Models.Clients.Chat;
using Glimesh.Base.Models.Users;
using StreamingClient.Base.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Glimesh.ChatSample.Console
{
    public class Program
    {
        public const string clientID = "86cbc95fe6e583fd72654af65c68a0a2cea8890cde4464de26c0f946d24fae1a";
        public const string clientSecret = "8562d68a91a1913e558b333115801b95f3db05699ee4c1f0300cef19f54fd839";

        private static List<OAuthClientScopeEnum> scopes = new List<OAuthClientScopeEnum>()
        {
            OAuthClientScopeEnum.publicinfo,
            OAuthClientScopeEnum.chat,
            OAuthClientScopeEnum.email,
            OAuthClientScopeEnum.streamkey
        };

        private static GlimeshConnection connection;
        private static UserModel user;
        private static ChannelModel channel;
        private static ChatEventClient client;

        public static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                try
                {
                    Logger.SetLogLevel(LogLevel.Debug);
                    Logger.LogOccurred += Logger_LogOccurred;

                    System.Console.WriteLine("Connecting to Glimesh...");

                    connection = await GlimeshConnection.ConnectViaLocalhostOAuthBrowser(clientID, clientSecret, scopes);
                    if (connection != null)
                    {
                        System.Console.WriteLine("Glimesh connection successful!");

                        user = await connection.Users.GetCurrentUser();
                        if (user != null)
                        {
                            System.Console.WriteLine("Current User: " + user.username);

                            channel = await connection.Channel.GetChannelByName(user.username);
                            if (channel != null)
                            {
                                System.Console.WriteLine("Channel ID: " + channel.id);

                                client = await ChatEventClient.CreateWithToken(connection);
                                client.OnChatMessageReceived += Client_OnChatMessageReceived;
                                client.OnChannelUpdated += Client_OnChannelUpdated;
                                client.OnFollowOccurred += Client_OnFollowOccurred;

                                System.Console.WriteLine("Connecting client...");
                                if (await client.Connect())
                                {
                                    System.Console.WriteLine("Successfully connected!");

                                    System.Console.WriteLine("Joining channel...");
                                    if (await client.JoinChannelChat(channel.id) && await client.JoinChannelEvents(channel.id, user.id))
                                    {
                                        System.Console.WriteLine("Successfully joined channel!");

                                        await client.SendMessage(channel.id, "Hello World!");

                                        while (true)
                                        {
                                            string line = System.Console.ReadLine();
                                            await client.SendMessage(channel.id, line);
                                        }
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

        private static void Client_OnChatMessageReceived(object sender, ChatMessagePacketModel message)
        {
            System.Console.WriteLine(string.Format("{0}: {1}", message.User?.displayname, message.Message));
        }

        private static void Client_OnChannelUpdated(object sender, ChannelUpdatePacketModel channelUpdate)
        {
            System.Console.WriteLine(string.Format("Stream Title: {0}", channelUpdate.Channel.title));
        }

        private static void Client_OnFollowOccurred(object sender, FollowPacketModel follow)
        {
            System.Console.WriteLine(string.Format("User Followed: {0}", follow.Follow.user?.username));
        }

        private static void Logger_LogOccurred(object sender, Log log)
        {
            System.Console.WriteLine(string.Format("LOG: {0} - {1}", log.Level, log.Message));
        }
    }
}
