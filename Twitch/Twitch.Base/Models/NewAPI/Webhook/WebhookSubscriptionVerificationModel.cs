using Newtonsoft.Json;

namespace Twitch.Base.Models.NewAPI.Webhook
{
    /// <summary>
    /// The verification sent to the web server for a web hook registration.
    /// </summary>
    public class WebhookSubscriptionVerificationModel
    {
        /// <summary>
        /// URL where notifications will be delivered.
        /// </summary>
        [JsonProperty(PropertyName = "hub.callback")]
        public string callback { get; set; }

        /// <summary>
        /// Type of request. Valid values: subscribe, unsubscribe.
        /// </summary>
        [JsonProperty(PropertyName = "hub.mode")]
        public string mode { get; set; }

        /// <summary>
        /// URL for the topic to subscribe to or unsubscribe from. topic maps to a new Twitch API endpoint.
        /// </summary>
        [JsonProperty(PropertyName = "hub.topic")]
        public string topic { get; set; }

        /// <summary>
        /// Number of seconds until the subscription expires. Default: 0. Maximum: 864000. Should be specified to a value greater than 0 otherwise subscriptions will expire before any useful notifications can be sent.
        /// </summary>
        [JsonProperty(PropertyName = "hub.lease_seconds")]
        public int lease_seconds { get; set; }
    }
}
