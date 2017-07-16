using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mixer.Base;
using Mixer.Base.Model.Channel;
using Mixer.Base.Model.User;
using System.Collections.Generic;
using System.Configuration;
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
            string channelName = ConfigurationManager.AppSettings["ChannelName"];
            if (string.IsNullOrEmpty(channelName))
            {
                Assert.Fail("ChannelName value isn't set in application configuration");
            }

            ChannelModel channel = await connection.Channels.GetChannel(channelName);

            Assert.IsNotNull(channel);
            Assert.IsTrue(channel.id > (uint)0);

            return channel;
        }

        [TestMethod]
        public void GetChannelForUser()
        {
            this.TestWrapper(async (MixerConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(connection);
            });
        }

        /// <summary>
        /// New bug where channel updating does not seem to be working correctly, need to investigate why the call is returning with a 403 error
        /// </summary>
        [TestMethod]
        public void UpdateChannel()
        {
            this.TestWrapper(async (MixerConnection connection) =>
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
        public void GetFollowers()
        {
            this.TestWrapper(async (MixerConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(connection);

                IEnumerable<UserWithChannelModel> users = await connection.Channels.GetFollowers(channel);

                Assert.IsNotNull(users);
                Assert.IsTrue(users.Count() > 0);
            });
        }

        [TestMethod]
        public void GetHosters()
        {
            this.TestWrapper(async (MixerConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(connection);

                IEnumerable<ChannelAdvancedModel> hosters = await connection.Channels.GetHosters(channel);

                Assert.IsNotNull(hosters);
            });
        }

        [TestMethod]
        public void GetPreferences()
        {
            this.TestWrapper(async (MixerConnection connection) =>
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
            this.TestWrapper(async (MixerConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(connection);

                IEnumerable<UserWithGroupsModel> users = await connection.Channels.GetUsersWithRoles(channel);

                Assert.IsNotNull(users);
                Assert.IsTrue(users.Count() > 0);
            });
        }
    }
}
