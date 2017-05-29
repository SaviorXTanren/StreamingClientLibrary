using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mixer.Base;
using Mixer.Base.Model.Channel;
using Mixer.Base.Model.User;
using System.Collections.Generic;
using System.Linq;

namespace Mixer.UnitTests
{
    [TestClass]
    public class ChatsUnitTests : UnitTestBase
    {
        [TestMethod]
        public void GetChat()
        {
            this.TestWrapper(async (MixerClient client) =>
            {
                ChannelModel channel = await ChannelUnitTests.GetChannel(client);

                IEnumerable<string> endpoints = await client.Chats.GetChat(channel);

                Assert.IsNotNull(endpoints);
                Assert.IsTrue(endpoints.Count() > 0);
            });
        }

        [TestMethod]
        public void GetUsers()
        {
            this.TestWrapper(async (MixerClient client) =>
            {
                ChannelModel channel = await ChannelUnitTests.GetChannel(client);

                IEnumerable<ChatUserModel> users = await client.Chats.GetUsers(channel);

                Assert.IsNotNull(users);
                Assert.IsTrue(users.Count() > 0);
            });
        }
    }
}
