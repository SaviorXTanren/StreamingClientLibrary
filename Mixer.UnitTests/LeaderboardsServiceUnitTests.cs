using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mixer.Base;
using Mixer.Base.Model.Channel;
using Mixer.Base.Model.Leaderboards;
using System.Collections.Generic;
using System.Linq;

namespace Mixer.UnitTests
{
    [TestClass]
    public class LeaderboardsServiceUnitTests : UnitTestBase
    {
        [TestMethod]
        public void GetWeeklyLeaderboard()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(connection);

                IEnumerable<SparksLeaderboardModel> leaderboard = await connection.Leaderboards.GetWeeklyLeaderboard(channel);
                Assert.IsNotNull(leaderboard);
                Assert.IsTrue(leaderboard.Count() >= 0);
            });
        }

        [TestMethod]
        public void GetMonthlyLeaderboard()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(connection);

                IEnumerable<SparksLeaderboardModel> leaderboard = await connection.Leaderboards.GetMonthlyLeaderboard(channel);
                Assert.IsNotNull(leaderboard);
                Assert.IsTrue(leaderboard.Count() >= 0);
            });
        }

        [TestMethod]
        public void GetYearlyLeaderboard()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(connection);

                IEnumerable<SparksLeaderboardModel> leaderboard = await connection.Leaderboards.GetYearlyLeaderboard(channel);
                Assert.IsNotNull(leaderboard);
                Assert.IsTrue(leaderboard.Count() >= 0);
            });
        }

        [TestMethod]
        public void GetAllTimeLeaderboard()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(connection);

                IEnumerable<SparksLeaderboardModel> leaderboard = await connection.Leaderboards.GetAllTimeLeaderboard(channel);
                Assert.IsNotNull(leaderboard);
                Assert.IsTrue(leaderboard.Count() >= 0);
            });
        }
    }
}
