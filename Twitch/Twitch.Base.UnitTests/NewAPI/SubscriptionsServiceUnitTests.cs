using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Twitch.Base.Models.NewAPI.Subscriptions;
using Twitch.Base.Models.NewAPI.Users;

namespace Twitch.Base.UnitTests.NewAPI
{
    [TestClass]
    public class SubscriptionsServiceUnitTests : UnitTestBase
    {
        [TestMethod]
        public void GetAllSubscriptions()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                UserModel user = await connection.NewAPI.Users.GetUserByLogin("Ninja");

                IEnumerable<SubscriptionModel> results = await connection.NewAPI.Subscriptions.GetAllSubscriptions(user);

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() > 0);
            });
        }

        [TestMethod]
        public void GetSubscriptions()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                UserModel user = await UsersServiceUnitTests.GetCurrentUser(connection);

                UserModel broadcaster = await connection.NewAPI.Users.GetUserByLogin("Ninja");

                IEnumerable<SubscriptionModel> results = await connection.NewAPI.Subscriptions.GetSubscriptions(broadcaster, new List<string>() { user.id });

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() > 0);
            });
        }
    }
}
