using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mixer.Base.Clients;
using Mixer.Base.Model.Channel;
using Mixer.Base.Model.Interactive;
using System.Collections.Generic;
using System.Linq;

namespace Mixer.UnitTests
{
    [TestClass]
    public class InteractiveServiceUnitTests : UnitTestBase
    {
        [TestMethod]
        public void GetInteractive()
        {
            this.TestWrapper(async (MixerClient client) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(client);

                InteractiveConnectionInfoModel interactive = await client.Interactive.GetInteractive(channel);

                Assert.IsNotNull(interactive);
                Assert.IsNotNull(interactive.address);
            });
        }

        [TestMethod]
        public void GetInteractiveRobot()
        {
            this.TestWrapper(async (MixerClient client) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(client);

                InteractiveRobotConnectionModel interactive = await client.Interactive.GetInteractiveRobot(channel);

                Assert.IsNotNull(interactive);
                Assert.IsNotNull(interactive.address);
            });
        }

        [TestMethod]
        public void GetInteractiveHosts()
        {
            this.TestWrapper(async (MixerClient client) =>
            {
                IEnumerable<string> addresses = await client.Interactive.GetInteractiveHosts();

                Assert.IsNotNull(addresses);
                Assert.IsTrue(addresses.Count() > 0);
            });
        }

        [TestMethod]
        public void GetOwnedInteractiveGames()
        {
            this.TestWrapper(async (MixerClient client) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(client);

                IEnumerable<InteractiveGameListingModel> games = await client.Interactive.GetOwnedInteractiveGames(channel);

                Assert.IsNotNull(games);
                Assert.IsTrue(games.Count() > 0);
            });
        }

        [TestMethod]
        public void GetSharedInteractiveGames()
        {
            this.TestWrapper(async (MixerClient client) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(client);

                IEnumerable<InteractiveGameListingModel> games = await client.Interactive.GetSharedInteractiveGames(channel);

                Assert.IsNotNull(games);
                Assert.IsTrue(games.Count() > 0);
            });
        }

        [TestMethod]
        public void GetInteractiveVersionInfo()
        {
            this.TestWrapper(async (MixerClient client) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(client);

                IEnumerable<InteractiveGameListingModel> games = await client.Interactive.GetOwnedInteractiveGames(channel);

                Assert.IsNotNull(games);
                Assert.IsTrue(games.Count() > 0);

                InteractiveVersionModel version = await client.Interactive.GetInteractiveVersionInfo(games.First().versions.First());

                Assert.IsNotNull(version);
            });
        }
    }
}
