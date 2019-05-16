using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Twitch.Base.Models.NewAPI.Tags;
using Twitch.Base.Models.NewAPI.Users;

namespace Twitch.Base.UnitTests.NewAPI
{
    [TestClass]
    public class TagsServiceUnitTests : UnitTestBase
    {
        [TestMethod]
        public void GetStreamTags()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                IEnumerable<TagModel> results = await connection.NewAPI.Tags.GetStreamTags();

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() > 0);
            });
        }

        [TestMethod]
        public void GetStreamTagsByIDs()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                IEnumerable<TagModel> tags = await connection.NewAPI.Tags.GetStreamTags();

                IEnumerable<TagModel> results = await connection.NewAPI.Tags.GetStreamTagsByIDs(tags.Take(10).Select(t => t.tag_id));

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() > 0);
            });
        }

        [TestMethod]
        public void GetStreamTagsForBroadcaster()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                UserModel user = await connection.NewAPI.Users.GetUserByLogin("TwitchPresents");

                IEnumerable<TagModel> results = await connection.NewAPI.Tags.GetStreamTagsForBroadcaster(user);

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() > 0);
            });
        }

        [TestMethod]
        public void UpdateStreamTags()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                IEnumerable<TagModel> tags = await connection.NewAPI.Tags.GetStreamTags();

                UserModel user = await UsersServiceUnitTests.GetCurrentUser(connection);

                await connection.NewAPI.Tags.UpdateStreamTags(user);

                IEnumerable<TagModel> results = await connection.NewAPI.Tags.GetStreamTagsForBroadcaster(user);

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() == 0);

                await connection.NewAPI.Tags.UpdateStreamTags(user, tags.Take(3));

                results = await connection.NewAPI.Tags.GetStreamTagsForBroadcaster(user);

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() == 3);
            });
        }
    }
}
