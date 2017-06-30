using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mixer.Base;
using Mixer.Base.Model;
using Mixer.Base.Model.Channel;
using Mixer.Base.Model.Teams;
using Mixer.Base.Model.User;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mixer.UnitTests
{
    [TestClass]
    public class UsersServiceUnitTests : UnitTestBase
    {
        public static async Task<UserModel> GetCurrentUser(MixerConnection connection)
        {
            ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(connection);

            PrivatePopulatedUserModel user = await connection.Users.GetCurrentUser();

            Assert.IsNotNull(user);
            Assert.IsTrue(user.id > (uint)0);

            return user;
        }

        [TestMethod]
        public void GetCurrentUser()
        {
            this.TestWrapper(async (MixerConnection connection) =>
            {
                UserModel user = await UsersServiceUnitTests.GetCurrentUser(connection);
            });
        }

        [TestMethod]
        public void GetUser()
        {
            this.TestWrapper(async (MixerConnection connection) =>
            {
                UserModel user = await UsersServiceUnitTests.GetCurrentUser(connection);

                UserWithChannelModel channelUser = await connection.Users.GetUser(user);

                Assert.IsNotNull(channelUser);
                Assert.IsTrue(channelUser.id > (uint)0);
            });
        }

        [TestMethod]
        public void GetUserByUsername()
        {
            this.TestWrapper(async (MixerConnection connection) =>
            {
                UserModel user = await connection.Users.GetUser("SaviorXTanren");

                Assert.IsNotNull(user);
                Assert.IsTrue(user.id > (uint)0);
            });
        }

        [TestMethod]
        public void GetAvatar()
        {
            this.TestWrapper(async (MixerConnection connection) =>
            {
                UserModel user = await UsersServiceUnitTests.GetCurrentUser(connection);

                string avatar = await connection.Users.GetAvatar(user);

                Assert.IsNotNull(avatar);
            });
        }

        [TestMethod]
        public void GetFollows()
        {
            this.TestWrapper(async (MixerConnection connection) =>
            {
                UserModel user = await UsersServiceUnitTests.GetCurrentUser(connection);

                IEnumerable<ChannelAdvancedModel> follows = await connection.Users.GetFollows(user);

                Assert.IsNotNull(follows);
                Assert.IsTrue(follows.Count() > 0);
            });
        }

        [TestMethod]
        public void GetLogs()
        {
            this.TestWrapper(async (MixerConnection connection) =>
            {
                UserModel user = await UsersServiceUnitTests.GetCurrentUser(connection);

                IEnumerable<UserLogModel> logs = await connection.Users.GetLogs(user);

                Assert.IsNotNull(logs);
                Assert.IsTrue(logs.Count() > 0);
            });
        }

        [TestMethod]
        public void GetNotifications()
        {
            this.TestWrapper(async (MixerConnection connection) =>
            {
                UserModel user = await UsersServiceUnitTests.GetCurrentUser(connection);

                IEnumerable<NotificationModel> notifications = await connection.Users.GetNotifications(user);

                Assert.IsNotNull(notifications);
                Assert.IsTrue(notifications.Count() > 0);
            });
        }

        [TestMethod]
        public void GetPreferences()
        {
            this.TestWrapper(async (MixerConnection connection) =>
            {
                UserModel user = await UsersServiceUnitTests.GetCurrentUser(connection);

                UserPreferencesModel preferences = await connection.Users.GetPreferences(user);

                Assert.IsNotNull(preferences);
            });
        }

        [TestMethod]
        public void UpdatePreferences()
        {
            this.TestWrapper(async (MixerConnection connection) =>
            {
                UserModel user = await UsersServiceUnitTests.GetCurrentUser(connection);

                UserPreferencesModel preferences = await connection.Users.GetPreferences(user);

                Assert.IsNotNull(preferences);

                preferences.chatLurkMode = !preferences.chatLurkMode;

                UserPreferencesModel newPreferences = await connection.Users.UpdatePreferences(user, preferences);

                Assert.IsNotNull(preferences);
                Assert.AreEqual(preferences.chatLurkMode, newPreferences.chatLurkMode);

                preferences.chatLurkMode = !preferences.chatLurkMode;

                newPreferences = await connection.Users.UpdatePreferences(user, preferences);

                Assert.IsNotNull(preferences);
                Assert.AreEqual(preferences.chatLurkMode, newPreferences.chatLurkMode);
            });
        }

        [TestMethod]
        public void GetTeams()
        {
            this.TestWrapper(async (MixerConnection connection) =>
            {
                UserModel user = await UsersServiceUnitTests.GetCurrentUser(connection);

                IEnumerable<TeamMembershipExpandedModel> teams = await connection.Users.GetTeams(user);

                Assert.IsNotNull(teams);
            });
        }
    }
}
