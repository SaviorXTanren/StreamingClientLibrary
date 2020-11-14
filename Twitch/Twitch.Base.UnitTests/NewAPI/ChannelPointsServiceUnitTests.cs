using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Twitch.Base.Models.NewAPI.ChannelPoints;
using Twitch.Base.Models.NewAPI.Users;

namespace Twitch.Base.UnitTests.NewAPI
{
    [TestClass]
    public class ChannelPointsServiceUnitTests : UnitTestBase
    {
        [TestMethod]
        public void GetCustomRewards()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                UserModel broadcaster = await connection.NewAPI.Users.GetCurrentUser();

                Assert.IsNotNull(broadcaster);

                IEnumerable<CustomChannelPointRewardModel> results = await connection.NewAPI.ChannelPoints.GetCustomRewards(broadcaster);

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() > 0);

                CustomChannelPointRewardModel result = await connection.NewAPI.ChannelPoints.GetCustomReward(broadcaster, results.First().id);

                Assert.IsNotNull(result);
                Assert.AreEqual(results.First().id, result.id);
            });
        }

        [TestMethod]
        public void CreateUpdateDeleteCustomReward()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                UserModel broadcaster = await connection.NewAPI.Users.GetCurrentUser();

                Assert.IsNotNull(broadcaster);

                UpdatableCustomChannelPointRewardModel update = new UpdatableCustomChannelPointRewardModel()
                {
                    title = "Test Channel Point Reward",
                    cost = 123
                };

                CustomChannelPointRewardModel result = await connection.NewAPI.ChannelPoints.CreateCustomReward(broadcaster, update);

                Assert.IsNotNull(result);
                Assert.AreEqual(result.title, update.title);
                Assert.AreEqual(result.cost, update.cost);

                result = await connection.NewAPI.ChannelPoints.GetCustomReward(broadcaster, result.id);

                Assert.IsNotNull(result);
                Assert.AreEqual(result.title, update.title);
                Assert.AreEqual(result.cost, update.cost);

                update.title = "Newer Test Channel Point Reward";
                update.cost = 456;

                result = await connection.NewAPI.ChannelPoints.UpdateCustomReward(broadcaster, result.id, update);

                Assert.IsNotNull(result);
                Assert.AreEqual(result.title, update.title);
                Assert.AreEqual(result.cost, update.cost);

                result = await connection.NewAPI.ChannelPoints.GetCustomReward(broadcaster, result.id);

                Assert.IsNotNull(result);
                Assert.AreEqual(result.title, update.title);
                Assert.AreEqual(result.cost, update.cost);

                Assert.IsTrue(await connection.NewAPI.ChannelPoints.DeleteCustomReward(broadcaster, result.id));
            });
        }
    }
}
