namespace Mixer.Base.Model.Channel
{
    /// <summary>
    /// Represents a single channel within a Discord server.
    /// </summary>
    public class DiscordChannelModel
    {
        /// <summary>
        /// The unique ID of the Discord channel.
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// The human name of the channel.
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// Either "voice", "text" or "category".
        /// </summary>
        public string type { get; set; }
    }
}
