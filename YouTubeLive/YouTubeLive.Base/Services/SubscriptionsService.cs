using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using StreamingClient.Base.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeLive.Base.Services
{
    /// <summary>
    /// The APIs for Subscription-based services.
    /// </summary>
    public class SubscriptionsService : YouTubeLiveServiceBase
    {
        /// <summary>
        /// Creates an instance of the SubscriptionsService.
        /// </summary>
        /// <param name="connection">The YouTube Live connection to use</param>
        public SubscriptionsService(YouTubeLiveConnection connection) : base(connection) { }

        /// <summary>
        /// Gets the subscriptions for the currently authenticated account.
        /// </summary>
        /// <param name="maxResults">The maximum results to return</param>
        /// <returns>The list of subscriptions</returns>
        public async Task<IEnumerable<Subscription>> GetMySubscriptions(int maxResults = 1)
        {
            return await this.GetSubscriptions(mySubscriptions: true, maxResults: maxResults);
        }

        /// <summary>
        /// Gets the channels that are subscribing to the currently authenticated account.
        /// </summary>
        /// <param name="maxResults">The maximum results to return</param>
        /// <returns>The list of subscriptions</returns>
        public async Task<IEnumerable<Subscription>> GetMySubscribers(int maxResults = 1)
        {
            return await this.GetSubscriptions(mySubscribers: true, maxResults: maxResults);
        }

        /// <summary>
        /// Gets the channels that have recently subscribed to the currently authenticated account.
        /// </summary>
        /// <param name="maxResults">The maximum results to return</param>
        /// <returns>The list of subscriptions</returns>
        public async Task<IEnumerable<Subscription>> GetMyRecentSubscribers(int maxResults = 1)
        {
            return await this.GetSubscriptions(myRecentSubscribers: true, maxResults: maxResults);
        }

        internal async Task<IEnumerable<Subscription>> GetSubscriptions(bool mySubscriptions = false, bool myRecentSubscribers = false, bool mySubscribers = false, int maxResults = 1)
        {
            return await this.YouTubeServiceWrapper(async () =>
            {
                List<Subscription> results = new List<Subscription>();
                string pageToken = null;
                do
                {
                    SubscriptionsResource.ListRequest request = this.connection.GoogleYouTubeService.Subscriptions.List("snippet,contentDetails");
                    if (mySubscriptions)
                    {
                        request.Mine = true;
                    }
                    else if (myRecentSubscribers)
                    {
                        request.MyRecentSubscribers = myRecentSubscribers;
                    }
                    else if (mySubscribers)
                    {
                        request.MySubscribers = mySubscribers;
                    }
                    request.MaxResults = Math.Min(maxResults, 50);
                    request.PageToken = pageToken;

                    SubscriptionListResponse response = await request.ExecuteAsync();
                    results.AddRange(response.Items);
                    maxResults -= response.Items.Count;
                    pageToken = response.NextPageToken;

                } while (maxResults > 0 && !string.IsNullOrEmpty(pageToken));
                return results;
            });
        }
    }
}
