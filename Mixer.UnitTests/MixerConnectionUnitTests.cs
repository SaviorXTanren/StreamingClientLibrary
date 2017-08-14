using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mixer.Base;
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
                MixerConnection connection = MixerConnection.ConnectViaShortCode(clientID, new List<ClientScopeEnum>() { ClientScopeEnum.chat__connect },
                (ShortCode code) =>
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
                AuthorizationToken token = await connection.GetAuthorizationToken();
                await token.RefreshToken();
            });
        }
    }
}
