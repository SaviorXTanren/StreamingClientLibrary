using Newtonsoft.Json;

namespace Twitch.Base.Models.V5.Channel
{
    /// <summary>
    /// Information about a channel.
    /// </summary>
    public class ChannelModel
    {
        /// <summary>
        /// The ID of the channel.
        /// </summary>
        [JsonProperty("_id")]
        public string id { get; set; }
        /// <summary>
        /// The language of the broadcaster.
        /// </summary>
        public string broadcaster_language { get; set; }
        /// <summary>
        /// The date the channel was created.
        /// </summary>
        public string created_at { get; set; }
        /// <summary>
        /// The display name of the channel.
        /// </summary>
        public string display_name { get; set; }
        /// <summary>
        /// The total number of followers.
        /// </summary>
        public long followers { get; set; }
        /// <summary>
        /// The name of the game set for the channel.
        /// </summary>
        public string game { get; set; }
        /// <summary>
        /// The currently set language
        /// </summary>
        public string language { get; set; }
        /// <summary>
        /// The url to the logo.
        /// </summary>
        public string logo { get; set; }
        /// <summary>
        /// Whether the channel is set to mature.
        /// </summary>
        public bool mature { get; set; }
        /// <summary>
        /// The account name of the channel.
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// Whether the channel is partnered.
        /// </summary>
        public bool partner { get; set; }
        /// <summary>
        /// The url to the profile banner.
        /// </summary>
        public string profile_banner { get; set; }
        /// <summary>
        /// The background color of the profile banner.
        /// </summary>
        public string profile_banner_background_color { get; set; }
        /// <summary>
        /// The status of the channel.
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// The date when the channel was updated.
        /// </summary>
        public string updated_at { get; set; }
        /// <summary>
        /// The url for the channel.
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// The url of the video for the banner.
        /// </summary>
        public string video_banner { get; set; }
        /// <summary>
        /// The total number of views for the channel.
        /// </summary>
        public long views { get; set; }
    }
}
