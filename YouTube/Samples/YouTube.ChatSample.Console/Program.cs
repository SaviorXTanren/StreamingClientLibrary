using Google.Apis.YouTube.v3.Data;
using StreamingClient.Base.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using YouTube.Base;
using YouTube.Base.Clients;

namespace YouTube.ChatSample.Console
{
    public class Program
    {
        public static string clientID = "884596410562-pcrl1fn8ov0npj7fhjl086ffmud7r5j6.apps.googleusercontent.com";
        public static string clientSecret = "QBkxNmPNIvWatRvOIfRYrXlc";

        private static SemaphoreSlim fileLock = new SemaphoreSlim(1);

        public static readonly List<OAuthClientScopeEnum> scopes = new List<OAuthClientScopeEnum>()
        {
            OAuthClientScopeEnum.ChannelMemberships,
            OAuthClientScopeEnum.ManageAccount,
            OAuthClientScopeEnum.ManageData,
            OAuthClientScopeEnum.ManagePartner,
            OAuthClientScopeEnum.ManagePartnerAudit,
            OAuthClientScopeEnum.ManageVideos,
            OAuthClientScopeEnum.ReadOnlyAccount,
            OAuthClientScopeEnum.ViewAnalytics,
            OAuthClientScopeEnum.ViewMonetaryAnalytics
        };

        public static void Main(string[] args)
        {
            Logger.SetLogLevel(LogLevel.Debug);

            Logger.LogOccurred += Logger_LogOccurred;
            Task.Run(async () =>
            {
                try
                {
                    System.Console.WriteLine("Initializing connection");

                    YouTubeConnection connection = await YouTubeConnection.ConnectViaLocalhostOAuthBrowser(clientID, clientSecret, scopes);
                    if (connection != null)
                    {
                        Channel channel = await connection.Channels.GetMyChannel();

                        //Channel channel = await connection.Channels.GetChannelByID("");

                        if (channel != null)
                        {
                            System.Console.WriteLine("Connection successful. Logged in as: " + channel.Snippet.Title);

                            var broadcast = await connection.LiveBroadcasts.GetMyActiveBroadcast();

                            System.Console.WriteLine("Connecting chat client!");

                            ChatClient client = new ChatClient(connection);
                            client.OnMessagesReceived += Client_OnMessagesReceived;

                            if (await client.Connect(broadcast))
                            {
                                System.Console.WriteLine("Live chat connection successful!");

                                if (await connection.LiveBroadcasts.GetMyActiveBroadcast() != null)
                                {
                                    await client.SendMessage("Hello World!");
                                }

                                while (true) { }
                            }
                            else
                            {
                                System.Console.WriteLine("Failed to connect to live chat");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.ToString());
                }
            });

            System.Console.ReadLine();
        }

        private static void Client_OnMessagesReceived(object sender, IEnumerable<LiveChatMessage> messages)
        {
            foreach (LiveChatMessage message in messages)
            {
                try
                {
                    if (message.Snippet.HasDisplayContent.GetValueOrDefault())
                    {
                        System.Console.WriteLine(string.Format("{0}: {1}", message.AuthorDetails.DisplayName, message.Snippet.DisplayMessage));
                    }

                    //Logger.Log(JSONSerializerHelper.SerializeToString(message));
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
            }
        }

        private static async void Logger_LogOccurred(object sender, Log log)
        {
            if (log.Level >= LogLevel.Error)
            {
                System.Console.WriteLine(log.Message);
            }

            await fileLock.WaitAsync();

            try
            {
                using (StreamWriter writer = new StreamWriter(File.Open("Log.txt", FileMode.Append)))
                {
                    await writer.WriteAsync(string.Format("{0} - {1} - {2} " + Environment.NewLine + Environment.NewLine, DateTimeOffset.Now.ToString(), EnumHelper.GetEnumName(log.Level), log.Message));
                    await writer.FlushAsync();
                }
            }
            catch (Exception) { }

            fileLock.Release();
        }
    }
}
