using Glimesh.Base.Models.Channels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Glimesh.Base.UnitTests
{
    [TestClass]
    public class CategoryServiceUnitTests : UnitTestBase
    {
        [TestMethod]
        public void GetCategories()
        {
            TestWrapper(async (GlimeshConnection connection) =>
            {
                IEnumerable<CategoryModel> categories = await connection.Category.GetCategories();
                Assert.IsNotNull(categories);
                Assert.IsTrue(categories.Count() > 0);
                Assert.IsNotNull(categories.First());
                Assert.IsTrue(!string.IsNullOrEmpty(categories.First().id));
                Assert.IsTrue(!string.IsNullOrEmpty(categories.First().name));
                Assert.IsTrue(!string.IsNullOrEmpty(categories.First().slug));
            });
        }

        [TestMethod]
        public void GetCategoryBySlug()
        {
            TestWrapper(async (GlimeshConnection connection) =>
            {
                IEnumerable<CategoryModel> categories = await connection.Category.GetCategories();
                Assert.IsNotNull(categories);
                Assert.IsTrue(categories.Count() > 0);
                Assert.IsNotNull(categories.First());
                Assert.IsTrue(!string.IsNullOrEmpty(categories.First().id));
                Assert.IsTrue(!string.IsNullOrEmpty(categories.First().name));
                Assert.IsTrue(!string.IsNullOrEmpty(categories.First().slug));

                CategoryModel category = await connection.Category.GetCategoryBySlug(categories.First().slug);
                Assert.IsNotNull(category);
                Assert.IsTrue(string.Equals(categories.First().id, category.id));
            });
        }
    }
}
