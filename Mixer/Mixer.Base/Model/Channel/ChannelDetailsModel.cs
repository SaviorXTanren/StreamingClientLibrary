namespace Mixer.Base.Model.Channel
{
    /// <summary>
    /// Details of a channel.
    /// </summary>
    public class ChannelDetailsModel : ExpandedChannelModel
    {
        /// <summary>
        /// The users stream key.
        /// </summary>
        public string streamKey { get; set; }
        /// <summary>
        /// The current amount of subscribers the channel has.
        /// </summary>
        public uint numSubscribers { get; set; }
        /// <summary>
        /// The maximum amount of concurrent subscribers.
        /// </summary>
        public uint maxConcurrentSubscribers { get; set; }
        /// <summary>
        /// The total amount of unique subscribers the user has had.
        /// </summary>
        public uint totalUniqueSubscribers { get; set; }
    }
}
