using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mixer.Base;
using Mixer.Base.API;
using Mixer.Base.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Mixer.UnitTests
{
    [TestClass]
    public class AuthorizationUnitTests
    {
        [TestMethod]
        public void AuthorizeViaShortCode()
        {
            try
            {
                string clientID = "a95a8520f369fdd46d08aba183953c5c8a4c3822affec476";

                ShortCode shortCode = AuthorizationToken.GenerateShortCode("a95a8520f369fdd46d08aba183953c5c8a4c3822affec476", new List<ClientScopeEnum>() { ClientScopeEnum.chat__chat }).Result;

                Assert.IsNotNull(shortCode);
                Trace.WriteLine("Short Code: " + shortCode.code);

                string code = AuthorizationToken.ValidateShortCode(shortCode).Result;

                Assert.IsNotNull(code);
                Assert.AreNotEqual(code, string.Empty);

                AuthorizationToken token = AuthorizationToken.GetAuthorizationToken(clientID, code).Result;

                Assert.IsNotNull(token);

                token.RefreshToken().Wait();

                Channel channel = ChannelsService.GetChannel(token, "GlockGirl").Result;
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}
