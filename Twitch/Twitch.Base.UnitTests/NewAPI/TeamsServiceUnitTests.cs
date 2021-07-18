using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Twitch.Base.Models.NewAPI.Teams;
using Twitch.Base.Models.NewAPI.Users;

namespace Twitch.Base.UnitTests.NewAPI
{
    [TestClass]
    public class TeamsServiceUnitTests : UnitTestBase
    {
        [TestMethod]
        public void GetChannelTeams()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                UserModel user = await UsersServiceUnitTests.GetCurrentUser(connection);

                IEnumerable<TeamModel> results = await connection.NewAPI.Teams.GetChannelTeams(user);

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() > 0);
            });
        }

        [TestMethod]
        public void GetTeam()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                UserModel user = await UsersServiceUnitTests.GetCurrentUser(connection);

                IEnumerable<TeamModel> results = await connection.NewAPI.Teams.GetChannelTeams(user);

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() > 0);

                TeamDetailsModel team = await connection.NewAPI.Teams.GetTeam(results.First().id);

                Assert.IsNotNull(team);
                Assert.AreEqual(results.First().id, team.id);
            });
        }
    }
}
