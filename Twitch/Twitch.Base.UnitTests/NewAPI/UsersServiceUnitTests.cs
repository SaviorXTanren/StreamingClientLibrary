using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Twitch.Base.Models.NewAPI.Users;

namespace Twitch.Base.UnitTests.NewAPI
{
    [TestClass]
    public class UsersServiceUnitTests : UnitTestBase
    {
        public static async Task<UserModel> GetCurrentUser(TwitchConnection connection)
        {
            UserModel user = await connection.NewAPI.Users.GetCurrentUser();

            Assert.IsNotNull(user);
            Assert.IsNotNull(user.id);

            return user;
        }

        [TestMethod]
        public void GetCurrentUser()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                UserModel user = await UsersServiceUnitTests.GetCurrentUser(connection);
            });
        }

        [TestMethod]
        public void GetUser()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                UserModel user = await UsersServiceUnitTests.GetCurrentUser(connection);

                UserModel result = await connection.NewAPI.Users.GetUser(user);

                Assert.IsNotNull(result);
                Assert.IsNotNull(result.id);
                Assert.AreEqual(user.id, result.id);
            });
        }

        [TestMethod]
        public void GetUserByID()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                UserModel user = await UsersServiceUnitTests.GetCurrentUser(connection);

                UserModel result = await connection.NewAPI.Users.GetUserByID(user.id);

                Assert.IsNotNull(result);
                Assert.IsNotNull(result.id);
                Assert.AreEqual(user.id, result.id);
            });
        }

        [TestMethod]
        public void GetUserByLogin()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                UserModel user = await UsersServiceUnitTests.GetCurrentUser(connection);

                UserModel result = await connection.NewAPI.Users.GetUserByLogin("SaviorXTanren");

                Assert.IsNotNull(result);
                Assert.IsNotNull(result.id);
                Assert.AreEqual(user.id, result.id);
            });
        }

        [TestMethod]
        public void UpdateCurrentUserDescription()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                UserModel user = await UsersServiceUnitTests.GetCurrentUser(connection);

                string newDescription = "This is a test description update at " + DateTimeOffset.Now.ToString();
                UserModel result = await connection.NewAPI.Users.UpdateCurrentUserDescription(newDescription);

                Assert.IsNotNull(result);
                Assert.IsNotNull(result.id);
                Assert.AreEqual(user.id, result.id);
                Assert.AreEqual(result.description, newDescription);
            });
        }
    }
}
