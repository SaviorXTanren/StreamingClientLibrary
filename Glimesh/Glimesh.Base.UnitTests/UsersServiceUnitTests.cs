using Glimesh.Base.Models.Users;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
    }
}
