using Glimesh.Base.Models.Channels;
using Glimesh.Base.Models.Users;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Glimesh.Base.UnitTests
{
    [TestClass]
    public class ChannelServiceUnitTests : UnitTestBase
    {
        [TestMethod]
        public void GetAllChannels()
        {
            TestWrapper(async (GlimeshConnection connection) =>
            {
                IEnumerable<ChannelModel> channels = await connection.Channel.GetAllChannels();
                Assert.IsNotNull(channels);
                Assert.IsTrue(channels.Count() > 0);
                Assert.IsNotNull(channels.First());
                Assert.IsTrue(!string.IsNullOrEmpty(channels.First().id));
                Assert.IsTrue(!string.IsNullOrEmpty(channels.First().status));
            });
        }

        [TestMethod]
        public void GetChannelByID()
        {
            TestWrapper(async (GlimeshConnection connection) =>
            {
                UserModel currentUser = await connection.Users.GetCurrentUser();
                Assert.IsNotNull(currentUser);
                Assert.IsTrue(!string.IsNullOrEmpty(currentUser.id));
                Assert.IsTrue(!string.IsNullOrEmpty(currentUser.username));

                ChannelModel channel = await connection.Channel.GetChannelByID(currentUser.id);
                Assert.IsNotNull(channel);
                Assert.IsTrue(!string.IsNullOrEmpty(channel.id));
                Assert.IsTrue(!string.IsNullOrEmpty(channel.status));

                Assert.IsTrue(string.Equals(currentUser.id, channel.id));
                Assert.IsTrue(string.Equals(currentUser.username, channel.streamer.id));
            });
        }

        [TestMethod]
        public void GetChannelByName()
        {
            TestWrapper(async (GlimeshConnection connection) =>
            {
                UserModel currentUser = await connection.Users.GetCurrentUser();
                Assert.IsNotNull(currentUser);
                Assert.IsTrue(!string.IsNullOrEmpty(currentUser.id));
                Assert.IsTrue(!string.IsNullOrEmpty(currentUser.username));

                ChannelModel channel = await connection.Channel.GetChannelByName(currentUser.username);
                Assert.IsNotNull(channel);
                Assert.IsTrue(!string.IsNullOrEmpty(channel.id));
                Assert.IsTrue(!string.IsNullOrEmpty(channel.status));

                Assert.IsTrue(string.Equals(currentUser.id, channel.id));
                Assert.IsTrue(string.Equals(currentUser.username, channel.streamer.id));
            });
        }
    }
}
