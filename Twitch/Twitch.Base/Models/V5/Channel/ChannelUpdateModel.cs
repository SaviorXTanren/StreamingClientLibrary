namespace Twitch.Base.Models.V5.Channel
{
    /// <summary>
    /// Updatable properties on a channel.
    /// </summary>
    public class ChannelUpdateModel
    {
        /// <summary>
        /// The status of the channel.
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// The name of the game for the channel.
        /// </summary>
        public string game { get; set; }
        /// <summary>
        /// The live video feed delay in seconds.
        /// </summary>
        public string delay { get; set; }
        /// <summary>
        /// Whether the channel feed is enabled.
        /// </summary>
        public string channel_feed_enabled { get; set; }
    }
}
