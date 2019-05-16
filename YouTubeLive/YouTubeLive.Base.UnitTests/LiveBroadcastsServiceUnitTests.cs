using Google.Apis.YouTube.v3.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace YouTubeLive.Base.UnitTests
{
    [TestClass]
    public class LiveBroadcastsServiceUnitTests : UnitTestBase
    {
        [TestMethod]
        public void GetCommentThreadsForChannel()
        {
            TestWrapper(async (YouTubeLiveConnection connection) =>
            {
                IEnumerable<LiveBroadcast> results = await connection.LiveBroadcasts.GetMyBroadcasts();

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() > 0);
            });
        }
    }
}
