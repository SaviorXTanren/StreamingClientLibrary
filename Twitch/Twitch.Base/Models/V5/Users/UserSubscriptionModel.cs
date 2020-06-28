using Newtonsoft.Json;
using Twitch.Base.Models.V5.Channel;

namespace Twitch.Base.Models.V5.Users
{
    /// <summary>
    /// Information about a user subscription to a channel.
    /// </summary>
    public class UserSubscriptionModel
    {
        /// <summary>
        /// The ID of the user.
        /// </summary>
        [JsonProperty("_id")]
        public string id { get; set; }
        /// <summary>
        /// The plan type of the subscription.
        /// </summary>
        public string sub_plan { get; set; }
        /// <summary>
        /// The name of the subscription plan.
        /// </summary>
        public string sub_plan_name { get; set; }
        /// <summary>
        /// The channel the user is subscribed to.
        /// </summary>
        public ChannelModel channel { get; set; }
        /// <summary>
        /// The user who is subscribed.
        /// </summary>
        public UserModel user { get; set; }
        /// <summary>
        /// The date of the subscription creation.
        /// </summary>
        public string created_at { get; set; }
    }
}
