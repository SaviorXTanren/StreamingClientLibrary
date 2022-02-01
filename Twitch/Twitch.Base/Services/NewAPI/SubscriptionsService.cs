using StreamingClient.Base.Util;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Twitch.Base.Models.NewAPI.Subscriptions;
using Twitch.Base.Models.NewAPI.Users;

namespace Twitch.Base.Services.NewAPI
{
    /// <summary>
    /// The APIs for Subscription-based services.
    /// </summary>
    public class SubscriptionsService : NewTwitchAPIServiceBase
    {
        /// <summary>
        /// Creates an instance of the SubscriptionsService.
        /// </summary>
        /// <param name="connection">The Twitch connection to use</param>
        public SubscriptionsService(TwitchConnection connection) : base(connection) { }

        /// <summary>
        /// Gets the subscriptions for a broadcaster
        /// </summary>
        /// <param name="broadcaster">The broadcaster to get subscriptions for</param>
        /// <param name="maxResults">The maximum number of results. Will be either that amount or slightly more</param>
        /// <returns>The broadcaster's subscriptions</returns>
        public async Task<IEnumerable<SubscriptionModel>> GetBroadcasterSubscriptions(UserModel broadcaster, int maxResults = 1)
        {
            Validator.ValidateVariable(broadcaster, "broadcaster");
            return await this.GetPagedDataResultAsync<SubscriptionModel>("subscriptions?broadcaster_id=" + broadcaster.id, maxResults);
        }

        /// <summary>
        /// Gets the subscriptions for a broadcaster for the specified user IDs.
        /// </summary>
        /// <param name="broadcaster">The broadcaster to get subscriptions for</param>
        /// <param name="userIDs">The user IDs to get subscriptions for</param>
        /// <returns>The broadcaster's subscriptions</returns>
        public async Task<IEnumerable<SubscriptionModel>> GetBroadcasterSubscriptions(UserModel broadcaster, IEnumerable<string> userIDs)
        {
            Validator.ValidateVariable(broadcaster, "broadcaster");
            Validator.ValidateList(userIDs, "userIDs");
            return await this.GetPagedDataResultAsync<SubscriptionModel>("subscriptions?broadcaster_id=" + broadcaster.id + "&user_id=" + string.Join("&user_id=", userIDs), userIDs.Count());
        }

        /// <summary>
        /// Gets the total number of subscribers for a channel.
        /// </summary>
        /// <param name="broadcaster">The channel to get the total number of subscribers for</param>
        /// <returns>The total number of subscribers for the channel</returns>
        public async Task<long> GetBroadcasterSubscriptionsCount(UserModel broadcaster)
        {
            Validator.ValidateVariable(broadcaster, "broadcaster");
            return await this.GetPagedResultTotalCountAsync("subscriptions?broadcaster_id=" +broadcaster.id);
        }

        /// <summary>
        /// Checks of the specified user is subscribed to the specified broadcaster.
        /// </summary>
        /// <param name="broadcaster">The broadcaster to get subscriptions for</param>
        /// <param name="user">The user to get subscription for</param>
        /// <returns>The user's subscriptions</returns>
        public async Task<SubscriptionModel> GetUserSubscription(UserModel broadcaster, UserModel user)
        {
            Validator.ValidateVariable(broadcaster, "broadcaster");
            Validator.ValidateVariable(user, "user");
            IEnumerable<SubscriptionModel> subscriptions = await this.GetPagedDataResultAsync<SubscriptionModel>("subscriptions/user?broadcaster_id=" + broadcaster.id + "&user_id=" + user.id);
            return (subscriptions != null) ? subscriptions.FirstOrDefault() : null;
        }
    }
}
