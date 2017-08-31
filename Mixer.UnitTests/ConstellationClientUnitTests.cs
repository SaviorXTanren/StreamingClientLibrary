using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mixer.Base;
using Mixer.Base.Clients;
using Mixer.Base.Model.Channel;
using Mixer.Base.Model.Client;
using Mixer.Base.Model.Constellation;
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
                Assert.IsTrue(await constellationClient.SubscribeToEvents(new List<ConstellationEventType>() { eventType }));

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

                Assert.IsTrue(await constellationClient.UnsubscribeToEvents(new List<ConstellationEventType>() { eventType }));
            });
        }

        private void ConstellationWrapper(Func<MixerConnection, ConstellationClient, Task> function)
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                this.ClearPackets();

                ConstellationClient constellationClient = await ConstellationClient.Create(connection);

                constellationClient.OnReplyOccurred += ConstellationClient_OnReplyOccurred;
                constellationClient.OnEventOccurred += ConstellationClient_OnEventOccurred;

                Assert.IsTrue(await constellationClient.Connect());

                this.ClearPackets();

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
