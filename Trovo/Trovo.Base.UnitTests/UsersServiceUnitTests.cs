using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trovo.Base.Models.Users;

namespace Trovo.Base.UnitTests
{
    [TestClass]
    public class UsersServiceUnitTests : UnitTestBase
    {
        [TestMethod]
        public void GetCurrentUser()
        {
            TestWrapper(async (TrovoConnection connection) =>
            {
                PrivateUserModel user = await connection.Users.GetCurrentUser();

                Assert.IsNotNull(user);
                Assert.IsTrue(!string.IsNullOrEmpty(user.userId));
                Assert.IsTrue(!string.IsNullOrEmpty(user.userName));
            });
        }

        [TestMethod]
        public void GetUser()
        {
            TestWrapper(async (TrovoConnection connection) =>
            {
                PrivateUserModel privateUser = await connection.Users.GetCurrentUser();

                Assert.IsNotNull(privateUser);
                Assert.IsTrue(!string.IsNullOrEmpty(privateUser.userId));
                Assert.IsTrue(!string.IsNullOrEmpty(privateUser.userName));

                UserModel user = await connection.Users.GetUser(privateUser.userName);

                Assert.IsNotNull(user);
                Assert.IsTrue(!string.IsNullOrEmpty(user.user_id));
                Assert.IsTrue(!string.IsNullOrEmpty(user.username));
            });
        }
    }
}
