using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;

namespace Twitch.Webhooks.Base.Models
{
    /// <summary>
    /// The verification response for a web hook.
    /// </summary>
    public class WebhookSubscriptionVerificationModel
    {
        private const string SubscribeMode = "subscribe";
        private const string UnsubscribeMode = "unsubscribe";

        /// <summary>
        /// Type of request. Valid values: subscribe, unsubscribe.
        /// </summary>
        public string mode { get; set; }

        /// <summary>
        /// URL for the topic to subscribe to or unsubscribe from. topic maps to a new Twitch API endpoint.
        /// </summary>
        public string topic { get; set; }

        /// <summary>
        /// The challenge to be echoed back to the servier.
        /// </summary>
        public string challenge { get; set; }

        /// <summary>
        /// Number of seconds until the subscription expires. Default: 0. Maximum: 864000. Should be specified to a value greater than 0 otherwise subscriptions will expire before any useful notifications can be sent.
        /// </summary>
        public int lease_seconds { get; set; }

        /// <summary>
        /// The reason why the subscription was denied.
        /// </summary>
        public string reason { get; set; }

        /// <summary>
        /// Creates a new instance of the WebhookSubscriptionVerificationModel class.
        /// </summary>
        public WebhookSubscriptionVerificationModel() { }

        /// <summary>
        /// Creates a new instance of the WebhookSubscriptionVerificationModel class.
        /// <param name="query">The Http query parameters</param>
        /// </summary>
        public WebhookSubscriptionVerificationModel(IQueryCollection query)
        {
            if (query.TryGetValue("hub.mode", out StringValues mode)) { this.mode = mode; }
            if (query.TryGetValue("hub.topic", out StringValues topic)) { this.topic = topic; }
            if (query.TryGetValue("hub.challenge", out StringValues challenge)) { this.challenge = challenge; }
            if (query.TryGetValue("hub.lease_seconds", out StringValues lease) && int.TryParse(lease, out int leaseSeconds)) { this.lease_seconds = leaseSeconds; }
            if (query.TryGetValue("hub.reason", out StringValues reason)) { this.reason = reason; }
        }

        /// <summary>
        /// Whether the event is a subscribe.
        /// </summary>
        public bool IsSubscribe { get { return SubscribeMode.Equals(this.mode, StringComparison.InvariantCultureIgnoreCase); } }

        /// <summary>
        /// Whether the event is an unsubscribe.
        /// </summary>
        public bool IsUnsubscribe { get { return UnsubscribeMode.Equals(this.mode, StringComparison.InvariantCultureIgnoreCase); } }

        /// <summary>
        /// Whether the subscription was successful.
        /// </summary>
        public bool IsSuccessful { get { return !string.IsNullOrEmpty(this.challenge); } }
    }
}
