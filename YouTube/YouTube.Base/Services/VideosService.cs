﻿using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using StreamingClient.Base.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YouTube.Base.Services
{
    /// <summary>
    /// The APIs for Videos-based services.
    /// </summary>
    public class VideosService : YouTubeServiceBase
    {
        /// <summary>
        /// Creates an instance of the VideosService.
        /// </summary>
        /// <param name="connection">The YouTube connection to use</param>
        public VideosService(YouTubeConnection connection) : base(connection) { }

        /// <summary>
        /// Gets the videos for the currently authenticated account.
        /// </summary>
        /// <param name="maxResults">The maximum results to return</param>
        /// <returns>The list of videos</returns>
        public async Task<IEnumerable<Video>> GetMyVideos(int maxResults = 1)
        {
            return await this.YouTubeServiceWrapper(async () =>
            {
                IEnumerable<SearchResult> searchResults = await this.SearchVideos(myVideos: true, maxResults: maxResults);
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
            Validator.ValidateVariable(channel, "channel");
            return await this.YouTubeServiceWrapper(async () =>
            {
                IEnumerable<SearchResult> searchResults = await this.SearchVideos(channelID: channel.Id, maxResults: maxResults);
                if (searchResults.Count() > 0)
                {
                    return await this.GetVideosByID(searchResults.Select(s => s.Id.VideoId), isOwned: false);
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

                    VideoListResponse response = await request.ExecuteAsync();
                    results.AddRange(response.Items);
                    searchIDs = new List<string>(searchIDs.Skip(searchAmount));

                } while (searchIDs.Count > 0);
                return results;
            });
        }

        /// <summary>
        /// Searchs for videos with the specified keyword search.
        /// </summary>
        /// <param name="keyword">The keyword to search for</param>
        /// <param name="maxResults">The maximum results to return</param>
        /// <returns>The list of videos</returns>
        public async Task<IEnumerable<SearchResult>> GetVideosByKeyword(string keyword, int maxResults = 1)
        {
            Validator.ValidateString(keyword, "keyword");
            return await this.SearchVideos(keyword: keyword, maxResults: maxResults);
        }

        /// <summary>
        /// Performs a search for youtube videos
        /// </summary>
        /// <param name="myVideos">Only get videos associated with the connected account</param>
        /// <param name="channelID">The specific ID of the channel to get videos for</param>
        /// <param name="keyword">Keywords to search for in the video</param>
        /// <param name="liveType">The live video type to search for</param>
        /// <param name="maxResults">The maximum results to return</param>
        /// <returns>The list of videos</returns>
        public async Task<IEnumerable<SearchResult>> SearchVideos(bool myVideos = false, string channelID = null, string keyword = null, SearchResource.ListRequest.EventTypeEnum liveType = SearchResource.ListRequest.EventTypeEnum.None, int maxResults = 1)
        {
            if (myVideos && !string.IsNullOrEmpty(channelID))
            {
                Validator.Validate(false, "Only myVideos or channelID can be set");
            }

            return await this.YouTubeServiceWrapper(async () =>
            {
                List<SearchResult> results = new List<SearchResult>();
                string pageToken = null;
                do
                {
                    SearchResource.ListRequest search = this.connection.GoogleYouTubeService.Search.List("snippet");
                    if (myVideos)
                    {
                        search.ForMine = true;
                    }
                    else if (!string.IsNullOrEmpty(channelID))
                    {
                        search.ChannelId = channelID;
                    }

                    if (!string.IsNullOrEmpty(keyword))
                    {
                        search.Q = keyword;
                    }

                    if (liveType != SearchResource.ListRequest.EventTypeEnum.None)
                    {
                        search.EventType = liveType;
                    }

                    search.Type = "video";
                    search.Order = SearchResource.ListRequest.OrderEnum.Date;
                    search.MaxResults = Math.Min(maxResults, 50);
                    search.PageToken = pageToken;

                    SearchListResponse response = await search.ExecuteAsync();
                    results.AddRange(response.Items);
                    maxResults -= response.Items.Count;
                    pageToken = response.NextPageToken;

                } while (maxResults > 0 && !string.IsNullOrEmpty(pageToken));
                return results;
            });
        }
    }
}
