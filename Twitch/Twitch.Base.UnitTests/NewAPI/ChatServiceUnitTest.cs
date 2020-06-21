using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Twitch.Base.Models.NewAPI.Chat;
using Twitch.Base.Models.NewAPI.Users;

namespace Twitch.Base.UnitTests.NewAPI
{
    [TestClass]
    public class ChatServiceUnitTest : UnitTestBase
    {
        [TestMethod]
        public void GetChannelChatBadges()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                UserModel user = await UsersServiceUnitTests.GetCurrentUser(connection);
                Assert.IsNotNull(user);

                IEnumerable<ChatBadgeSetModel> badges = await connection.NewAPI.Chat.GetChannelChatBadges(user);
                Assert.IsNotNull(badges);
                Assert.IsTrue(badges.Count() > 0);
            });
        }

        [TestMethod]
        public void GetGlobalChatBadges()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                IEnumerable<ChatBadgeSetModel> badges = await connection.NewAPI.Chat.GetGlobalChatBadges();
                Assert.IsNotNull(badges);
                Assert.IsTrue(badges.Count() > 0);
            });
        }
    }
}
