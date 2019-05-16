using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Twitch.Base.Models.NewAPI.Clips;
using Twitch.Base.Models.NewAPI.Games;
using Twitch.Base.Models.NewAPI.Users;

namespace Twitch.Base.UnitTests.NewAPI
{
    [TestClass]
    public class ClipsServiceUnitTests : UnitTestBase
    {
        [TestMethod]
        public void CreateAndGetClip()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                UserModel user = await UsersServiceUnitTests.GetCurrentUser(connection);

                ClipCreationModel result = await connection.NewAPI.Clips.CreateClip(user);

                Assert.IsNotNull(result);
                Assert.IsNotNull(result.id);
            });
        }

        [TestMethod]
        public void GetClip()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                IEnumerable<GameModel> games = await connection.NewAPI.Games.GetGamesByName("Fortnite");

                Assert.IsNotNull(games);
                Assert.IsTrue(games.Count() > 0);

                IEnumerable<ClipModel> results = await connection.NewAPI.Clips.GetGameClips(games.First());

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() > 0);

                ClipModel result = await connection.NewAPI.Clips.GetClipByID(results.First().id);

                Assert.IsNotNull(result);
                Assert.IsNotNull(result.id);
            });
        }

        [TestMethod]
        public void GetBroadcasterClips()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                UserModel user = await connection.NewAPI.Users.GetUserByLogin("Ninja");

                IEnumerable<ClipModel> results = await connection.NewAPI.Clips.GetBroadcasterClips(user);

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() > 0);
            });
        }

        [TestMethod]
        public void GetGameClips()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                IEnumerable<GameModel> games = await connection.NewAPI.Games.GetGamesByName("Fortnite");

                Assert.IsNotNull(games);
                Assert.IsTrue(games.Count() > 0);

                IEnumerable<ClipModel> results = await connection.NewAPI.Clips.GetGameClips(games.First());

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() > 0);
            });
        }
    }
}
