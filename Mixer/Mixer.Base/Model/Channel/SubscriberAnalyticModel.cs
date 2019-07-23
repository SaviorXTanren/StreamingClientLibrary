namespace Mixer.Base.Model.Channel
{
    /// <summary>
    /// A channel metric that shows when a subscription was made to a channel.
    /// </summary>
    public class SubscriberAnalyticModel : ChannelAnalyticModel
    {
        /// <summary>
        /// The total number of subscribers the channel now has.
        /// </summary>
        public uint total { get; set; }
        /// <summary>
        /// Increment that the total changed as a result of this event.
        /// </summary>
        public int delta { get; set; }
        /// <summary>
        /// The ID of the user who subscribed, may not be present in aggregations.
        /// </summary>
        public uint user { get; set; }
    }
}
