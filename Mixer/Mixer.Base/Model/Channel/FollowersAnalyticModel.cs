namespace Mixer.Base.Model.Channel
{
    /// <summary>
    /// A channel metric showing how many followers the channel had at this time and the change in that metric.
    /// </summary>
    public class FollowersAnalyticModel : ChannelAnalyticModel
    {
        /// <summary>
        /// The total number of followers the channel now has.
        /// </summary>
        public uint total { get; set; }
        /// <summary>
        /// Increment that the total changed as a result of this event.
        /// </summary>
        public int delta { get; set; }
        /// <summary>
        /// The ID of the user who followed, may not be present in aggregations.
        /// </summary>
        public int? user { get; set; }
    }
}
