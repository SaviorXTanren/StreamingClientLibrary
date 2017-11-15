using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mixer.Base;
using Mixer.Base.Clients;
using Mixer.Base.Model.Channel;
using Mixer.Base.Model.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mixer.UnitTests
{
    [TestClass]
    public class ConstellationClientUnitTests : UnitTestBase
    {
        private static ConstellationClient constellationClient;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                constellationClient = await ConstellationClient.Create(connection);

                Assert.IsTrue(await constellationClient.Connect());
            });
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                await constellationClient.Disconnect();
            });
        }

        [TestInitialize]
        public void TestInitialize()
        {
            this.ClearPackets();
        }

        [TestMethod]
        public void Ping()
        {
            this.ConstellationWrapper(async (MixerConnection connection, ConstellationClient constellationClient) =>
            {
                Assert.IsTrue(await constellationClient.Ping());
            });
        }

        [TestMethod]
        public void LiveSubscribeAndUnsubscribe()
        {
            this.ConstellationWrapper(async (MixerConnection connection, ConstellationClient constellationClient) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(connection);

                this.ClearPackets();

                ConstellationEventType eventType = new ConstellationEventType(ConstellationEventTypeEnum.channel__id__update, channel.id);
                Assert.IsTrue(await constellationClient.SubscribeToEventsWithResponse(new List<ConstellationEventType>() { eventType }));

                this.ClearPackets();

                bool eventReceived = false;
                constellationClient.OnSubscribedEventOccurred += (sender, le) =>
                {
                    if (le.channel.Equals(eventType.ToString()))
                    {
                        eventReceived = true;
                    }
                };

                string newName = "Test Name - " + DateTimeOffset.Now;
                channel.name = newName;

                channel = await connection.Channels.UpdateChannel(channel);

                Assert.IsNotNull(channel);
                Assert.IsTrue(string.Equals(channel.name, newName));

                this.ClearPackets();

                await Task.Delay(5000);

                if (!eventReceived)
                {
                    Assert.Fail("Did not get live event for channel updating");
                }

                Assert.IsTrue(await constellationClient.UnsubscribeToEventsWithResponse(new List<ConstellationEventType>() { eventType }));
            });
        }

        private void ConstellationWrapper(Func<MixerConnection, ConstellationClient, Task> function)
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                this.ClearPackets();

                constellationClient.OnReplyOccurred += ConstellationClient_OnReplyOccurred;
                constellationClient.OnEventOccurred += ConstellationClient_OnEventOccurred;

                await function(connection, constellationClient);

                constellationClient.OnReplyOccurred -= ConstellationClient_OnReplyOccurred;
                constellationClient.OnEventOccurred -= ConstellationClient_OnEventOccurred;
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
