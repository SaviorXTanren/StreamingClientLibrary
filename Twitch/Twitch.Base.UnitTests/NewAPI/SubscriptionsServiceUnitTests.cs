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
        public void GetBroadcasterSubscriptions()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                UserModel user = await UsersServiceUnitTests.GetCurrentUser(connection);

                IEnumerable<SubscriptionModel> results = await connection.NewAPI.Subscriptions.GetBroadcasterSubscriptions(user);

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() > 0);
            });
        }

        [TestMethod]
        public void GetSpecificBroadcasterSubscriptions()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                UserModel broadcaster = await UsersServiceUnitTests.GetCurrentUser(connection);
                UserModel user = await connection.NewAPI.Users.GetUserByLogin("Ninja");

                IEnumerable<SubscriptionModel> results = await connection.NewAPI.Subscriptions.GetBroadcasterSubscriptions(broadcaster, new List<string>() { user.id });

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() > 0);
            });
        }

        [TestMethod]
        public void GetBroadcasterSubscriptionsCount()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                UserModel user = await UsersServiceUnitTests.GetCurrentUser(connection);

                long result = await connection.NewAPI.Subscriptions.GetBroadcasterSubscriptionsCount(user);

                Assert.IsTrue(result > 0);
            });
        }

        [TestMethod]
        public void GetBroadcasterSubscriptionPoints()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                UserModel user = await UsersServiceUnitTests.GetCurrentUser(connection);

                long result = await connection.NewAPI.Subscriptions.GetBroadcasterSubscriptionPoints(user);

                Assert.IsTrue(result > 0);
            });
        }
    }
}
