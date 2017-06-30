using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mixer.Base.Clients;
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
        public static async Task<ChannelModel> GetChannel(MixerClient client)
        {
            string channelName = ConfigurationManager.AppSettings["ChannelName"];
            if (string.IsNullOrEmpty(channelName))
            {
                Assert.Fail("ChannelName value isn't set in application configuration");
            }

            ChannelModel channel = await client.Channels.GetChannel(channelName);

            Assert.IsNotNull(channel);
            Assert.IsTrue(channel.id > (uint)0);

            return channel;
        }

        [TestMethod]
        public void GetChannelForUser()
        {
            this.TestWrapper(async (MixerClient client) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(client);
            });
        }

        [TestMethod]
        public void UpdateChannel()
        {
            this.TestWrapper(async (MixerClient client) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(client);

                string newName = "Test Name";
                channel.name = newName;

                channel = await client.Channels.UpdateChannel(channel);

                Assert.IsNotNull(channel);
                Assert.IsTrue(string.Equals(channel.name, newName));
            });
        }

        [TestMethod]
        public void GetFollowers()
        {
            this.TestWrapper(async (MixerClient client) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(client);

                IEnumerable<UserWithChannelModel> users = await client.Channels.GetFollowers(channel);

                Assert.IsNotNull(users);
                Assert.IsTrue(users.Count() > 0);
            });
        }

        [TestMethod]
        public void GetHosters()
        {
            this.TestWrapper(async (MixerClient client) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(client);

                IEnumerable<ChannelAdvancedModel> hosters = await client.Channels.GetHosters(channel);

                Assert.IsNotNull(hosters);
            });
        }

        [TestMethod]
        public void GetPreferences()
        {
            this.TestWrapper(async (MixerClient client) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(client);

                ChannelPreferencesModel preference = await client.Channels.GetPreferences(channel);

                Assert.IsNotNull(preference);

                preference.linksClickable = !preference.linksClickable;

                ChannelPreferencesModel newPreferences = await client.Channels.UpdatePreferences(channel, preference);

                Assert.IsNotNull(newPreferences);
                Assert.AreEqual(preference.linksClickable, newPreferences.linksClickable);

                preference.linksClickable = !preference.linksClickable;

                newPreferences = await client.Channels.UpdatePreferences(channel, preference);

                Assert.IsNotNull(newPreferences);
                Assert.AreEqual(preference.linksClickable, newPreferences.linksClickable);
            });
        }

        [TestMethod]
        public void GetUsersWithRoles()
        {
            this.TestWrapper(async (MixerClient client) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(client);

                IEnumerable<UserWithGroupsModel> users = await client.Channels.GetUsersWithRoles(channel);

                Assert.IsNotNull(users);
                Assert.IsTrue(users.Count() > 0);
            });
        }
    }
}
