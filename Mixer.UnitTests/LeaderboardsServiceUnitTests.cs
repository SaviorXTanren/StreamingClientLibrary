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
        public void GetWeeklySparksLeaderboard()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(connection);

                IEnumerable<SparksLeaderboardModel> leaderboard = await connection.Leaderboards.GetWeeklySparksLeaderboard(channel);
                Assert.IsNotNull(leaderboard);
                Assert.IsTrue(leaderboard.Count() >= 0);
            });
        }

        [TestMethod]
        public void GetMonthlySparksLeaderboard()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(connection);

                IEnumerable<SparksLeaderboardModel> leaderboard = await connection.Leaderboards.GetMonthlySparksLeaderboard(channel);
                Assert.IsNotNull(leaderboard);
                Assert.IsTrue(leaderboard.Count() >= 0);
            });
        }

        [TestMethod]
        public void GetYearlySparksLeaderboard()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(connection);

                IEnumerable<SparksLeaderboardModel> leaderboard = await connection.Leaderboards.GetYearlySparksLeaderboard(channel);
                Assert.IsNotNull(leaderboard);
                Assert.IsTrue(leaderboard.Count() >= 0);
            });
        }

        [TestMethod]
        public void GetAllTimeSparksLeaderboard()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(connection);

                IEnumerable<SparksLeaderboardModel> leaderboard = await connection.Leaderboards.GetAllTimeSparksLeaderboard(channel);
                Assert.IsNotNull(leaderboard);
                Assert.IsTrue(leaderboard.Count() >= 0);
            });
        }

        [TestMethod]
        public void GetWeeklyEmbersLeaderboard()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                ChannelModel channel = await connection.Channels.GetChannel("VerbatimT");
                Assert.IsNotNull(channel);

                IEnumerable<EmbersLeaderboardModel> leaderboard = await connection.Leaderboards.GetWeeklyEmbersLeaderboard(channel);
                Assert.IsNotNull(leaderboard);
                Assert.IsTrue(leaderboard.Count() >= 0);
            });
        }

        [TestMethod]
        public void GetMonthlyEmbersLeaderboard()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                ChannelModel channel = await connection.Channels.GetChannel("VerbatimT");
                Assert.IsNotNull(channel);

                IEnumerable<EmbersLeaderboardModel> leaderboard = await connection.Leaderboards.GetMonthlyEmbersLeaderboard(channel);
                Assert.IsNotNull(leaderboard);
                Assert.IsTrue(leaderboard.Count() >= 0);
            });
        }

        [TestMethod]
        public void GetYearlyEmbersLeaderboard()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                ChannelModel channel = await connection.Channels.GetChannel("VerbatimT");
                Assert.IsNotNull(channel);

                IEnumerable<EmbersLeaderboardModel> leaderboard = await connection.Leaderboards.GetYearlyEmbersLeaderboard(channel);
                Assert.IsNotNull(leaderboard);
                Assert.IsTrue(leaderboard.Count() >= 0);
            });
        }

        [TestMethod]
        public void GetAllTimeEmbersLeaderboard()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                ChannelModel channel = await connection.Channels.GetChannel("VerbatimT");
                Assert.IsNotNull(channel);

                IEnumerable<EmbersLeaderboardModel> leaderboard = await connection.Leaderboards.GetAllTimeEmbersLeaderboard(channel);
                Assert.IsNotNull(leaderboard);
                Assert.IsTrue(leaderboard.Count() >= 0);
            });
        }
    }
}
