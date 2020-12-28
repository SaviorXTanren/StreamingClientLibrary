using Glimesh.Base.Models.Users;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Glimesh.Base.UnitTests
{
    [TestClass]
    public class UsersServiceUnitTests : UnitTestBase
    {
        [TestMethod]
        public void GetCurrentUser()
        {
            TestWrapper(async (GlimeshConnection connection) =>
            {
                UserModel currentUser = await connection.Users.GetCurrentUser();
                Assert.IsNotNull(currentUser);
                Assert.IsTrue(!string.IsNullOrEmpty(currentUser.id));
                Assert.IsTrue(!string.IsNullOrEmpty(currentUser.username));
            });
        }

        [TestMethod]
        public void GetUserByID()
        {
            TestWrapper(async (GlimeshConnection connection) =>
            {
                UserModel currentUser = await connection.Users.GetCurrentUser();
                Assert.IsNotNull(currentUser);
                Assert.IsTrue(!string.IsNullOrEmpty(currentUser.id));

                UserModel user = await connection.Users.GetUserByID(currentUser.id);
                Assert.IsNotNull(user);
                Assert.IsTrue(string.Equals(user.id, currentUser.id));
            });
        }

        [TestMethod]
        public void GetUserByName()
        {
            TestWrapper(async (GlimeshConnection connection) =>
            {
                UserModel currentUser = await connection.Users.GetCurrentUser();
                Assert.IsNotNull(currentUser);
                Assert.IsTrue(!string.IsNullOrEmpty(currentUser.username));

                UserModel user = await connection.Users.GetUserByName(currentUser.username);
                Assert.IsNotNull(user);
                Assert.IsTrue(string.Equals(user.username, currentUser.username));
            });
        }

        [TestMethod]
        public void GetUsersFollowed()
        {
            TestWrapper(async (GlimeshConnection connection) =>
            {
                UserModel currentUser = await connection.Users.GetCurrentUser();
                Assert.IsNotNull(currentUser);
                Assert.IsTrue(!string.IsNullOrEmpty(currentUser.username));

                IEnumerable<UserFollowModel> follows = await connection.Users.GetUsersFollowed(currentUser.username);
                Assert.IsNotNull(follows);
                Assert.IsTrue(follows.Count() > 0);
                Assert.IsNotNull(follows.First());
                Assert.IsTrue(!string.IsNullOrEmpty(follows.First().id));
            });
        }

        [TestMethod]
        public void GetFollowingUsers()
        {
            TestWrapper(async (GlimeshConnection connection) =>
            {
                UserModel currentUser = await connection.Users.GetCurrentUser();
                Assert.IsNotNull(currentUser);
                Assert.IsTrue(!string.IsNullOrEmpty(currentUser.username));

                IEnumerable<UserFollowModel> follows = await connection.Users.GetFollowingUsers(currentUser.username);
                Assert.IsNotNull(follows);
                Assert.IsTrue(follows.Count() > 0);
                Assert.IsNotNull(follows.First());
                Assert.IsTrue(!string.IsNullOrEmpty(follows.First().id));
            });
        }

        [TestMethod]
        public void GetUsersSubscribedTo()
        {
            TestWrapper(async (GlimeshConnection connection) =>
            {
                UserModel currentUser = await connection.Users.GetCurrentUser();
                Assert.IsNotNull(currentUser);
                Assert.IsTrue(!string.IsNullOrEmpty(currentUser.username));

                IEnumerable<UserSubscriptionModel> subscriptions = await connection.Users.GetUsersSubscribedTo(currentUser.username);
                Assert.IsNotNull(subscriptions);
                Assert.IsTrue(subscriptions.Count() > 0);
                Assert.IsNotNull(subscriptions.First());
                Assert.IsTrue(!string.IsNullOrEmpty(subscriptions.First().id));
            });
        }

        [TestMethod]
        public void GetSubscribedUsers()
        {
            TestWrapper(async (GlimeshConnection connection) =>
            {
                UserModel currentUser = await connection.Users.GetCurrentUser();
                Assert.IsNotNull(currentUser);
                Assert.IsTrue(!string.IsNullOrEmpty(currentUser.username));

                IEnumerable<UserSubscriptionModel> subscriptions = await connection.Users.GetSubscribedUsers(currentUser.username);
                Assert.IsNotNull(subscriptions);
                Assert.IsTrue(subscriptions.Count() > 0);
                Assert.IsNotNull(subscriptions.First());
                Assert.IsTrue(!string.IsNullOrEmpty(subscriptions.First().id));
            });
        }
    }
}
