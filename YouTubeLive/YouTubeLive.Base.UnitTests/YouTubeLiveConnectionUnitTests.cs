using Microsoft.VisualStudio.TestTools.UnitTesting;
using StreamingClient.Base.Model.OAuth;
using System.Threading.Tasks;

namespace YouTubeLive.Base.UnitTests
{
    [TestClass]
    public class YouTubeLiveConnectionUnitTests : UnitTestBase
    {
        [TestMethod]
        public void AuthorizeViaOAuthRedirect()
        {
            TestWrapper((YouTubeLiveConnection connection) =>
            {
                return Task.FromResult(0);
            });
        }

        [TestMethod]
        public void GetAuthorizationCodeURLForOAuth()
        {
            TestWrapper((YouTubeLiveConnection connection) =>
            {
                string url = YouTubeLiveConnection.GetAuthorizationCodeURLForOAuthBrowser(UnitTestBase.clientID, UnitTestBase.scopes, YouTubeLiveConnection.DEFAULT_OAUTH_LOCALHOST_URL).Result;
                Assert.IsNotNull(url);
                return Task.FromResult(0);
            });
        }

        [TestMethod]
        public void RefreshToken()
        {
            TestWrapper(async (YouTubeLiveConnection connection) =>
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
            TestWrapper(async (YouTubeLiveConnection connection) =>
            {
                YouTubeLiveConnection connection2 = await YouTubeLiveConnection.ConnectViaOAuthToken(connection.GetOAuthTokenCopy());

                YouTubeLiveConnection connection3 = await YouTubeLiveConnection.ConnectViaOAuthToken(connection2.GetOAuthTokenCopy());
            });
        }
    }
}
