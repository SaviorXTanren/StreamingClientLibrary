using StreamingClient.Base.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Twitch.Base;
using Twitch.Base.Clients;
using Twitch.Base.Models.Clients.PubSub;
using Twitch.Base.Models.Clients.PubSub.Messages;
using Twitch.Base.Models.NewAPI.Users;

namespace Twitch.PubSubSample.Console
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
            OAuthClientScopeEnum.user_subscriptions,

            OAuthClientScopeEnum.user_read,

            OAuthClientScopeEnum.bits__read,
            OAuthClientScopeEnum.channel__moderate,
            OAuthClientScopeEnum.channel__read__redemptions,
            OAuthClientScopeEnum.chat__edit,
            OAuthClientScopeEnum.chat__read,
            OAuthClientScopeEnum.user__edit,
            OAuthClientScopeEnum.whispers__read,
            OAuthClientScopeEnum.whispers__edit,
        };

        private static TwitchConnection connection;
        private static UserModel user;
        private static PubSubClient pubSub;

        public static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                try
                {
                    Logger.SetLogLevel(LogLevel.Debug);
                    Logger.LogOccurred += Logger_LogOccurred;

                    System.Console.WriteLine("Connecting to Twitch...");

                    connection = TwitchConnection.ConnectViaLocalhostOAuthBrowser(clientID, clientSecret, scopes).Result;
                    if (connection != null)
                    {
                        System.Console.WriteLine("Twitch connection successful!");

                        user = await connection.NewAPI.Users.GetCurrentUser();
                        if (user != null)
                        {
                            System.Console.WriteLine("Logged in as: " + user.display_name);

                            System.Console.WriteLine("Connecting to PubSub...");

                            pubSub = new PubSubClient(connection);

                            pubSub.OnDisconnectOccurred += PubSub_OnDisconnectOccurred;
                            pubSub.OnSentOccurred += PubSub_OnSentOccurred;
                            pubSub.OnReconnectReceived += PubSub_OnReconnectReceived;
                            pubSub.OnResponseReceived += PubSub_OnResponseReceived;
                            pubSub.OnMessageReceived += PubSub_OnMessageReceived;
                            pubSub.OnWhisperReceived += PubSub_OnWhisperReceived;
                            pubSub.OnPongReceived += PubSub_OnPongReceived;

                            await pubSub.Connect();

                            await Task.Delay(1000);

                            List<PubSubListenTopicModel> topics = new List<PubSubListenTopicModel>();
                            foreach (PubSubTopicsEnum topic in EnumHelper.GetEnumList<PubSubTopicsEnum>())
                            {
                                topics.Add(new PubSubListenTopicModel(topic, user.id));
                            }

                            await pubSub.Listen(topics);

                            await Task.Delay(1000);

                            await pubSub.Ping();

                            while (true)
                            {
                                System.Console.ReadLine();
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

        private static void PubSub_OnDisconnectOccurred(object sender, System.Net.WebSockets.WebSocketCloseStatus e)
        {
            System.Console.WriteLine("DISCONNECTED");
        }

        private static void PubSub_OnSentOccurred(object sender, string packet)
        {
            System.Console.WriteLine("SEND: " + packet);
        }

        private static void PubSub_OnReconnectReceived(object sender, System.EventArgs e)
        {
            System.Console.WriteLine("RECONNECT");
        }

        private static void PubSub_OnResponseReceived(object sender, PubSubResponsePacketModel packet)
        {
            System.Console.WriteLine("RESPONSE: " + packet.error);
        }

        private static void PubSub_OnMessageReceived(object sender, PubSubMessagePacketModel packet)
        {
            System.Console.WriteLine(string.Format("MESSAGE: {0} {1} ", packet.type, packet.message));
        }

        private static void PubSub_OnWhisperReceived(object sender, PubSubWhisperEventModel whisper)
        {
            System.Console.WriteLine("WHISPER: " + whisper.body);
        }

        private static void PubSub_OnPongReceived(object sender, System.EventArgs e)
        {
            System.Console.WriteLine("PONG");
            Task.Run(async () =>
            {
                await Task.Delay(1000 * 60 * 3);
                await pubSub.Ping();
            });
        }

        private static void Logger_LogOccurred(object sender, Log log)
        {
            System.Console.WriteLine(string.Format("LOG: {0} - {1}", log.Level, log.Message));
        }
    }
}
