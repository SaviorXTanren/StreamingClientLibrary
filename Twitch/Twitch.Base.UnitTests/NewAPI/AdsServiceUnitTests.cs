using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twitch.Base.Models.NewAPI.Ads;
using Twitch.Base.Models.NewAPI.Users;

namespace Twitch.Base.UnitTests.NewAPI
{
    [TestClass]
    public class AdsServiceUnitTests : UnitTestBase
    {
        [TestMethod]
        public void RunAd()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                UserModel channel = await connection.NewAPI.Users.GetCurrentUser();

                Assert.IsNotNull(channel);

                AdResponseModel result = await connection.NewAPI.Ads.RunAd(channel, 30);

                Assert.IsNotNull(result);
                Assert.IsTrue(string.IsNullOrEmpty(result.message));
            });
        }
    }
}
