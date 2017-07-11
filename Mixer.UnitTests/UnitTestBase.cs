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
        public static readonly List<ClientScopeEnum> scopes = new List<ClientScopeEnum>()
        {
            ClientScopeEnum.channel__details__self,
            ClientScopeEnum.channel__update__self,

            ClientScopeEnum.chat__chat,
            ClientScopeEnum.chat__clear_messages,
            ClientScopeEnum.chat__connect,
            ClientScopeEnum.chat__giveaway_start,
            ClientScopeEnum.chat__poll_start,
            ClientScopeEnum.chat__poll_vote,
            ClientScopeEnum.chat__purge,
            ClientScopeEnum.chat__remove_message,
            ClientScopeEnum.chat__timeout,
            ClientScopeEnum.chat__whisper,

            ClientScopeEnum.interactive__manage__self,
            ClientScopeEnum.interactive__robot__self,

            ClientScopeEnum.user__details__self,
            ClientScopeEnum.user__log__self,
            ClientScopeEnum.user__notification__self,
            ClientScopeEnum.user__update__self,
        };

        private static MixerConnection connection;

        public MixerConnection GetMixerClient()
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

        protected void TestWrapper(Func<MixerConnection, Task> function)
        {
            try
            {
                MixerConnection connection = this.GetMixerClient();
                function(connection).Wait();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
        }
    }
}
