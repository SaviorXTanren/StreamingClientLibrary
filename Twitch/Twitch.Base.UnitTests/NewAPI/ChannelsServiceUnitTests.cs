using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Twitch.Base.Models.NewAPI.Channels;
using Twitch.Base.Models.NewAPI.Users;

namespace Twitch.Base.UnitTests.NewAPI
{
    [TestClass]
    public class ChannelsServiceUnitTests : UnitTestBase
    {
        [TestMethod]
        public void GetChannelInformation()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                UserModel channel = await UsersServiceUnitTests.GetCurrentUser(connection);

                ChannelInformationModel result = await connection.NewAPI.Channels.GetChannelInformation(channel);

                Assert.IsNotNull(result);
                Assert.AreEqual(result.broadcaster_id, channel.id);
                Assert.IsTrue(result.tags != null && result.tags.Count > 0);
            });
        }

        [TestMethod]
        public void GetChannelBannedEvents()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                UserModel channel = await UsersServiceUnitTests.GetCurrentUser(connection);

                IEnumerable<ChannelBannedEventModel> results = await connection.NewAPI.Channels.GetChannelBannedEvents(channel);

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() > 0);
            });
        }

        [TestMethod]
        public void GetChannelBannedUsers()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                UserModel channel = await UsersServiceUnitTests.GetCurrentUser(connection);

                IEnumerable<ChannelBannedUserModel> results = await connection.NewAPI.Channels.GetChannelBannedUsers(channel);

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() > 0);
            });
        }

        [TestMethod]
        public void GetChannelModeratorEvents()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                UserModel channel = await UsersServiceUnitTests.GetCurrentUser(connection);

                IEnumerable<ChannelModeratorEventModel> results = await connection.NewAPI.Channels.GetChannelModeratorEvents(channel);

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() > 0);
            });
        }

        [TestMethod]
        public void GetChannelModeratorUsers()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                UserModel channel = await UsersServiceUnitTests.GetCurrentUser(connection);

                IEnumerable<ChannelModeratorUserModel> results = await connection.NewAPI.Channels.GetChannelModeratorUsers(channel);

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() > 0);
            });
        }

        [TestMethod]
        public void GetChannelEditorUsers()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                UserModel channel = await UsersServiceUnitTests.GetCurrentUser(connection);

                IEnumerable<ChannelEditorUserModel> results = await connection.NewAPI.Channels.GetChannelEditorUsers(channel);

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() > 0);
            });
        }

        [TestMethod]
        public void GetHypeTrainEvents()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                UserModel channel = await UsersServiceUnitTests.GetCurrentUser(connection);

                IEnumerable<ChannelHypeTrainModel> results = await connection.NewAPI.Channels.GetHypeTrainEvents(channel);

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() > 0);
            });
        }
    }
}
