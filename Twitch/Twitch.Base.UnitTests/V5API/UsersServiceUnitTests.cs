using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Twitch.Base.Models.V5.Channel;
using Twitch.Base.Models.V5.Emotes;
using Twitch.Base.Models.V5.Users;

namespace Twitch.Base.UnitTests.V5API
{
    [TestClass]
    public class UsersServiceUnitTests : UnitTestBase
    {
        public static async Task<UserModel> GetCurrentUser(TwitchConnection connection)
        {
            UserModel user = await connection.V5API.Users.GetCurrentUser();

            Assert.IsNotNull(user);
            Assert.IsNotNull(user.id);

            return user;
        }

        [TestMethod]
        public void GetCurrentUser()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                UserModel user = await UsersServiceUnitTests.GetCurrentUser(connection);
            });
        }

        [TestMethod]
        public void GetUser()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                UserModel user = await UsersServiceUnitTests.GetCurrentUser(connection);

                UserModel result = await connection.V5API.Users.GetUser(user);

                Assert.IsNotNull(result);
                Assert.IsNotNull(result.id);
                Assert.AreEqual(user.id, result.id);
            });
        }

        [TestMethod]
        public void GetUserByID()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                UserModel user = await UsersServiceUnitTests.GetCurrentUser(connection);

                UserModel result = await connection.V5API.Users.GetUserByID(user.id);

                Assert.IsNotNull(result);
                Assert.IsNotNull(result.id);
                Assert.AreEqual(user.id, result.id);
            });
        }

        [TestMethod]
        public void GetUserByLogin()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                UserModel user = await UsersServiceUnitTests.GetCurrentUser(connection);

                UserModel result = await connection.V5API.Users.GetUserByLogin("SaviorXTanren");

                Assert.IsNotNull(result);
                Assert.IsNotNull(result.id);
                Assert.AreEqual(user.id, result.id);
            });
        }

        [TestMethod]
        public void GetUsersByLogin()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                UserModel user = await UsersServiceUnitTests.GetCurrentUser(connection);

                IEnumerable<UserModel> results = await connection.V5API.Users.GetUsersByLogin(new List<string>() { user.name, "Ninja", "Kitboga" });

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() > 0);
            });
        }

        [TestMethod]
        public void GetUserEmotes()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                UserModel user = await UsersServiceUnitTests.GetCurrentUser(connection);

                IEnumerable<EmoteModel> results = await connection.V5API.Users.GetUserEmotes(user);

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() > 0);
            });
        }

        [TestMethod]
        public void CheckUserSubscription()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                UserModel user = await UsersServiceUnitTests.GetCurrentUser(connection);

                ChannelModel channel = await connection.V5API.Channels.GetCurrentChannel();

                Assert.IsNotNull(channel);
                Assert.IsNotNull(channel.id);

                UserSubscriptionModel subscription = await connection.V5API.Users.CheckUserSubscription(user, channel);

                Assert.IsNotNull(subscription);
            });
        }

        [TestMethod]
        public void GetUserFollows()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                UserModel user = await UsersServiceUnitTests.GetCurrentUser(connection);

                IEnumerable<UserFollowModel> results = await connection.V5API.Users.GetUserFollows(user);

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() > 0);
            });
        }

        [TestMethod]
        public void CheckUserFollow()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                UserModel user = await UsersServiceUnitTests.GetCurrentUser(connection);

                ChannelModel channel = await connection.V5API.Channels.GetCurrentChannel();

                Assert.IsNotNull(channel);
                Assert.IsNotNull(channel.id);

                UserFollowModel follow = await connection.V5API.Users.CheckUserFollow(user, channel);

                Assert.IsNotNull(follow);
            });
        }

        [TestMethod]
        public void FollowChannel()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                UserModel user = await UsersServiceUnitTests.GetCurrentUser(connection);

                ChannelModel channel = await connection.V5API.Channels.GetCurrentChannel();

                UserFollowModel result = await connection.V5API.Users.FollowChannel(user, channel);

                Assert.IsNotNull(result);
                Assert.IsNotNull(result.user);
                Assert.IsNotNull(result.user.id);
            });
        }

        [TestMethod]
        public void UnfollowChannel()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                UserModel user = await UsersServiceUnitTests.GetCurrentUser(connection);

                ChannelModel channel = await connection.V5API.Channels.GetCurrentChannel();

                bool result = await connection.V5API.Users.UnfollowChannel(user, channel);

                Assert.IsTrue(result);
            });
        }

        [TestMethod]
        public void GetUserBlockList()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                UserModel user = await UsersServiceUnitTests.GetCurrentUser(connection);

                IEnumerable<UserModel> results = await connection.V5API.Users.GetUserBlockList(user);

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() > 0);
            });
        }

        [TestMethod]
        public void BlockUser()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                UserModel user = await UsersServiceUnitTests.GetCurrentUser(connection);

                UserModel result = await connection.V5API.Users.BlockUser(user, user);

                Assert.IsNotNull(result);
                Assert.IsNotNull(result.id);
            });
        }

        [TestMethod]
        public void UnblockUser()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                UserModel user = await UsersServiceUnitTests.GetCurrentUser(connection);

                bool result = await connection.V5API.Users.UnblockUser(user, user);

                Assert.IsTrue(result);
            });
        }
    }
}
