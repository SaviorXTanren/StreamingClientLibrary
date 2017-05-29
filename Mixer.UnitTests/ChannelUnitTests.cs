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
    public class ChannelUnitTests : UnitTestBase
    {
        [TestMethod]
        public void GetChannelForUser()
        {
            this.TestWrapper(async (MixerClient client) =>
            {
                ChannelModel channel = await this.GetChannel(client);
            });
        }

        [TestMethod]
        public void UpdateChannel()
        {
            this.TestWrapper(async (MixerClient client) =>
            {
                ChannelModel channel = await this.GetChannel(client);

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
                ChannelModel channel = await this.GetChannel(client);

                IEnumerable<UserWithChannelModel> users = await client.Channels.GetFollowers(channel);

                Assert.IsNotNull(users);
                Assert.IsTrue(users.Count() > 0);
            });
        }

        [TestMethod]
        public void GetPreferences()
        {
            this.TestWrapper(async (MixerClient client) =>
            {
                ChannelModel channel = await this.GetChannel(client);

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

        private async Task<ChannelModel> GetChannel(MixerClient client)
        {
            ChannelModel channel = await client.Channels.GetChannel(this.GetChannelName());

            Assert.IsNotNull(channel);
            Assert.AreEqual(channel.id, (uint)5641335);

            return channel;
        }

        private string GetChannelName()
        {
            string channelName = ConfigurationManager.AppSettings["ChannelName"];
            if (string.IsNullOrEmpty(channelName))
            {
                Assert.Fail("ChannelName value isn't set in application configuration");
            }
            return channelName;
        }
    }
}
