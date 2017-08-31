using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mixer.Base;
using Mixer.Base.Model.Channel;
using Mixer.Base.Model.Game;
using Mixer.Base.Model.User;
using System.Collections;
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
    }
}
