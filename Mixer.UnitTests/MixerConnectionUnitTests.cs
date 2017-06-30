using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mixer.Base;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Mixer.UnitTests
{
    [TestClass]
    public class MixerConnectionUnitTests : UnitTestBase
    {
        private static MixerConnection client;

        public static MixerConnection GetMixerClient()
        {
            if (MixerConnectionUnitTests.client == null)
            {
                string clientID = ConfigurationManager.AppSettings["ClientID"];
                if (string.IsNullOrEmpty(clientID))
                {
                    Assert.Fail("ClientID value isn't set in application configuration");
                }

                MixerConnectionUnitTests.client = MixerConnection.ConnectViaShortCode(clientID, new List<ClientScopeEnum>()
                {
                    ClientScopeEnum.channel__details__self,
                    ClientScopeEnum.channel__update__self,

                    //ClientScopeEnum.chat__bypass_links,
                    //ClientScopeEnum.chat__bypass_slowchat,
                    //ClientScopeEnum.chat__change_ban,
                    //ClientScopeEnum.chat__change_role,
                    ClientScopeEnum.chat__chat,
                    //ClientScopeEnum.chat__clear_messages,
                    ClientScopeEnum.chat__connect,
                    //ClientScopeEnum.chat__edit_options,
                    //ClientScopeEnum.chat__giveaway_start,
                    //ClientScopeEnum.chat__poll_start,
                    //ClientScopeEnum.chat__poll_vote,
                    //ClientScopeEnum.chat__purge,
                    //ClientScopeEnum.chat__remove_message,
                    //ClientScopeEnum.chat__timeout,
                    //ClientScopeEnum.chat__view_deleted,
                    ClientScopeEnum.chat__whisper,

                    ClientScopeEnum.interactive__manage__self,
                    ClientScopeEnum.interactive__robot__self,

                    ClientScopeEnum.user__details__self,
                    ClientScopeEnum.user__log__self,
                    ClientScopeEnum.user__notification__self,
                    ClientScopeEnum.user__update__self,
                },
                (string code) =>
                {
                    Assert.IsNotNull(code);
                    Process.Start("https://mixer.com/oauth/shortcode?code=" + code);
                }).Result;
            }

            Assert.IsNotNull(MixerConnectionUnitTests.client);
            return MixerConnectionUnitTests.client;
        }

        [TestMethod]
        public void AuthorizeViaShortCode()
        {
            this.TestWrapper((MixerConnection client) =>
            {
                return Task.FromResult(0);
            });
        }

        [TestMethod]
        public void RefreshToken()
        {
            this.TestWrapper(async (MixerConnection client) =>
            {
                AuthorizationToken token = await client.GetAuthorizationToken();
                await token.RefreshToken();
            });
        }
    }
}
