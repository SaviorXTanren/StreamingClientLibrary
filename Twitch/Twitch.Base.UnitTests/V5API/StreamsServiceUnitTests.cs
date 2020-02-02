using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Twitch.Base.Models.V5.Channel;
using Twitch.Base.Models.V5.Streams;

namespace Twitch.Base.UnitTests.V5API
{
    [TestClass]
    public class StreamsServiceUnitTests : UnitTestBase
    {
        [TestMethod]
        public void GetCurrentChannelStream()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                PrivateChannelModel channel = await connection.V5API.Channels.GetCurrentChannel();

                Assert.IsNotNull(channel);

                StreamModel result = await connection.V5API.Streams.GetChannelStream(channel);

                Assert.IsNotNull(result);
                Assert.IsTrue(result.id > 0);
            });
        }

        [TestMethod]
        public void GetChannelStream()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                IEnumerable<StreamModel> results = await connection.V5API.Streams.GetStreams(game: "Fortnite");

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() > 0);

                StreamModel result = await connection.V5API.Streams.GetChannelStream(results.First().channel);

                Assert.IsNotNull(result);
                Assert.IsTrue(result.id > 0);
            });
        }

        [TestMethod]
        public void GetStreams()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                IEnumerable<StreamModel> results = await connection.V5API.Streams.GetStreams(game: "Fortnite");

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() > 0);
            });
        }

        [TestMethod]
        public void GetStreamsSummary()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                StreamsSummaryModel result = await connection.V5API.Streams.GetStreamsSummary("Fortnite");

                Assert.IsNotNull(result);
                Assert.IsTrue(result.channels > 0);
                Assert.IsTrue(result.viewers > 0);
            });
        }

        [TestMethod]
        public void GetFeaturedStreams()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                IEnumerable<FeaturedStreamModel> results = await connection.V5API.Streams.GetFeaturedStreams();

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() > 0);
            });
        }

        [TestMethod]
        public void GetFollowedStreams()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                IEnumerable<StreamModel> results = await connection.V5API.Streams.GetFollowedStreams();

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() > 0);
            });
        }
    }
}
