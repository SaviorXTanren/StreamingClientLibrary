using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using StreamingClient.Base.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace YouTube.Base.Services
{
    /// <summary>
    /// The APIs for Search-based services.
    /// </summary>
    public class SearchService : YouTubeServiceBase
    {
        /// <summary>
        /// Creates an instance of the SearchService.
        /// </summary>
        /// <param name="connection">The YouTube connection to use</param>
        public SearchService(YouTubeConnection connection) : base(connection) { }

        /// <summary>
        /// Gets the videos for the currently authenticated account.
        /// </summary>
        /// <param name="maxResults">The maximum results to return</param>
        /// <returns>The list of videos</returns>
        public async Task<IEnumerable<SearchResult>> GetMyVideos(int maxResults = 1)
        {
            return await this.Search(myVideos: true, keyword: null, maxResults: maxResults);
        }

        /// <summary>
        /// Searchs for videos with the specified keyword search.
        /// </summary>
        /// <param name="keyword">The keyword to search for</param>
        /// <param name="maxResults">The maximum results to return</param>
        /// <returns>The list of videos</returns>
        public async Task<IEnumerable<SearchResult>> SearchByKeyword(string keyword, int maxResults = 1)
        {
            Validator.ValidateString(keyword, "keyword");
            return await this.Search(myVideos: false, keyword: keyword, maxResults: maxResults);
        }

        internal async Task<IEnumerable<SearchResult>> Search(bool myVideos = false, string keyword = null, int maxResults = 1)
        {
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
                        search.Order = SearchResource.ListRequest.OrderEnum.Date;
                    }
                    else if (!string.IsNullOrEmpty(keyword))
                    {
                        search.Q = keyword;
                    }
                    search.MaxResults = Math.Min(maxResults, 50);
                    search.Type = "video";
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
