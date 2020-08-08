using StreamingClient.Base.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trovo.Base;
using Trovo.Base.Models.Channels;
using Trovo.Base.Models.Users;

namespace Trovo.ChatSample.Console
{
    public class Program
    {
        private static string clientID = "8FMjuk785AX4FMyrwPTU3B8vYvgHWN33";
        public static readonly List<OAuthClientScopeEnum> scopes = new List<OAuthClientScopeEnum>()
        {
            OAuthClientScopeEnum.channel_details_self,
            OAuthClientScopeEnum.channel_update_self,

            OAuthClientScopeEnum.user_details_self,
        };

        private static TrovoConnection connection;

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

        private static void Logger_LogOccurred(object sender, Log log)
        {
            System.Console.WriteLine(string.Format("LOG: {0} - {1}", log.Level, log.Message));
        }
    }
}
