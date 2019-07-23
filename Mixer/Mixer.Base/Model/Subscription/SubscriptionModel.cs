using System;

namespace Mixer.Base.Model.Subscription
{
    /// <summary>
    /// A subscription lists details about a user's subscription to a resource.
    /// </summary>
    public class SubscriptionModel : TimeStampedModel
    {
        /// <summary>
        /// The unique ID of the subscription.
        /// </summary>
        public uint id { get; set; }
        /// <summary>
        /// ID of the resource type this payment is for. Currently, this is only a channel ID.
        /// </summary>
        public uint? resourceId { get; set; }
        /// <summary>
        /// The resource type the resourceId points to. Currently, this is only channel.
        /// </summary>
        public string resourceType { get; set; }
        /// <summary>
        /// The status of the subscription.
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// Indicates whether the payment has been cancelled after completion.
        /// </summary>
        public bool? cancelled { get; set; }
        /// <summary>
        /// The time when the subscription expires.
        /// </summary>
        public DateTimeOffset expiresAt { get; set; }
    }
}
