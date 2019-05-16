using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using StreamingClient.Base.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YouTubeLive.Base.Services
{
    /// <summary>
    /// The APIs for Videos-based services.
    /// </summary>
    public class VideosService : YouTubeLiveServiceBase
    {
        /// <summary>
        /// Creates an instance of the VideosService.
        /// </summary>
        /// <param name="connection">The YouTube Live connection to use</param>
        public VideosService(YouTubeLiveConnection connection) : base(connection) { }

        /// <summary>
        /// Gets the videos for the currently authenticated account.
        /// </summary>
        /// <param name="maxResults">The maximum results to return</param>
        /// <returns>The list of videos</returns>
        public async Task<IEnumerable<Video>> GetMyVideos(int maxResults = 1)
        {
            return await this.YouTubeServiceWrapper(async () =>
            {
                IEnumerable<SearchResult> searchResults = await this.connection.Search.GetMyVideos(maxResults);
                if (searchResults.Count() > 0)
                {
                    return await this.GetVideosByID(searchResults.Select(s => s.Id.VideoId), isOwned: true);
                }
                return new List<Video>();
            });
        }

        /// <summary>
        /// Gets the videos for the specified channel.
        /// </summary>
        /// <param name="channel">The channel to get videos for</param>
        /// <param name="maxResults">The maximum results to return</param>
        /// <returns>The list of videos</returns>
        public async Task<IEnumerable<Video>> GetVideosForChannel(Channel channel, int maxResults = 1)
        {
            return await this.YouTubeServiceWrapper(async () =>
            {
                IEnumerable<PlaylistItem> playlistItems = await this.connection.Playlists.GetPlaylistItems(channel.ContentDetails.RelatedPlaylists.Uploads);
                if (playlistItems.Count() > 0)
                {
                    return await this.GetVideosByID(playlistItems.Select(p => p.ContentDetails.VideoId), isOwned: false);
                }
                return new List<Video>();
            });
        }

        /// <summary>
        /// Gets the video for the specified ID
        /// </summary>
        /// <param name="id">The ID of the videos</param>
        /// <param name="isOwned">Indicates whether the video is owned by the currently authenticated user and includes additional details if so</param>
        /// <returns>The video information</returns>
        public async Task<Video> GetVideoByID(string id, bool isOwned = false)
        {
            Validator.ValidateList(id, "id");
            IEnumerable<Video> results = await this.GetVideosByID(new List<string>() { id }, isOwned);
            return (results != null && results.Count() > 0) ? results.First() : null;
        }

        /// <summary>
        /// Gets the videos for the specified IDs
        /// </summary>
        /// <param name="ids">The IDs of the videos</param>
        /// <param name="isOwned">Indicates whether the video is owned by the currently authenticated user and includes additional details if so</param>
        /// <returns>The video information</returns>
        public async Task<IEnumerable<Video>> GetVideosByID(IEnumerable<string> ids, bool isOwned = false)
        {
            Validator.ValidateList(ids, "ids");
            return await this.YouTubeServiceWrapper(async () =>
            {
                List<Video> results = new List<Video>();
                List<string> searchIDs = new List<string>(ids);
                string pageToken = null;
                do
                {
                    int searchAmount = Math.Min(searchIDs.Count, 50);

                    string parts = "snippet,contentDetails,statistics,liveStreamingDetails,recordingDetails,status,topicDetails";
                    if (isOwned)
                    {
                        parts += ",fileDetails,processingDetails";
                    }
                    VideosResource.ListRequest request = this.connection.GoogleYouTubeService.Videos.List(parts);
                    request.MaxResults = searchAmount;
                    request.Id = string.Join(",", searchIDs.Take(searchAmount));
                    request.PageToken = pageToken;

                    VideoListResponse response = await request.ExecuteAsync();
                    results.AddRange(response.Items);
                    searchIDs = new List<string>(searchIDs.Skip(searchAmount));
                    pageToken = response.NextPageToken;

                } while (searchIDs.Count > 0 && !string.IsNullOrEmpty(pageToken));
                return results;
            });
        }
    }
}
