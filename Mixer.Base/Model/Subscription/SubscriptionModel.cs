using System;

namespace Mixer.Base.Model.Subscription
{
    public class SubscriptionModel : TimeStampedModel
    {
        public uint id { get; set; }
        public uint? resourceId { get; set; }
        public string resourceType { get; set; }
        public string status { get; set; }
        public bool? cancelled { get; set; }
        public DateTimeOffset expiresAt { get; set; }
    }
}
