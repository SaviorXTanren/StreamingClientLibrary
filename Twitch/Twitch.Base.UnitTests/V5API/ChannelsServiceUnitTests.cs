using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Twitch.Base.Models.V5.Channel;
using Twitch.Base.Models.V5.Communities;
using Twitch.Base.Models.V5.Teams;
using Twitch.Base.Models.V5.Users;
using Twitch.Base.Models.V5.Videos;

namespace Twitch.Base.UnitTests.V5API
{
    [TestClass]
    public class ChannelsServiceUnitTests : UnitTestBase
    {
        public static async Task<ChannelModel> GetCurrentChannel(TwitchConnection connection)
        {
            ChannelModel channel = await connection.V5API.Channels.GetCurrentChannel();

            Assert.IsNotNull(channel);
            Assert.IsNotNull(channel.id);

            return channel;
        }

        [TestMethod]
        public void GetCurrentChannel()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetCurrentChannel(connection);
            });
        }

        [TestMethod]
        public void GetChannel()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetCurrentChannel(connection);

                ChannelModel result = await connection.V5API.Channels.GetChannel(channel);

                Assert.IsNotNull(result);
                Assert.IsNotNull(result.id);
                Assert.AreEqual(channel.id, result.id);
            });
        }

        [TestMethod]
        public void GetChannelByID()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetCurrentChannel(connection);

                ChannelModel result = await connection.V5API.Channels.GetChannelByID(channel.id);

                Assert.IsNotNull(result);
                Assert.IsNotNull(result.id);
                Assert.AreEqual(channel.id, result.id);
            });
        }

        [TestMethod]
        public void UpdateChannel()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetCurrentChannel(connection);

                ChannelUpdateModel update = new ChannelUpdateModel() { status = "Test Status - " + DateTimeOffset.Now.ToString() };

                ChannelModel result = await connection.V5API.Channels.UpdateChannel(channel, update);

                Assert.IsNotNull(result);
                Assert.IsNotNull(result.id);
                Assert.AreEqual(result.status, update.status);
            });
        }

        [TestMethod]
        public void GetChannelEditors()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetCurrentChannel(connection);

                IEnumerable<UserModel> results = await connection.V5API.Channels.GetChannelEditors(channel);

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() > 0);
            });
        }

        [TestMethod]
        public void GetChannelFollowers()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetCurrentChannel(connection);

                IEnumerable<UserFollowModel> results = await connection.V5API.Channels.GetChannelFollowers(channel);

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() > 0);
            });
        }

        [TestMethod]
        public void GetChannelTeams()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetCurrentChannel(connection);

                IEnumerable<TeamModel> results = await connection.V5API.Channels.GetChannelTeams(channel);

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() > 0);
            });
        }

        [TestMethod]
        public void GetChannelSubscribers()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetCurrentChannel(connection);

                IEnumerable<UserSubscriptionModel> results = await connection.V5API.Channels.GetChannelSubscribers(channel);

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() > 0);
            });
        }

        [TestMethod]
        public void GetChannelUserSubscription()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetCurrentChannel(connection);

                UserModel user = await connection.V5API.Users.GetCurrentUser();

                UserSubscriptionModel result = await connection.V5API.Channels.GetChannelUserSubscription(channel, user);

                Assert.IsNotNull(result);
            });
        }

        [TestMethod]
        public void GetChannelVideos()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetCurrentChannel(connection);

                IEnumerable<VideoModel> results = await connection.V5API.Channels.GetChannelVideos(channel);

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() > 0);
            });
        }

        [TestMethod]
        public void ResetChannelStreamKey()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetCurrentChannel(connection);

                ChannelModel result = await connection.V5API.Channels.ResetChannelStreamKey(channel);

                Assert.IsNotNull(result);
                Assert.IsNotNull(result.id);
                Assert.AreEqual(channel.id, result.id);
            });
        }

        [TestMethod]
        public void GetChannelCommunities()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetCurrentChannel(connection);

                IEnumerable<CommunityModel> results = await connection.V5API.Channels.GetChannelCommunities(channel);

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() > 0);
            });
        }

        [TestMethod]
        public void DeleteAndSetChannelCommunities()
        {
            TestWrapper(async (TwitchConnection connection) =>
            {
                ChannelModel channel = await ChannelsServiceUnitTests.GetCurrentChannel(connection);

                IEnumerable<CommunityModel> communities = await connection.V5API.Channels.GetChannelCommunities(channel);

                Assert.IsNotNull(communities);
                Assert.IsTrue(communities.Count() > 0);

                await connection.V5API.Channels.DeleteChannelCommunities(channel);

                IEnumerable<CommunityModel> results = await connection.V5API.Channels.GetChannelCommunities(channel);

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() == 0);

                await connection.V5API.Channels.SetChannelCommunities(channel, results);

                results = await connection.V5API.Channels.GetChannelCommunities(channel);

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() >0);
            });
        }
    }
}
