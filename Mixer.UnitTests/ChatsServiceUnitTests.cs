using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mixer.Base;
using Mixer.Base.Model.Channel;
using Mixer.Base.Model.User;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mixer.UnitTests
{
    [TestClass]
    public class ChatsServiceUnitTests : UnitTestBase
    {
        public static async Task<ChannelChatModel> GetChat(MixerClient client)
        {
            ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(client);

            ChannelChatModel chat = await client.Chats.GetChat(channel);

            Assert.IsNotNull(chat);
            Assert.IsTrue(chat.endpoints.Count() > 0);

            return chat;
        }

        [TestMethod]
        public void GetChat()
        {
            this.TestWrapper(async (MixerClient client) =>
            {
                ChannelChatModel chat = await ChatsServiceUnitTests.GetChat(client);
            });
        }

        [TestMethod]
        public void GetUsers()
        {
            this.TestWrapper(async (MixerClient client) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(client);

                IEnumerable<ChatUserModel> users = await client.Chats.GetUsers(channel);

                Assert.IsNotNull(users);
                Assert.IsTrue(users.Count() > 0);
            });
        }
    }
}
