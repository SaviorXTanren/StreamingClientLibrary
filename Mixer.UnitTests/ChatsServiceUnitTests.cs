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
        public static async Task<ChannelChatModel> GetChat(MixerConnection connection)
        {
            ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(connection);

            ChannelChatModel chat = await connection.Chats.GetChat(channel);

            Assert.IsNotNull(chat);
            Assert.IsTrue(chat.endpoints.Count() > 0);

            return chat;
        }

        [TestMethod]
        public void GetChat()
        {
            this.TestWrapper(async (MixerConnection connection) =>
            {
                ChannelChatModel chat = await ChatsServiceUnitTests.GetChat(connection);
            });
        }

        [TestMethod]
        public void GetUsers()
        {
            this.TestWrapper(async (MixerConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetChannel(connection);

                IEnumerable<ChatUserModel> users = await connection.Chats.GetUsers(channel);

                Assert.IsNotNull(users);
            });
        }
    }
}
