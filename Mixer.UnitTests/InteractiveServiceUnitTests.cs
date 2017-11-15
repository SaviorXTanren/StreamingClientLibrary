using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mixer.Base;
using Mixer.Base.Clients;
using Mixer.Base.Model.Channel;
using Mixer.Base.Model.Interactive;
using Mixer.Base.Model.User;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mixer.UnitTests
{
    [TestClass]
    public class InteractiveServiceUnitTests : UnitTestBase
    {
        private const string InteractiveGameName = "Mixer C# Unit Tests Game";

        private static InteractiveGameListingModel testGameListing;

        public static async Task<InteractiveGameListingModel> CreateTestGame(MixerConnection connection, ChannelModel channel)
        {
            UserModel user = await UsersServiceUnitTests.GetCurrentUser(connection);

            IEnumerable<InteractiveGameListingModel> gameListings = await connection.Interactive.GetOwnedInteractiveGames(channel);

            InteractiveGameListingModel previousTestGame = gameListings.FirstOrDefault(g => g.name.Equals(InteractiveServiceUnitTests.InteractiveGameName));
            if (previousTestGame != null)
            {
                await InteractiveServiceUnitTests.DeleteTestGame(connection, previousTestGame);
            }

            InteractiveGameModel game = new InteractiveGameModel()
            {
                name = InteractiveServiceUnitTests.InteractiveGameName,
                ownerId = user.id,             
            };
            game = await connection.Interactive.CreateInteractiveGame(game);

            Assert.IsNotNull(game);
            Assert.IsTrue(game.id > 0);

            game.controlVersion = "2.0";
            game = await connection.Interactive.UpdateInteractiveGame(game);

            Assert.IsNotNull(game);
            Assert.IsTrue(game.id > 0);

            gameListings = await connection.Interactive.GetOwnedInteractiveGames(channel);

            Assert.IsNotNull(gameListings);
            Assert.IsTrue(gameListings.Count() > 0);

            InteractiveGameListingModel gameListing = gameListings.FirstOrDefault(gl => gl.id.Equals(game.id));
            Assert.IsNotNull(gameListing);

            InteractiveGameVersionModel version = gameListing.versions.First();
            InteractiveSceneModel defaultScene = new InteractiveSceneModel()
            {
                sceneID = "default",
            };

            defaultScene.buttons.Add(InteractiveClientUnitTests.CreateTestButton());
            defaultScene.joysticks.Add(InteractiveClientUnitTests.CreateTestJoystick());

            version.controls.scenes.Add(defaultScene);
            version.controlVersion = "2.0";
            version = await connection.Interactive.UpdateInteractiveGameVersion(version);

            gameListings = await connection.Interactive.GetOwnedInteractiveGames(channel);

            Assert.IsNotNull(gameListings);
            Assert.IsTrue(gameListings.Count() > 0);

            gameListing = gameListings.FirstOrDefault(gl => gl.id.Equals(game.id));
            Assert.IsNotNull(gameListing);

            return gameListing;
        }

        public static async Task DeleteTestGame(MixerConnection connection, InteractiveGameModel game)
        {
            Assert.IsTrue(await connection.Interactive.DeleteInteractiveGame(game));
        }

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(connection);

                testGameListing = await InteractiveServiceUnitTests.CreateTestGame(connection, channel);
            });
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                await InteractiveServiceUnitTests.DeleteTestGame(connection, testGameListing);
            });
        }

        [TestMethod]
        public void GetInteractiveConnections()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(connection);

                InteractiveClient interactiveClient = await InteractiveClient.CreateFromChannel(connection, channel, testGameListing);

                Assert.IsTrue(await interactiveClient.Connect());

                InteractiveConnectionInfoModel interactiveConnection = await connection.Interactive.GetInteractiveConnectionInfo(channel);

                Assert.IsNotNull(interactiveConnection);
                Assert.IsNotNull(interactiveConnection.address);

                InteractiveRobotConnectionModel robotConnection = await connection.Interactive.GetInteractiveRobotConnectionInfo(channel);

                Assert.IsNotNull(robotConnection);
                Assert.IsNotNull(robotConnection.address);

                await interactiveClient.Disconnect();
            });
        }

        [TestMethod]
        public void GetInteractiveHosts()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                IEnumerable<string> addresses = await connection.Interactive.GetInteractiveHosts();

                Assert.IsNotNull(addresses);
                Assert.IsTrue(addresses.Count() > 0);
            });
        }

        [TestMethod]
        public void GetOwnedInteractiveGames()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(connection);

                IEnumerable<InteractiveGameListingModel> games = await connection.Interactive.GetOwnedInteractiveGames(channel);

                Assert.IsNotNull(games);
            });
        }

        [TestMethod]
        public void GetSharedInteractiveGames()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(connection);

                IEnumerable<InteractiveGameListingModel> games = await connection.Interactive.GetSharedInteractiveGames(channel);

                Assert.IsNotNull(games);
            });
        }

        [TestMethod]
        public void CreateGetUpdateDeleteGame()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(connection);

                InteractiveGameListingModel gameListing = await InteractiveServiceUnitTests.CreateTestGame(connection, channel);

                IEnumerable<InteractiveGameListingModel> games = await connection.Interactive.GetOwnedInteractiveGames(channel);

                Assert.IsNotNull(games);
                Assert.IsTrue(games.Count() > 0);
                Assert.IsTrue(games.Any(g => g.id.Equals(gameListing.id)));

                string gameName = gameListing.name = "Test Game";
                InteractiveGameModel game = await connection.Interactive.UpdateInteractiveGame(gameListing);

                Assert.IsNotNull(game);
                Assert.IsTrue(game.id > 0);
                Assert.AreEqual(game.name, gameName);

                await InteractiveServiceUnitTests.DeleteTestGame(connection, game);

                games = await connection.Interactive.GetOwnedInteractiveGames(channel);

                Assert.IsNotNull(games);
                Assert.IsFalse(games.Any(g => g.id.Equals(game.id)));
            });
        }
    }
}
