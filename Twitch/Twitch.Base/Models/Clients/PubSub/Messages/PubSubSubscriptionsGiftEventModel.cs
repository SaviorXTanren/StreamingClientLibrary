namespace Twitch.Base.Models.Clients.PubSub.Messages
{
    /// <summary>
    /// Information about a Subscription Gift.
    /// </summary>
    public class PubSubSubscriptionsGiftEventModel : PubSubSubscriptionsEventModel
    {
        /// <summary>
        /// The total months of the subscription (ONLY FOR SUB GIFT EVENTS).
        /// </summary>
        public int months { get; set; }
        /// <summary>
        /// The recipient ID of the gift.
        /// </summary>
        public string recipient_id { get; set; }
        /// <summary>
        /// The recipient name of the gift.
        /// </summary>
        public string recipient_user_name { get; set; }
        /// <summary>
        /// The recipient display name of the gift.
        /// </summary>
        public string recipient_display_name { get; set; }
    }
}
