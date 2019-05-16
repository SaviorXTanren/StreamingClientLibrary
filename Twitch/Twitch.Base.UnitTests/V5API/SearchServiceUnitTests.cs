using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Twitch.Base.Models.V5.Channel;
using Twitch.Base.Models.V5.Games;
using Twitch.Base.Models.V5.Streams;

namespace Twitch.Base.UnitTests.V5API
{
    [TestClass]
    public class SearchServiceUnitTests : UnitTestBase
    {
        [TestMethod]
        public void SearchChannels()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                IEnumerable<ChannelModel> results = await connection.V5API.Search.SearchChannels("Fortnite", 10);

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() >= 10);
            });
        }

        [TestMethod]
        public void SearchGames()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                IEnumerable<GameModel> results = await connection.V5API.Search.SearchGames("Fortnite");

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() > 0);
            });
        }

        [TestMethod]
        public void SearchStreams()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                IEnumerable<StreamModel> results = await connection.V5API.Search.SearchStreams("Fortnite", 10);

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() >= 10);
            });
        }
    }
}
