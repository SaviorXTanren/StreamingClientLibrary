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

                IEnumerable<UserFollowModel> follows = await connection.Users.GetUsersFollowed(currentUser);
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

                IEnumerable<UserFollowModel> follows = await connection.Users.GetFollowingUsers(currentUser);
                Assert.IsNotNull(follows);
                Assert.IsTrue(follows.Count() > 0);
                Assert.IsNotNull(follows.First());
                Assert.IsTrue(!string.IsNullOrEmpty(follows.First().id));
            });
        }

        [TestMethod]
        public void GetFollowingUser()
        {
            TestWrapper(async (GlimeshConnection connection) =>
            {
                UserModel currentUser = await connection.Users.GetCurrentUser();
                Assert.IsNotNull(currentUser);
                Assert.IsTrue(!string.IsNullOrEmpty(currentUser.username));

                IEnumerable<UserFollowModel> follows = await connection.Users.GetFollowingUsers(currentUser);
                Assert.IsNotNull(follows);
                Assert.IsTrue(follows.Count() > 0);
                Assert.IsNotNull(follows.First());
                Assert.IsTrue(!string.IsNullOrEmpty(follows.First().id));

                UserFollowModel follow = await connection.Users.GetFollowingUser(currentUser, follows.First().user);
                Assert.IsNotNull(follow);
                Assert.IsTrue(!string.IsNullOrEmpty(follow.id));
            });
        }
    }
}
