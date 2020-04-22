using Google.Apis.YouTube.v3.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace YouTube.Base.UnitTests
{
    [TestClass]
    public class ChannelsServiceUnitTests : UnitTestBase
    {
        public static async Task<Channel> GetMyChannel(YouTubeConnection connection)
        {
            Channel channel = await connection.Channels.GetMyChannel();

            Assert.IsNotNull(channel);
            Assert.IsNotNull(channel.Id);

            return channel;
        }

        [TestMethod]
        public void GetMyChannel()
        {
            TestWrapper(async (YouTubeConnection connection) =>
            {
                Channel channel = await ChannelsServiceUnitTests.GetMyChannel(connection);
            });
        }

        [TestMethod]
        public void GetChannelByUsername()
        {
            TestWrapper(async (YouTubeConnection connection) =>
            {
                Channel channel = await connection.Channels.GetChannelByUsername("GoogleDevelopers");

                Assert.IsNotNull(channel);
                Assert.IsNotNull(channel.Id);
            });
        }

        [TestMethod]
        public void GetChannelByID()
        {
            TestWrapper(async (YouTubeConnection connection) =>
            {
                Channel channel = await connection.Channels.GetChannelByID("UCHRSNim0jPoFPJS_PX2v8sg");

                Assert.IsNotNull(channel);
                Assert.IsNotNull(channel.Id);
            });
        }
    }
}
