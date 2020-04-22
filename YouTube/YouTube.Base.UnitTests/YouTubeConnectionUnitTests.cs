using Microsoft.VisualStudio.TestTools.UnitTesting;
using StreamingClient.Base.Model.OAuth;
using System.Threading.Tasks;

namespace YouTube.Base.UnitTests
{
    [TestClass]
    public class YouTubeConnectionUnitTests : UnitTestBase
    {
        [TestMethod]
        public void AuthorizeViaOAuthRedirect()
        {
            TestWrapper((YouTubeConnection connection) =>
            {
                return Task.FromResult(0);
            });
        }

        [TestMethod]
        public void GetAuthorizationCodeURLForOAuth()
        {
            TestWrapper((YouTubeConnection connection) =>
            {
                string url = YouTubeConnection.GetAuthorizationCodeURLForOAuthBrowser(UnitTestBase.clientID, UnitTestBase.scopes, YouTubeConnection.DEFAULT_OAUTH_LOCALHOST_URL).Result;
                Assert.IsNotNull(url);
                return Task.FromResult(0);
            });
        }

        [TestMethod]
        public void RefreshToken()
        {
            TestWrapper(async (YouTubeConnection connection) =>
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
            TestWrapper(async (YouTubeConnection connection) =>
            {
                YouTubeConnection connection2 = await YouTubeConnection.ConnectViaOAuthToken(connection.GetOAuthTokenCopy());

                YouTubeConnection connection3 = await YouTubeConnection.ConnectViaOAuthToken(connection2.GetOAuthTokenCopy());
            });
        }
    }
}
