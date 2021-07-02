using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Trovo.Base.Models.Channels;

namespace Trovo.Base.UnitTests
{
    [TestClass]
    public class ChannelsServiceUnitTests : UnitTestBase
    {
        [TestMethod]
        public void GetCurrentChannel()
        {
            TestWrapper(async (TrovoConnection connection) =>
            {
                ChannelModel channel = await connection.Channels.GetCurrentChannel();

                Assert.IsNotNull(channel);
                Assert.IsTrue(!string.IsNullOrEmpty(channel.channel_id));
                Assert.IsTrue(!string.IsNullOrEmpty(channel.live_title));
            });
        }

        [TestMethod]
        public void GetChannel()
        {
            TestWrapper(async (TrovoConnection connection) =>
            {
                ChannelModel channel = await connection.Channels.GetCurrentChannel();

                Assert.IsNotNull(channel);
                Assert.IsTrue(!string.IsNullOrEmpty(channel.channel_id));
                Assert.IsTrue(!string.IsNullOrEmpty(channel.live_title));

                channel = await connection.Channels.GetChannel(channel.channel_id);

                Assert.IsNotNull(channel);
                Assert.IsTrue(!string.IsNullOrEmpty(channel.channel_id));
                Assert.IsTrue(!string.IsNullOrEmpty(channel.live_title));
            });
        }

        [TestMethod]
        public void GetTopChannels()
        {
            TestWrapper(async (TrovoConnection connection) =>
            {
                IEnumerable<TopChannelModel> channels = await connection.Channels.GetTopChannels(maxResults: 10);

                Assert.IsNotNull(channels);
                Assert.IsTrue(channels.Count() > 0);
                Assert.IsNotNull(channels.First());

                Assert.IsTrue(!string.IsNullOrEmpty(channels.First().channel_id));
                Assert.IsTrue(!string.IsNullOrEmpty(channels.First().title));
            });
        }
    }
}
