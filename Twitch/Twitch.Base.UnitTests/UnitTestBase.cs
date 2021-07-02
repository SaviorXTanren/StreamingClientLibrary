using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Twitch.Base.UnitTests
{
    public abstract class UnitTestBase
    {
        public const string clientID = "xm067k6ffrsvt8jjngyc9qnaelt7oo";
        public const string clientSecret = "jtzezlc6iuc18vh9dktywdgdgtu44b";

        public static readonly List<OAuthClientScopeEnum> scopes = new List<OAuthClientScopeEnum>()
        {
            OAuthClientScopeEnum.channel_check_subscription,
            OAuthClientScopeEnum.channel_commercial,
            OAuthClientScopeEnum.channel_editor,
            OAuthClientScopeEnum.channel_read,
            OAuthClientScopeEnum.channel_stream,
            OAuthClientScopeEnum.channel_subscriptions,

            OAuthClientScopeEnum.user_blocks_edit,
            OAuthClientScopeEnum.user_blocks_read,
            OAuthClientScopeEnum.user_read,
            OAuthClientScopeEnum.user_subscriptions,

            OAuthClientScopeEnum.bits__read,
            OAuthClientScopeEnum.channel__manage__broadcast,
            OAuthClientScopeEnum.channel__manage__redemptions,
            OAuthClientScopeEnum.channel__read__subscriptions,
            OAuthClientScopeEnum.channel__read__hype_train,
            OAuthClientScopeEnum.clips__edit,
            OAuthClientScopeEnum.user__edit,
            OAuthClientScopeEnum.user__edit__broadcast,
            OAuthClientScopeEnum.user__read__broadcast,
            OAuthClientScopeEnum.whispers__read,
        };

        private static TwitchConnection connection;

        public static TwitchConnection GetTwitchClient()
        {
            if (UnitTestBase.connection == null)
            {
                UnitTestBase.connection = TwitchConnection.ConnectViaLocalhostOAuthBrowser(clientID, clientSecret, scopes).Result;
            }

            Assert.IsNotNull(UnitTestBase.connection);
            return UnitTestBase.connection;
        }

        public static TwitchConnection GetAppTwitchClient()
        {
            if (UnitTestBase.connection == null)
            {
                UnitTestBase.connection = TwitchConnection.ConnectViaAppAccess(clientID, clientSecret).Result;
            }

            Assert.IsNotNull(UnitTestBase.connection);
            return UnitTestBase.connection;
        }

        protected static void TestWrapper(Func<TwitchConnection, Task> function)
        {
            try
            {
                TwitchConnection connection = UnitTestBase.GetTwitchClient();
                function(connection).Wait();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
        }

        protected static void TestAppWrapper(Func<TwitchConnection, Task> function)
        {
            try
            {
                TwitchConnection connection = UnitTestBase.GetAppTwitchClient();
                function(connection).Wait();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
        }
    }
}
