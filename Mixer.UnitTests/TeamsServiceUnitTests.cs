using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mixer.Base;
using Mixer.Base.Model.Teams;
using Mixer.Base.Model.User;
using System.Collections.Generic;
using System.Linq;

namespace Mixer.UnitTests
{
    [TestClass]
    public class TeamsServiceUnitTests : UnitTestBase
    {
        [TestMethod]
        public void GetTeamsAndUsers()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                IEnumerable<TeamModel> teams = await connection.Teams.GetTeams(1);

                Assert.IsNotNull(teams);
                Assert.IsTrue(teams.Count() > 0);

                TeamModel team = await connection.Teams.GetTeam(teams.First().id);

                Assert.IsNotNull(team);
                Assert.IsTrue(team.id > 0);

                IEnumerable<UserModel> users = await connection.Teams.GetTeamUsers(team, 1);

                Assert.IsNotNull(users);
                Assert.IsTrue(users.Count() > 0);
            });
        }
    }
}
