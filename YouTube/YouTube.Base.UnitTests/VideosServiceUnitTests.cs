using Google.Apis.YouTube.v3.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace YouTube.Base.UnitTests
{
    [TestClass]
    public class VideosServiceUnitTests : UnitTestBase
    {
        [TestMethod]
        public void GetMyVideos()
        {
            TestWrapper(async (YouTubeConnection connection) =>
            {
                IEnumerable<Video> results = await connection.Videos.GetMyVideos(maxResults: 10);

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() > 0);
            });
        }

        [TestMethod]
        public void GetVideosForChannel()
        {
            TestWrapper(async (YouTubeConnection connection) =>
            {
                Channel channel = await connection.Channels.GetChannelByID("UC_x5XG1OV2P6uZZ5FSM9Ttw");

                Assert.IsNotNull(channel);
                Assert.IsNotNull(channel.Id);

                IEnumerable<Video> results = await connection.Videos.GetVideosForChannel(channel, maxResults: 100);

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() > 0);
            });
        }

        [TestMethod]
        public void GetVideoByID()
        {
            TestWrapper(async (YouTubeConnection connection) =>
            {
                Video result = await connection.Videos.GetVideoByID("Ks-_Mh1QhMc");

                Assert.IsNotNull(result);
                Assert.IsNotNull(result.Id);
            });
        }

        [TestMethod]
        public void GetVideosByKeyword()
        {
            TestWrapper(async (YouTubeConnection connection) =>
            {
                IEnumerable<SearchResult> results = await connection.Videos.GetVideosByKeyword("food", maxResults: 100);

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() == 100);
            });
        }
    }
}
