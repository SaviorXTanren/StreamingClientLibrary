namespace Twitch.Base.Models.NewAPI.Channels
{
    /// <summary>
    /// Information about a channel.
    /// </summary>
    public class ChannelInformationModel
    {
        /// <summary>
        /// Twitch User ID of this channel owner
        /// </summary>
        public string broadcaster_id { get; set; }
        /// <summary>
        /// Twitch User name of this channel owner
        /// </summary>
        public string broadcaster_name { get; set; }
        /// <summary>
        /// Language of the channel
        /// </summary>
        public string broadcaster_language { get; set; }
        /// <summary>
        /// Current game ID being played on the channel
        /// </summary>
        public string game_id { get; set; }
        /// <summary>
        /// Current game name being played on the channel
        /// </summary>
        public string game_name { get; set; }
        /// <summary>
        /// Title of the stream
        /// </summary>
        public string title { get; set; }
    }
}
