using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mixer.Base.Model.User;

namespace Mixer.Base.UnitTests
{
    [TestClass]
    public class AdsServiceUnitTests : UnitTestBase
    {
        [TestMethod]
        public void GetCurrentBroadcast()
        {
            TestWrapper(async (MixerConnection connection) =>
            {
                PrivatePopulatedUserModel user = await connection.Users.GetCurrentUser();

                Assert.IsNotNull(user);
                Assert.IsTrue(user.id > 0);

                bool success = await connection.Ads.RunAd(user.channel);

                Assert.IsTrue(success);
            });
        }
    }
}