using Microsoft.VisualStudio.TestTools.UnitTesting;
using StreamingClient.Base.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace YouTubeLive.Base.UnitTests
{
    public class UnitTestBase
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

        private static YouTubeLiveConnection connection;

        [AssemblyInitialize]
        public void Initialize()
        {
            Logger.LogOccurred += Logger_LogOccurred;
        }

        public static YouTubeLiveConnection GetYouTubeLiveClient()
        {
            if (UnitTestBase.connection == null)
            {
                UnitTestBase.connection = YouTubeLiveConnection.ConnectViaLocalhostOAuthBrowser(clientID, clientSecret, scopes).Result;
            }

            Assert.IsNotNull(UnitTestBase.connection);
            return UnitTestBase.connection;
        }

        protected static void TestWrapper(Func<YouTubeLiveConnection, Task> function)
        {
            if (string.IsNullOrEmpty(clientID) || string.IsNullOrEmpty(clientSecret))
            {
                throw new InvalidOperationException("Client ID and/or Client Secret are not set in the UnitTestBase class");
            }

            try
            {
                YouTubeLiveConnection connection = UnitTestBase.GetYouTubeLiveClient();
                function(connection).Wait();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
        }

        private void Logger_LogOccurred(object sender, Log log)
        {
            if (log.Level >= LogLevel.Error)
            {
                Assert.Fail(log.Message);
            }
        }
    }
}
