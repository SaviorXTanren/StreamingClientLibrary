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
        /// Gets all subscriptions for a broadcaster
        /// </summary>
        /// <param name="broadcaster">The broadcaster to get subscriptions for</param>
        /// <param name="maxResults">The maximum number of results. Will be either that amount or slightly more</param>
        /// <returns>The broadcaster's subscriptions</returns>
        public async Task<IEnumerable<SubscriptionModel>> GetAllSubscriptions(UserModel broadcaster, int maxResults = 1)
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
        public async Task<IEnumerable<SubscriptionModel>> GetSubscriptions(UserModel broadcaster, IEnumerable<string> userIDs)
        {
            Validator.ValidateVariable(broadcaster, "broadcaster");
            Validator.ValidateList(userIDs, "userIDs");
            return await this.GetPagedDataResultAsync<SubscriptionModel>("subscriptions?broadcaster_id=" + broadcaster.id + "&user_id=" + string.Join("&user_id=", userIDs), userIDs.Count());
        }
    }
}
