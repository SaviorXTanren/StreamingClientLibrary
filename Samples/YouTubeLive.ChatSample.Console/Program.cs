using Google.Apis.YouTube.v3.Data;
using StreamingClient.Base.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YouTubeLive.Base;
using YouTubeLive.Base.Clients;

namespace YouTubeLive.ChatSample.Console
{
    public class Program
    {
        public static string clientID = "";     // SET YOUR OAUTH CLIENT ID
        public static string clientSecret = ""; // SET YOUR OAUTH CLIENT SECRET

        public static readonly List<OAuthClientScopeEnum> scopes = new List<OAuthClientScopeEnum>()
        {
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
            Logger.LogOccurred += Logger_LogOccurred;
            Task.Run(async () =>
            {
                try
                {
                    if (string.IsNullOrEmpty(clientID) || string.IsNullOrEmpty(clientSecret))
                    {
                        throw new InvalidOperationException("Client ID and/or Client Secret are not set in the UnitTestBase class");
                    }

                    System.Console.WriteLine("Initializing connection");

                    YouTubeLiveConnection connection = await YouTubeLiveConnection.ConnectViaLocalhostOAuthBrowser(clientID, clientSecret, scopes);
                    if (connection != null)
                    {
                        Channel channel = await connection.Channels.GetMyChannel();
                        if (channel != null)
                        {
                            System.Console.WriteLine("Connection successful. Logged in as: " + channel.Snippet.Title);

                            System.Console.WriteLine("Connecting chat client!");

                            ChatClient client = new ChatClient(connection);
                            client.OnMessagesReceived += Client_OnMessagesReceived;
                            if (await client.Connect())
                            {
                                System.Console.WriteLine("Live chat connection successful!");

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
                System.Console.WriteLine(string.Format("{0}: {1}", message.AuthorDetails.DisplayName, message.Snippet.TextMessageDetails.MessageText));
            }
        }

        private static void Logger_LogOccurred(object sender, Log log)
        {
            if (log.Level >= LogLevel.Error)
            {
                System.Console.WriteLine(log.Message);
            }
        }
    }
}
