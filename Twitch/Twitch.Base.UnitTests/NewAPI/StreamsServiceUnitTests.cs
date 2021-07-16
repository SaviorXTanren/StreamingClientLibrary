using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Twitch.Base.Models.NewAPI.Streams;
using Twitch.Base.Models.NewAPI.Users;

namespace Twitch.Base.UnitTests.NewAPI
{
    [TestClass]
    public class StreamsServiceUnitTests : UnitTestBase
    {
        [TestMethod]
        public void GetTopStreams()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                IEnumerable<StreamModel> results = await connection.NewAPI.Streams.GetTopStreams();

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() > 0);
            });
        }

        [TestMethod]
        public void GetStreamsByUserIDs()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                UserModel user = await connection.NewAPI.Users.GetUserByLogin("TwitchPresents");

                IEnumerable<StreamModel> results = await connection.NewAPI.Streams.GetStreamsByUserIDs(new List<string>() { user.id });

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() > 0);
            });
        }

        [TestMethod]
        public void GetStreamsByLogins()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                IEnumerable<StreamModel> results = await connection.NewAPI.Streams.GetStreamsByLogins(new List<string>() { "TwitchPresents" });

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() > 0);
            });
        }

        [TestMethod]
        public void GetFollowedStreams()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                UserModel broadcaster = await UsersServiceUnitTests.GetCurrentUser(connection);

                IEnumerable<StreamModel> results = await connection.NewAPI.Streams.GetFollowedStreams(broadcaster);

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() > 0);
            });
        }

        [TestMethod]
        public void CreateStreamMarker()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                UserModel user = await connection.NewAPI.Users.GetUserByLogin("TwitchPresents");

                CreatedStreamMarkerModel result = await connection.NewAPI.Streams.CreateStreamMarker(user, "test");

                Assert.IsNotNull(result);
                Assert.IsNotNull(result.id);
            });
        }
    }
}
