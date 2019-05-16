using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Twitch.Base.Models.NewAPI.Games;

namespace Twitch.Base.UnitTests.NewAPI
{
    [TestClass]
    public class GamesServiceUnitTests : UnitTestBase
    {
        [TestMethod]
        public void GetTopGames()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                IEnumerable<GameModel> results = await connection.NewAPI.Games.GetTopGames();

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() > 0);
            });
        }

        [TestMethod]
        public void GetGameByID()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                IEnumerable<GameModel> results = await connection.NewAPI.Games.GetGamesByName("Fortnite");

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() > 0);

                GameModel result = await connection.NewAPI.Games.GetGameByID(results.First().id);

                Assert.IsNotNull(result);
                Assert.IsNotNull(result.id);
                Assert.AreEqual(results.First().id, result.id);
            });
        }

        [TestMethod]
        public void GetGamesByName()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                IEnumerable<GameModel> results = await connection.NewAPI.Games.GetGamesByName("Fortnite");

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() > 0);
            });
        }
    }
}
