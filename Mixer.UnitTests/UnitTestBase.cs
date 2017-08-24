using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mixer.Base;
using Mixer.Base.Model.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

namespace Mixer.UnitTests
{
    public abstract class UnitTestBase
    {
        public static readonly string clientID = ConfigurationManager.AppSettings["ClientID"];
        public static readonly List<OAuthClientScopeEnum> scopes = new List<OAuthClientScopeEnum>()
        {
            OAuthClientScopeEnum.channel__details__self,
            OAuthClientScopeEnum.channel__update__self,

            OAuthClientScopeEnum.chat__chat,
            OAuthClientScopeEnum.chat__clear_messages,
            OAuthClientScopeEnum.chat__connect,
            OAuthClientScopeEnum.chat__giveaway_start,
            OAuthClientScopeEnum.chat__poll_start,
            OAuthClientScopeEnum.chat__poll_vote,
            OAuthClientScopeEnum.chat__purge,
            OAuthClientScopeEnum.chat__remove_message,
            OAuthClientScopeEnum.chat__timeout,
            OAuthClientScopeEnum.chat__whisper,

            OAuthClientScopeEnum.interactive__manage__self,
            OAuthClientScopeEnum.interactive__robot__self,

            OAuthClientScopeEnum.user__details__self,
            OAuthClientScopeEnum.user__log__self,
            OAuthClientScopeEnum.user__notification__self,
            OAuthClientScopeEnum.user__update__self,
        };

        private static MixerConnection connection;

        public static MixerConnection GetMixerClient()
        {
            if (UnitTestBase.connection == null)
            {
                UnitTestBase.connection = MixerConnection.ConnectViaLocalhostOAuthBrowser(clientID, scopes).Result;
            }

            Assert.IsNotNull(UnitTestBase.connection);
            return UnitTestBase.connection;
        }

        protected List<MethodPacket> methodPackets = new List<MethodPacket>();
        protected List<ReplyPacket> replyPackets = new List<ReplyPacket>();
        protected List<EventPacket> eventPackets = new List<EventPacket>();

        protected void ClearPackets()
        {
            this.methodPackets.Clear();
            this.replyPackets.Clear();
            this.eventPackets.Clear();
        }

        protected static void TestWrapper(Func<MixerConnection, Task> function)
        {
            try
            {
                MixerConnection connection = UnitTestBase.GetMixerClient();
                function(connection).Wait();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
        }
    }
}
