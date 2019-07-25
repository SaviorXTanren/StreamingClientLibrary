using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mixer.Base.Model.Channel;
using Mixer.Base.Model.MixPlay;
using Mixer.Base.Model.User;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mixer.Base.UnitTests
{
    [TestClass]
    public class MixPlayServiceUnitTests : UnitTestBase
    {
        private const string GameName = "Mixer C# Unit Tests Game";

        private static MixPlayGameListingModel testGameListing;

        public static async Task<MixPlayGameListingModel> CreateTestGame(MixerConnection connection, ChannelModel channel)
        {
            UserModel user = await UsersServiceUnitTests.GetCurrentUser(connection);

            IEnumerable<MixPlayGameListingModel> gameListings = await connection.MixPlay.GetOwnedMixPlayGames(channel);

            MixPlayGameListingModel previousTestGame = gameListings.FirstOrDefault(g => g.name.Equals(MixPlayServiceUnitTests.GameName));
            if (previousTestGame != null)
            {
                await MixPlayServiceUnitTests.DeleteTestGame(connection, previousTestGame);
            }

            MixPlayGameModel game = new MixPlayGameModel()
            {
                name = MixPlayServiceUnitTests.GameName,
                ownerId = user.id,             
            };
            game = await connection.MixPlay.CreateMixPlayGame(game);

            Assert.IsNotNull(game);
            Assert.IsTrue(game.id > 0);

            game.controlVersion = "2.0";
            game = await connection.MixPlay.UpdateMixPlayGame(game);

            Assert.IsNotNull(game);
            Assert.IsTrue(game.id > 0);

            gameListings = await connection.MixPlay.GetOwnedMixPlayGames(channel);

            Assert.IsNotNull(gameListings);
            Assert.IsTrue(gameListings.Count() > 0);

            MixPlayGameListingModel gameListing = gameListings.FirstOrDefault(gl => gl.id.Equals(game.id));
            Assert.IsNotNull(gameListing);

            MixPlayGameVersionModel version = gameListing.versions.First();
            MixPlaySceneModel defaultScene = new MixPlaySceneModel()
            {
                sceneID = "default",
            };

            defaultScene.buttons.Add(MixPlayClientUnitTests.CreateTestButton());
            defaultScene.joysticks.Add(MixPlayClientUnitTests.CreateTestJoystick());

            version.controls.scenes.Add(defaultScene);
            version.controlVersion = "2.0";
            version = await connection.MixPlay.UpdateMixPlayGameVersion(version);

            gameListings = await connection.MixPlay.GetOwnedMixPlayGames(channel);

            Assert.IsNotNull(gameListings);
            Assert.IsTrue(gameListings.Count() > 0);

            gameListing = gameListings.FirstOrDefault(gl => gl.id.Equals(game.id));
            Assert.IsNotNull(gameListing);

            return gameListing;
        }

        public static async Task DeleteTestGame(MixerConnection connection, MixPlayGameModel game)
        {
            Assert.IsTrue(await connection.MixPlay.DeleteMixPlayGame(game));
        }

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(connection);

                testGameListing = await MixPlayServiceUnitTests.CreateTestGame(connection, channel);
            });
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                await MixPlayServiceUnitTests.DeleteTestGame(connection, testGameListing);
            });
        }

        [TestMethod]
        public void GetMixPlayHosts()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                IEnumerable<string> addresses = await connection.MixPlay.GetMixPlayHosts();

                Assert.IsNotNull(addresses);
                Assert.IsTrue(addresses.Count() > 0);
            });
        }

        [TestMethod]
        public void GetOwnedMixPlayGames()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(connection);

                IEnumerable<MixPlayGameListingModel> games = await connection.MixPlay.GetOwnedMixPlayGames(channel);

                Assert.IsNotNull(games);
            });
        }

        [TestMethod]
        public void GetSharedMixPlayGames()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(connection);

                IEnumerable<MixPlayGameListingModel> games = await connection.MixPlay.GetSharedMixPlayGames(channel);

                Assert.IsNotNull(games);
            });
        }

        [TestMethod]
        public void GetEditorMixPlayGames()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(connection);

                IEnumerable<MixPlayGameListingModel> games = await connection.MixPlay.GetEditorMixPlayGames(channel);

                Assert.IsNotNull(games);
            });
        }


        [TestMethod]
        public void GetMixPlayVersion()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(connection);

                IEnumerable<MixPlayGameListingModel> games = await connection.MixPlay.GetOwnedMixPlayGames(channel);

                Assert.IsNotNull(games);
                Assert.IsTrue(games.Count() > 0);

                MixPlayGameVersionModel version = await connection.MixPlay.GetMixPlayGameVersion(games.First().versions.First());

                Assert.IsNotNull(version);
            });
        }

        [TestMethod]
        public void CreateGetUpdateDeleteGame()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(connection);

                MixPlayGameListingModel gameListing = await MixPlayServiceUnitTests.CreateTestGame(connection, channel);

                IEnumerable<MixPlayGameListingModel> games = await connection.MixPlay.GetOwnedMixPlayGames(channel);

                Assert.IsNotNull(games);
                Assert.IsTrue(games.Count() > 0);
                Assert.IsTrue(games.Any(g => g.id.Equals(gameListing.id)));

                string gameName = gameListing.name = "Test Game";
                MixPlayGameModel game = await connection.MixPlay.UpdateMixPlayGame(gameListing);

                Assert.IsNotNull(game);
                Assert.IsTrue(game.id > 0);
                Assert.AreEqual(game.name, gameName);

                await MixPlayServiceUnitTests.DeleteTestGame(connection, game);

                games = await connection.MixPlay.GetOwnedMixPlayGames(channel);

                Assert.IsNotNull(games);
                Assert.IsFalse(games.Any(g => g.id.Equals(game.id)));
            });
        }
    }
}
