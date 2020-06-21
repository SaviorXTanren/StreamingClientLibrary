using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Twitch.Base.Models.V5.Channel;
using Twitch.Base.Models.V5.Chat;
using Twitch.Base.Models.V5.Emotes;
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
        public void GetChatEmoticonsForSet()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                IEnumerable<EmoteModel> results = await connection.V5API.Chat.GetChatEmoticonsForSet(new List<int>() { 19151 });

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() > 0);
            });
        }
    }
}

