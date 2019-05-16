using Google.Apis.YouTube.v3.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace YouTubeLive.Base.UnitTests
{
    [TestClass]
    public class SearchServiceUnitTests : UnitTestBase
    {
        [TestMethod]
        public void GetMyVideos()
        {
            TestWrapper(async (YouTubeLiveConnection connection) =>
            {
                IEnumerable<SearchResult> results = await connection.Search.GetMyVideos(maxResults: 10);

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() > 0);
            });
        }

        [TestMethod]
        public void SearchByKeyword()
        {
            TestWrapper(async (YouTubeLiveConnection connection) =>
            {
                IEnumerable<SearchResult> results = await connection.Search.SearchByKeyword("food", maxResults: 100);

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() == 100);
            });
        }
    }
}
