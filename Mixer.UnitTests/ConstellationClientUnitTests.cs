using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mixer.Base;
using Mixer.Base.Clients;
using Mixer.Base.Model.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mixer.UnitTests
{
    [TestClass]
    public class ConstellationClientUnitTests : UnitTestBase
    {
        [TestInitialize]
        public void TestInitialize()
        {
            this.ClearPackets();
        }

        [TestMethod]
        public void ConnectToInteractive()
        {
            this.ConstellationWrapper((MixerConnection connection, ConstellationClient constellationClient) =>
            {
                return Task.FromResult(0);
            });
        }

        [TestMethod]
        public void Ping()
        {
            this.ConstellationWrapper(async (MixerConnection connection, ConstellationClient constellationClient) =>
            {
                this.ClearPackets();

                Assert.IsTrue(await constellationClient.Ping());
            });
        }

        [TestMethod]
        public void LiveSubscribe()
        {
            this.ConstellationWrapper(async (MixerConnection connection, ConstellationClient constellationClient) =>
            {
                this.ClearPackets();

                List<ConstellationEventType> eventTypes = new List<ConstellationEventType>();
                eventTypes.Add(new ConstellationEventType(ConstellationEventTypeEnum.announcement__announce));

                Assert.IsTrue(await constellationClient.LiveSubscribe(eventTypes));
            });
        }

        [TestMethod]
        public void LiveUnsubscribe()
        {
            this.ConstellationWrapper(async (MixerConnection connection, ConstellationClient constellationClient) =>
            {
                this.ClearPackets();

                List<ConstellationEventType> eventTypes = new List<ConstellationEventType>();
                eventTypes.Add(new ConstellationEventType(ConstellationEventTypeEnum.announcement__announce));

                Assert.IsTrue(await constellationClient.LiveUnsubscribe(eventTypes));
            });
        }

        private void ConstellationWrapper(Func<MixerConnection, ConstellationClient, Task> function)
        {
            this.TestWrapper(async (MixerConnection connection) =>
            {
                this.ClearPackets();

                ConstellationClient constellationClient = await ConstellationClient.Create(connection);

                constellationClient.OnReplyOccurred += ConstellationClient_OnReplyOccurred;
                constellationClient.OnEventOccurred += ConstellationClient_OnEventOccurred;

                Assert.IsTrue(await constellationClient.Connect());

                await function(connection, constellationClient);

                await constellationClient.Disconnect();
            });
        }

        private void ConstellationClient_OnReplyOccurred(object sender, ReplyPacket e)
        {
            this.replyPackets.Add(e);
        }

        private void ConstellationClient_OnEventOccurred(object sender, EventPacket e)
        {
            this.eventPackets.Add(e);
        }
    }
}
