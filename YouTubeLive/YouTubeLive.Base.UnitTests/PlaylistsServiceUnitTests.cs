using Google.Apis.YouTube.v3.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace YouTubeLive.Base.UnitTests
{
    [TestClass]
    public class PlaylistsServiceUnitTests : UnitTestBase
    {
        [TestMethod]
        public void GetMyPlaylists()
        {
            TestWrapper(async (YouTubeLiveConnection connection) =>
            {
                IEnumerable<Playlist> results = await connection.Playlists.GetMyPlaylists(maxResults: 10);

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() > 0);
            });
        }

        [TestMethod]
        public void GetPlaylistsForChannel()
        {
            TestWrapper(async (YouTubeLiveConnection connection) =>
            {
                Channel channel = await connection.Channels.GetChannelByID("UC_x5XG1OV2P6uZZ5FSM9Ttw");

                Assert.IsNotNull(channel);
                Assert.IsNotNull(channel.Id);

                IEnumerable<Playlist> results = await connection.Playlists.GetPlaylistsForChannel(channel, maxResults: 10);

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() > 0);
            });
        }

        [TestMethod]
        public void GetPlaylistItems()
        {
            TestWrapper(async (YouTubeLiveConnection connection) =>
            {
                Channel channel = await connection.Channels.GetChannelByID("UC_x5XG1OV2P6uZZ5FSM9Ttw");

                Assert.IsNotNull(channel);
                Assert.IsNotNull(channel.Id);

                IEnumerable<Playlist> playlists = await connection.Playlists.GetPlaylistsForChannel(channel, maxResults: 10);

                Assert.IsNotNull(playlists);
                Assert.IsTrue(playlists.Count() > 0);

                IEnumerable<PlaylistItem> results = await connection.Playlists.GetPlaylistItems(playlists.First(), maxResults: 10);

                Assert.IsNotNull(results);
                Assert.IsTrue(results.Count() > 0);
            });
        }

        [TestMethod]
        public void AddVideoToPlaylist()
        {
            TestWrapper(async (YouTubeLiveConnection connection) =>
            {
                Channel channel = await connection.Channels.GetChannelByID("UC_x5XG1OV2P6uZZ5FSM9Ttw");

                Assert.IsNotNull(channel);
                Assert.IsNotNull(channel.Id);

                IEnumerable<Video> videos = await connection.Videos.GetVideosForChannel(channel, maxResults: 10);

                Assert.IsNotNull(videos);
                Assert.IsTrue(videos.Count() > 0);

                IEnumerable<Playlist> playlists = await connection.Playlists.GetMyPlaylists();

                Assert.IsNotNull(playlists);
                Assert.IsTrue(playlists.Count() > 0);

                PlaylistItem result = await connection.Playlists.AddVideoToPlaylist(playlists.First(), videos.First());

                Assert.IsNotNull(result);
                Assert.IsNotNull(result.Id);
            });
        }
    }
}
