using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twitch.Base.Models.NewAPI.Bits;

namespace Twitch.Base.UnitTests.NewAPI
{
    [TestClass]
    public class BitsServiceUnitTests : UnitTestBase
    {
        [TestMethod]
        public void GetBitsLeaderboard()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                BitsLeaderboardModel result = await connection.NewAPI.Bits.GetBitsLeaderboard();

                Assert.IsNotNull(result);
                Assert.IsTrue(result.users.Count > 0);
            });
        }
    }
}
