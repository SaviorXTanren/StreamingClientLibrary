namespace Twitch.Base.Models.V5.Channel
{
    /// <summary>
    /// Private information about a channel.
    /// </summary>
    public class PrivateChannelModel : ChannelModel
    {
        /// <summary>
        /// The type of broadcaster for the channel.
        /// </summary>
        public string broadcaster_type { get; set; }
        /// <summary>
        /// The private key for streaming.
        /// </summary>
        public string stream_key { get; set; }
        /// <summary>
        /// The email of the channel.
        /// </summary>
        public string email { get; set; }
    }
}
