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
        public static async Task<UserModel> GetCurrentUser(MixerClient client)
        {
            ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(client);

            PrivatePopulatedUserModel user = await client.Users.GetCurrentUser();

            Assert.IsNotNull(user);
            Assert.IsTrue(user.id > (uint)0);

            return user;
        }

        [TestMethod]
        public void GetCurrentUser()
        {
            this.TestWrapper(async (MixerClient client) =>
            {
                UserModel user = await UsersServiceUnitTests.GetCurrentUser(client);
            });
        }

        [TestMethod]
        public void GetUser()
        {
            this.TestWrapper(async (MixerClient client) =>
            {
                UserModel user = await UsersServiceUnitTests.GetCurrentUser(client);

                UserWithChannelModel channelUser = await client.Users.GetUser(user);

                Assert.IsNotNull(channelUser);
                Assert.IsTrue(channelUser.id > (uint)0);
            });
        }

        [TestMethod]
        public void GetUserByUsername()
        {
            this.TestWrapper(async (MixerClient client) =>
            {
                UserModel user = await client.Users.GetUser("SaviorXTanren");

                Assert.IsNotNull(user);
                Assert.IsTrue(user.id > (uint)0);
            });
        }

        [TestMethod]
        public void GetAvatar()
        {
            this.TestWrapper(async (MixerClient client) =>
            {
                UserModel user = await UsersServiceUnitTests.GetCurrentUser(client);

                string avatar = await client.Users.GetAvatar(user);

                Assert.IsNotNull(avatar);
            });
        }

        [TestMethod]
        public void GetFollows()
        {
            this.TestWrapper(async (MixerClient client) =>
            {
                UserModel user = await UsersServiceUnitTests.GetCurrentUser(client);

                IEnumerable<ChannelAdvancedModel> follows = await client.Users.GetFollows(user);

                Assert.IsNotNull(follows);
                Assert.IsTrue(follows.Count() > 0);
            });
        }

        [TestMethod]
        public void GetLogs()
        {
            this.TestWrapper(async (MixerClient client) =>
            {
                UserModel user = await UsersServiceUnitTests.GetCurrentUser(client);

                IEnumerable<UserLogModel> logs = await client.Users.GetLogs(user);

                Assert.IsNotNull(logs);
                Assert.IsTrue(logs.Count() > 0);
            });
        }

        [TestMethod]
        public void GetNotifications()
        {
            this.TestWrapper(async (MixerClient client) =>
            {
                UserModel user = await UsersServiceUnitTests.GetCurrentUser(client);

                IEnumerable<NotificationModel> notifications = await client.Users.GetNotifications(user);

                Assert.IsNotNull(notifications);
                Assert.IsTrue(notifications.Count() > 0);
            });
        }

        [TestMethod]
        public void GetPreferences()
        {
            this.TestWrapper(async (MixerClient client) =>
            {
                UserModel user = await UsersServiceUnitTests.GetCurrentUser(client);

                UserPreferencesModel preferences = await client.Users.GetPreferences(user);

                Assert.IsNotNull(preferences);
            });
        }

        [TestMethod]
        public void UpdatePreferences()
        {
            this.TestWrapper(async (MixerClient client) =>
            {
                UserModel user = await UsersServiceUnitTests.GetCurrentUser(client);

                UserPreferencesModel preferences = await client.Users.GetPreferences(user);

                Assert.IsNotNull(preferences);

                preferences.chatLurkMode = !preferences.chatLurkMode;

                UserPreferencesModel newPreferences = await client.Users.UpdatePreferences(user, preferences);

                Assert.IsNotNull(preferences);
                Assert.AreEqual(preferences.chatLurkMode, newPreferences.chatLurkMode);

                preferences.chatLurkMode = !preferences.chatLurkMode;

                newPreferences = await client.Users.UpdatePreferences(user, preferences);

                Assert.IsNotNull(preferences);
                Assert.AreEqual(preferences.chatLurkMode, newPreferences.chatLurkMode);
            });
        }

        [TestMethod]
        public void GetTeams()
        {
            this.TestWrapper(async (MixerClient client) =>
            {
                UserModel user = await UsersServiceUnitTests.GetCurrentUser(client);

                IEnumerable<TeamMembershipExpandedModel> teams = await client.Users.GetTeams(user);

                Assert.IsNotNull(teams);
            });
        }
    }
}
