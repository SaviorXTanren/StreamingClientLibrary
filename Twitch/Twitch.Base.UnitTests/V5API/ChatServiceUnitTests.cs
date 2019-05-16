using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Twitch.Base.Models.V5.Channel;
using Twitch.Base.Models.V5.Chat;
using Twitch.Base.Models.V5.Users;

namespace Twitch.Base.UnitTests.V5API
{
    [TestClass]
    public class ChatServiceUnitTests : UnitTestBase
    {
        [TestMethod]
        public void GetChannelChatBadges()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                UserModel user = await connection.V5API.Users.GetUserByLogin("Ninja");

                ChannelModel channel = await connection.V5API.Channels.GetChannelByID(user.id);

                ChannelChatBadgesModel result = await connection.V5API.Chat.GetChannelChatBadges(channel);

                Assert.IsNotNull(result);
                Assert.IsNotNull(result.subscriber);
            });
        }

        [TestMethod]
        public void GetChatRoomsForChannel()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                ChannelModel channel = await connection.V5API.Channels.GetCurrentChannel();

                IEnumerable<ChatRoomModel> results = await connection.V5API.Chat.GetChatRoomsForChannel(channel);

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() > 0);
            });
        }
    }
}

