using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Twitch.Base.Models.V5.Bits;

namespace Twitch.Base.UnitTests.V5API
{
    [TestClass]
    public class BitsServiceUnitTests : UnitTestBase
    {
        [TestMethod]
        public void GetActions()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                IEnumerable<BitCheermoteModel> results = await connection.V5API.Bits.GetActions();

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() > 0);
            });
        }
    }
}
