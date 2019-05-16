using Microsoft.VisualStudio.TestTools.UnitTesting;
using StreamingClient.Base.Model.OAuth;
using System.Threading.Tasks;

namespace Twitch.Base.UnitTests
{
    [TestClass]
    public class TwitchConnectionUnitTests : UnitTestBase
    {
        [TestMethod]
        public void AuthorizeViaOAuthRedirect()
        {
            TestWrapper((TwitchConnection connection) =>
            {
                return Task.FromResult(0);
            });
        }

        [TestMethod]
        public void GetAuthorizationCodeURLForOAuth()
        {
            string url = TwitchConnection.GetAuthorizationCodeURLForOAuthBrowser(clientID, scopes, TwitchConnection.DEFAULT_OAUTH_LOCALHOST_URL).Result;
            Assert.IsNotNull(url);
        }

        [TestMethod]
        public void RefreshToken()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                OAuthTokenModel token = connection.GetOAuthTokenCopy();
                Assert.IsNotNull(token);

                await connection.RefreshOAuthToken();
                token = connection.GetOAuthTokenCopy();
                Assert.IsNotNull(token);
            });
        }

        [TestMethod]
        public void ConnectWithOldOAuthToken()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                TwitchConnection connection2 = await TwitchConnection.ConnectViaOAuthToken(connection.GetOAuthTokenCopy());

                TwitchConnection connection3 = await TwitchConnection.ConnectViaOAuthToken(connection2.GetOAuthTokenCopy());
            });
        }
    }
}
