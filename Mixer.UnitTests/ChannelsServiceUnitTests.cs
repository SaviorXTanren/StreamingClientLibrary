using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mixer.Base;
using Mixer.Base.Model.Broadcast;
using Mixer.Base.Model.Channel;
using Mixer.Base.Model.Game;
using Mixer.Base.Model.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mixer.UnitTests
{
    /// <summary>
    /// Summary description for ChannelUnitTests
    /// </summary>
    [TestClass]
    public class ChannelsServiceUnitTests : UnitTestBase
    {
        public static async Task<ChannelModel> GetChannel(MixerConnection connection)
        {
            UserModel user = await connection.Users.GetCurrentUser();

            Assert.IsNotNull(user);
            Assert.IsTrue(user.id > (uint)0);

            ChannelModel channel = await connection.Channels.GetChannel(user.username);

            Assert.IsNotNull(channel);
            Assert.IsTrue(channel.id > (uint)0);

            return channel;
        }

        [TestMethod]
        public void GetChannelForUser()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(connection);
            });
        }

        [TestMethod]
        public void GetChannelByID()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                UserModel user = await connection.Users.GetCurrentUser();

                Assert.IsNotNull(user);
                Assert.IsTrue(user.id > (uint)0);

                ChannelModel channel = await connection.Channels.GetChannel(user.username);

                Assert.IsNotNull(channel);
                Assert.IsTrue(channel.id > (uint)0);

                channel = await connection.Channels.GetChannel(channel.id);

                Assert.IsNotNull(channel);
                Assert.IsTrue(channel.id > (uint)0);
            });
        }

        [TestMethod]
        public void GetChannelDetails()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                UserModel user = await connection.Users.GetCurrentUser();

                Assert.IsNotNull(user);
                Assert.IsTrue(user.id > (uint)0);

                ChannelDetailsModel channelDetails = await connection.Channels.GetChannelDetails(user.username);

                Assert.IsNotNull(channelDetails);
                Assert.IsTrue(channelDetails.id > (uint)0);
            });
        }

        [TestMethod]
        public void GetFeaturedChannels()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                IEnumerable<ExpandedChannelModel> channels = await connection.Channels.GetFeaturedChannels();
                Assert.IsNotNull(channels);
                Assert.IsTrue(channels.Count() > 0);
                Assert.IsTrue(channels.Select(c => c.id).Distinct().Count() >= 1);
            });
        }

        [TestMethod]
        public void GetOnlineChannels()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                IEnumerable<ExpandedChannelModel> channels = await connection.Channels.GetChannels(maxResults: 150);
                Assert.IsNotNull(channels);
                Assert.IsTrue(channels.Count() > 0);
                Assert.IsTrue(channels.Select(c => c.id).Distinct().Count() >= 150);
            });
        }

        [TestMethod]
        public void GetChannels()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                IEnumerable<ExpandedChannelModel> channels = await connection.Channels.GetChannels(new List<uint>() { 41088881, 10961439, 31961586 });
                Assert.IsNotNull(channels);
                Assert.IsTrue(channels.Count() > 0);

                channels = await connection.Channels.GetChannelsFromUsers(new List<uint>() { 49048475, 14128065, 39072213 });
                Assert.IsNotNull(channels);
                Assert.IsTrue(channels.Count() > 0);
            });
        }

        [TestMethod]
        public void UpdateChannel()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(connection);

                string newName = "Test Name";
                channel.name = newName;

                channel = await connection.Channels.UpdateChannel(channel);

                Assert.IsNotNull(channel);
                Assert.IsTrue(string.Equals(channel.name, newName));
            });
        }

        [TestMethod]
        public void UpdateChannelGame()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(connection);

                string gameName = "Fortnite";
                IEnumerable<GameTypeSimpleModel> games = await connection.GameTypes.GetGameTypes(gameName);

                Assert.IsNotNull(games);
                Assert.IsTrue(games.Count() > 0);
                Assert.IsTrue(games.Any(gt => gt.name.Equals(gameName)));

                channel.typeId = games.First().id;

                channel = await connection.Channels.UpdateChannel(channel);

                Assert.IsNotNull(channel);
                Assert.IsTrue(channel.typeId == games.First().id);
            });
        }

        [TestMethod]
        public void GetViewerAnalytics()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(connection);

                IEnumerable<ViewerAnalyticModel> viewers = await connection.Channels.GetViewerAnalytics(channel, DateTimeOffset.Now.Subtract(TimeSpan.FromDays(7)));

                Assert.IsNotNull(viewers);
                Assert.IsTrue(viewers.Count() > 0);
            });
        }

        [TestMethod]
        public void GetStreamSessions()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(connection);

                IEnumerable<StreamSessionsAnalyticModel> streams = await connection.Channels.GetStreamSessions(channel, DateTimeOffset.Now.Subtract(TimeSpan.FromDays(7)));

                Assert.IsNotNull(streams);
                Assert.IsTrue(streams.Count() > 0);
            });
        }

        [TestMethod]
        public void GetFollowerAnalytics()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(connection);

                IEnumerable<FollowersAnalyticModel> followers = await connection.Channels.GetFollowerAnalytics(channel, DateTimeOffset.Now.Subtract(TimeSpan.FromDays(7)));

                Assert.IsNotNull(followers);
                Assert.IsTrue(followers.Count() > 0);
            });
        }

        [TestMethod]
        public void GetSubscriberAnalytics()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(connection);

                IEnumerable<SubscriberAnalyticModel> subscribers = await connection.Channels.GetSubscriberAnalytics(channel, DateTimeOffset.Now.Subtract(TimeSpan.FromDays(7)));

                Assert.IsNotNull(subscribers);
                Assert.IsTrue(subscribers.Count() > 0);
            });
        }

        [TestMethod]
        public void GetFollowers()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(connection);

                IEnumerable<UserWithChannelModel> users = await connection.Channels.GetFollowers(channel, 1);

                Assert.IsNotNull(users);
                Assert.IsTrue(users.Count() > 0);
            });
        }

        [TestMethod]
        public void CheckIfFollows()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(connection);

                IEnumerable<UserWithChannelModel> followedUsers = await connection.Channels.GetFollowers(channel, 1);
                Assert.IsNotNull(followedUsers);
                Assert.IsTrue(followedUsers.Count() > 0);

                UserModel notFollowedUser = await connection.Users.GetUser("ChannelOne");
                Assert.IsNotNull(notFollowedUser);

                Dictionary<uint, DateTimeOffset?> follows = await connection.Channels.CheckIfFollows(channel, new List<UserModel>() { followedUsers.First(), notFollowedUser });
                Assert.IsNotNull(follows);
                Assert.IsTrue(follows.Count == 2);
                Assert.IsNotNull(follows[followedUsers.First().id]);
                Assert.IsNull(follows[notFollowedUser.id]);
            });
        }

        [TestMethod]
        public void HostGetUnhostChannel()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(connection);

                ChannelModel hostChannel = await connection.Channels.GetChannel("Mixer");

                channel = await connection.Channels.SetHostChannel(channel, hostChannel);

                Assert.IsNotNull(channel);

                ChannelModel hostedChannel = await connection.Channels.GetHostedChannel(channel);

                Assert.IsNotNull(hostedChannel);
                Assert.AreEqual(hostChannel.id, hostedChannel.id);

                bool result = await connection.Channels.UnhostChannel(channel);

                Assert.IsTrue(result);
            });
        }

        [TestMethod]
        public void GetHosters()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(connection);

                IEnumerable<ChannelAdvancedModel> hosters = await connection.Channels.GetHosters(channel, 1);

                Assert.IsNotNull(hosters);
            });
        }

        [TestMethod]
        public void GetPreferences()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(connection);

                ChannelPreferencesModel preference = await connection.Channels.GetPreferences(channel);

                Assert.IsNotNull(preference);

                preference.linksClickable = !preference.linksClickable;

                ChannelPreferencesModel newPreferences = await connection.Channels.UpdatePreferences(channel, preference);

                Assert.IsNotNull(newPreferences);
                Assert.AreEqual(preference.linksClickable, newPreferences.linksClickable);

                preference.linksClickable = !preference.linksClickable;

                newPreferences = await connection.Channels.UpdatePreferences(channel, preference);

                Assert.IsNotNull(newPreferences);
                Assert.AreEqual(preference.linksClickable, newPreferences.linksClickable);
            });
        }

        [TestMethod]
        public void GetUsersWithRoles()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(connection);

                IEnumerable<UserWithGroupsModel> users = await connection.Channels.GetUsersWithRoles(channel, 1);

                Assert.IsNotNull(users);
                Assert.IsTrue(users.Count() > 0);

                users = await connection.Channels.GetUsersWithRoles(channel, "mod", 1);

                Assert.IsNotNull(users);
                Assert.IsTrue(users.Count() > 0);

                UserWithGroupsModel user = await connection.Channels.GetUser(channel, users.First().id);

                Assert.IsNotNull(user);
                Assert.IsTrue(user.id > 0);
            });
        }

        [TestMethod]
        public void CheckIfUsersHaveRole()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(connection);

                UserModel modUser = await connection.Users.GetUser("SXTBot");
                UserModel notModUser = await connection.Users.GetUser("ChannelOne");

                Dictionary<uint, DateTimeOffset?> usersWithRole = await connection.Channels.CheckIfUsersHaveRole(channel, new List<UserModel>() { modUser, notModUser }, "Mod");
                Assert.IsNotNull(usersWithRole);
                Assert.IsTrue(usersWithRole.Count == 1);
                Assert.IsNotNull(usersWithRole.ContainsKey(modUser.id));
            });
        }

        [TestMethod]
        public void UpdateUserRoles()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(connection);

                UserModel user = await connection.Users.GetUser("ChannelOne");

                Assert.IsNotNull(user);

                bool result = await connection.Channels.UpdateUserRoles(channel, user, new List<string>() { "Mod" }, null);

                Assert.IsTrue(result);

                IEnumerable<UserWithGroupsModel> userRoles = await connection.Channels.GetUsersWithRoles(channel, "Mod");

                Assert.IsNotNull(userRoles);
                Assert.IsTrue(userRoles.Any(ug => ug.username.Equals("ChannelOne")));

                result = await connection.Channels.UpdateUserRoles(channel, user, null, new List<string>() { "Mod" });

                Assert.IsTrue(result);

                userRoles = await connection.Channels.GetUsersWithRoles(channel, "Mod");

                Assert.IsNotNull(userRoles);
                Assert.IsFalse(userRoles.Any(ug => ug.username.Equals("ChannelOne")));
            });
        }

        [TestMethod]
        public void GetUpdateDiscord()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(connection);

                DiscordBotModel discord = await connection.Channels.GetDiscord(channel);

                Assert.IsNotNull(discord);
                Assert.IsTrue(discord.id > 0);

                IEnumerable<DiscordChannelModel> channels = await connection.Channels.GetDiscordChannels(channel);

                Assert.IsNotNull(channels);
                Assert.IsTrue(channels.Count() > 0);

                IEnumerable<DiscordRoleModel> roles = await connection.Channels.GetDiscordRoles(channel);

                Assert.IsNotNull(roles);
                Assert.IsTrue(roles.Count() > 0);

                discord = await connection.Channels.UpdateDiscord(channel, discord);

                Assert.IsNotNull(discord);
                Assert.IsTrue(discord.id > 0);
            });
        }

        [TestMethod]
        public void CheckGetDiscordInvite()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(connection);

                UserModel user = await connection.Users.GetCurrentUser();

                bool result = await connection.Channels.CanUserGetDiscordInvite(channel, user);

                Assert.IsTrue(result);

                string invite = await connection.Channels.GetUserDiscordInvite(channel, user);

                Assert.IsNotNull(invite);
                Assert.IsTrue(invite.Length > 0);
            });
        }

        [TestMethod]
        public void GetChannelBroadcast()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(connection);

                Assert.IsNotNull(channel);
                Assert.IsTrue(channel.id > (uint)0);

                BroadcastModel broadcast = await connection.Channels.GetCurrentBroadcast(channel);

                Assert.IsNotNull(broadcast);
                Assert.AreEqual(channel.id, broadcast.channelId);
            });
        }

        [TestMethod]
        public void GetChannelRecordings()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(connection);

                Assert.IsNotNull(channel);
                Assert.IsTrue(channel.id > (uint)0);

                IEnumerable<ChannelRecordingModel> recordings = await connection.Channels.GetRecordings(channel);

                Assert.IsNotNull(recordings);
                Assert.IsTrue(recordings.Count() > 0);
            });
        }

        [TestMethod]
        public void GetChannelEmoticons()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(connection);
                UserModel user = await connection.Users.GetCurrentUser();

                Assert.IsNotNull(user);
                Assert.IsTrue(user.id > (uint)0);

                IEnumerable<EmoticonPackModel> emoticonPacks = await connection.Channels.GetEmoticons(channel, user);

                Assert.IsNotNull(emoticonPacks);
            });
        }
    }
}
