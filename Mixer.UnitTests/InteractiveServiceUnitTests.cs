using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mixer.Base;
using Mixer.Base.Model.Channel;
using Mixer.Base.Model.Interactive;
using System.Collections.Generic;
using System.Linq;

namespace Mixer.UnitTests
{
    [TestClass]
    public class InteractiveServiceUnitTests : UnitTestBase
    {
        /// <summary>
        /// Requires an interactive connection to be established for this unit test to pass
        /// </summary>
        [TestMethod]
        public void GetInteractive()
        {
            this.TestWrapper(async (MixerConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(connection);

                InteractiveConnectionInfoModel interactive = await connection.Interactive.GetInteractive(channel);

                Assert.IsNotNull(interactive);
                Assert.IsNotNull(interactive.address);
            });
        }

        /// <summary>
        /// Requires an interactive connection to be established for this unit test to pass
        /// </summary>
        [TestMethod]
        public void GetInteractiveRobot()
        {
            this.TestWrapper(async (MixerConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(connection);

                InteractiveRobotConnectionModel interactive = await connection.Interactive.GetInteractiveRobot(channel);

                Assert.IsNotNull(interactive);
                Assert.IsNotNull(interactive.address);
            });
        }

        [TestMethod]
        public void GetInteractiveHosts()
        {
            this.TestWrapper(async (MixerConnection connection) =>
            {
                IEnumerable<string> addresses = await connection.Interactive.GetInteractiveHosts();

                Assert.IsNotNull(addresses);
                Assert.IsTrue(addresses.Count() > 0);
            });
        }

        [TestMethod]
        public void GetOwnedInteractiveGames()
        {
            this.TestWrapper(async (MixerConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(connection);

                IEnumerable<InteractiveGameListingModel> games = await connection.Interactive.GetOwnedInteractiveGames(channel);

                Assert.IsNotNull(games);
                Assert.IsTrue(games.Count() > 0);
            });
        }

        [TestMethod]
        public void GetSharedInteractiveGames()
        {
            this.TestWrapper(async (MixerConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(connection);

                IEnumerable<InteractiveGameListingModel> games = await connection.Interactive.GetSharedInteractiveGames(channel);

                Assert.IsNotNull(games);
            });
        }

        [TestMethod]
        public void GetInteractiveVersionInfo()
        {
            this.TestWrapper(async (MixerConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(connection);

                IEnumerable<InteractiveGameListingModel> games = await connection.Interactive.GetOwnedInteractiveGames(channel);

                Assert.IsNotNull(games);
                Assert.IsTrue(games.Count() > 0);

                InteractiveVersionModel version = await connection.Interactive.GetInteractiveVersionInfo(games.First().versions.First());

                Assert.IsNotNull(version);
            });
        }
    }
}
