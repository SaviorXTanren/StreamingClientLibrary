using Newtonsoft.Json.Linq;
using StreamingClient.Base.Util;
using StreamingClient.Base.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Twitch.Base.Models.NewAPI.EventSub;

namespace Twitch.Base.Services.NewAPI
{
    /// <summary>
    /// The APIs for EventSub-based services.
    /// </summary>
    public class EventSubService : NewTwitchAPIServiceBase
    {
        /// <summary>
        /// The header for the message id.
        /// </summary>
        public const string TwitchEventSubMessageIdHeader = "Twitch-Eventsub-Message-Id";
        /// <summary>
        /// The header for the message timestamp.
        /// </summary>
        public const string TwitchEventSubMessageTimestampHeader = "Twitch-Eventsub-Message-Timestamp";
        /// <summary>
        /// The header for the message signature.
        /// </summary>
        public const string TwitchEventSubMessageSignatureHeader = "Twitch-Eventsub-Message-Signature";

        /// <summary>
        /// Creates an instance of the EventSubService
        /// </summary>
        /// <param name="connection">The Twitch connection to use</param>
        public EventSubService(TwitchConnection connection) : base(connection) { }

        /// <summary>
        /// Gets the subscriptions of the app identified by a Bearer token.  Requires App Token.
        /// </summary>
        /// <param name="maxResults">Maximum results to return. Must be a value from 10 to 100.</param>
        /// <returns>The subscribed webhooks for the app identified by a Bearer token in the Twitch connection</returns>
        public async Task<IEnumerable<EventSubSubscriptionModel>> GetSubscriptions(int maxResults=100)
        {
            Validator.Validate(maxResults >= 10, "maxResults must be greater than or equal to 10.");
            Validator.Validate(maxResults <= 100, "maxResults must be less than or equal to 100.");
            return await this.GetPagedDataResultAsync<EventSubSubscriptionModel>("eventsub/subscriptions", maxResults);
        }

        /// <summary>
        /// Deletes a subscription of the app identified by a Bearer token.  Requires App Token.
        /// </summary>
        /// <param name="subscriptionId">The ID of the subscription to delete</param>
        /// <returns>The subscribed webhooks for the app identified by a Bearer token in the Twitch connection</returns>
        public async Task DeleteSubscription(string subscriptionId)
        {
            await this.DeleteAsync($"eventsub/subscriptions?id={subscriptionId}");
        }

        /// <summary>
        /// Creates a subscription for the app identified by a Bearer token.  Requires App Token.
        /// </summary>
        /// <returns>The subscribed webhooks for the app identified by a Bearer token in the Twitch connection</returns>
        public async Task<EventSubSubscriptionModel> CreateSubscription(EventSubTypesEnum type, string conditionName, string conditionValue, string callback, string secret)
        {
            JObject jobj = new JObject();
            jobj["type"] = EnumHelper.GetEnumName(type);
            jobj["version"] = "1";

            jobj["condition"] = new JObject();
            jobj["condition"][conditionName] = conditionValue;

            jobj["transport"] = new JObject();
            jobj["transport"]["method"] = "webhook";
            jobj["transport"]["callback"] = callback;
            jobj["transport"]["secret"] = secret;

            // TODO: Consider getting other top level fields
            //      "total": 1,
            //      "total_cost": 1,
            //      "max_total_cost": 10000,
            //      "limit": 10000
            var subs = await this.PostDataResultAsync<EventSubSubscriptionModel>("eventsub/subscriptions", AdvancedHttpClient.CreateContentFromObject(jobj));
            return subs?.FirstOrDefault();
        }

        /// <summary>
        /// Used to verify the signature from a twitch event sub webhook call.
        /// </summary>
        /// <param name="twitchEventSubMessageId">This comes from the Twitch-Eventsub-Message-Id header.</param>
        /// <param name="twitchEventSubMessageTimestamp">This comes from the Twitch-Eventsub-Message-Timestamp header.</param>
        /// <param name="body">This is the body of the message.</param>
        /// <param name="twitchEventSubMessageSignature">This comes from the Twitch-Eventsub-Message-Signature header.</param>
        /// <param name="secret">This is the secret you provided when you created the webhook.</param>
        /// <returns></returns>
        public static bool VerifySignature(string twitchEventSubMessageId, string twitchEventSubMessageTimestamp, string body, string twitchEventSubMessageSignature, string secret)
        {
            if (string.IsNullOrEmpty(twitchEventSubMessageId) ||
                string.IsNullOrEmpty(twitchEventSubMessageTimestamp) ||
                string.IsNullOrEmpty(body) ||
                string.IsNullOrEmpty(twitchEventSubMessageSignature))
            {
                return false;
            }

            var sha256 = new HMACSHA256(Encoding.UTF8.GetBytes(secret));

            var data = twitchEventSubMessageId + twitchEventSubMessageTimestamp + body;
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
            var stringHash = BitConverter.ToString(hash).Replace("-", "");

            var calculatedSignature = $"sha256={stringHash}";
            return string.Equals(calculatedSignature, twitchEventSubMessageSignature, StringComparison.OrdinalIgnoreCase);
        }
    }
}
