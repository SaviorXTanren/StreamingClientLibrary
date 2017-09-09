using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mixer.Base;
using Mixer.Base.Model.OAuth;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Mixer.UnitTests
{
    [TestClass]
    public class MixerConnectionUnitTests : UnitTestBase
    {
        [TestMethod]
        public void AuthorizeViaShortCode()
        {
            try
            {
                MixerConnection connection = MixerConnection.ConnectViaShortCode(clientID, new List<OAuthClientScopeEnum>() { OAuthClientScopeEnum.chat__connect },
                (OAuthShortCodeModel code) =>
                {
                    Assert.IsNotNull(code);
                    Process.Start("https://mixer.com/oauth/shortcode?code=" + code.code);
                }).Result;

                Assert.IsNotNull(connection);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
        }

        [TestMethod]
        public void AuthorizeViaOAuthRedirect()
        {
            TestWrapper((MixerConnection connection) =>
            {
                return Task.FromResult(0);
            });
        }

        [TestMethod]
        public void GetAuthorizationCodeURLForOAuth()
        {
            string url = MixerConnection.GetAuthorizationCodeURLForOAuthBrowser(clientID, scopes, "http://localhost").Result;

            Assert.IsNotNull(url);
        }

        [TestMethod]
        public void RefreshToken()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                OAuthTokenModel token = await connection.GetOAuthToken();

                Assert.IsNotNull(token);

                await connection.RefreshOAuthToken();
                token = await connection.GetOAuthToken();

                Assert.IsNotNull(token);
            });
        }

        [TestMethod]
        public void ConnectWithOldOAuthToken()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                MixerConnection connection2 = await MixerConnection.ConnectViaOAuthToken(connection.GetOAuthTokenCopy());
            });
        }
    }
}
