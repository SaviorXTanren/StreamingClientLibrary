using Microsoft.VisualStudio.TestTools.UnitTesting;
using StreamingClient.Base.Model.OAuth;
using System.Threading.Tasks;

namespace Glimesh.Base.UnitTests
{
    [TestClass]
    public class GlimeshConnectionUnitTests : UnitTestBase
    {
        [TestMethod]
        public void AuthorizeViaOAuthRedirect()
        {
            TestWrapper((GlimeshConnection connection) =>
            {
                return Task.FromResult(0);
            });
        }

        [TestMethod]
        public void GetAuthorizationCodeURLForOAuth()
        {
            string url = GlimeshConnection.GetAuthorizationCodeURLForOAuthBrowser(clientID, scopes, GlimeshConnection.DEFAULT_OAUTH_LOCALHOST_URL).Result;
            Assert.IsNotNull(url);
        }

        [TestMethod]
        public void RefreshToken()
        {
            TestWrapper(async (GlimeshConnection connection) =>
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
            TestWrapper(async (GlimeshConnection connection) =>
            {
                GlimeshConnection connection2 = await GlimeshConnection.ConnectViaOAuthToken(connection.GetOAuthTokenCopy());

                GlimeshConnection connection3 = await GlimeshConnection.ConnectViaOAuthToken(connection2.GetOAuthTokenCopy());
            });
        }
    }
}
