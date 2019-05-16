using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Twitch.Base.Models.V5.Channel;

namespace Twitch.Base.Models.V5.Videos
{
    /// <summary>
    /// Information about a video.
    /// </summary>
    public class VideoModel
    {
        /// <summary>
        /// The ID of the user.
        /// </summary>
        [JsonProperty("_id")]
        public string id { get; set; }
        /// <summary>
        /// The ID of the broadcast.
        /// </summary>
        public string broadcast_id { get; set; }
        /// <summary>
        /// The type of broadcast.
        /// </summary>
        public string broadcast_type { get; set; }
        /// <summary>
        /// The channel information.
        /// </summary>
        public ChannelModel channel { get; set; }
        /// <summary>
        /// The date the video was created.
        /// </summary>
        public string created_at { get; set; }
        /// <summary>
        /// The description of the video.
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// The HTML description of the video.
        /// </summary>
        public string description_html { get; set; }
        /// <summary>
        /// The FPS of the video.
        /// </summary>
        public JObject fps { get; set; }
        /// <summary>
        /// The name of the game of the video.
        /// </summary>
        public string game { get; set; }
        /// <summary>
        /// The language of the video.
        /// </summary>
        public string language { get; set; }
        /// <summary>
        /// The length of the video.
        /// </summary>
        public int length { get; set; }
        /// <summary>
        /// The preview information of the video.
        /// </summary>
        public JObject preview { get; set; }
        /// <summary>
        /// The date the video was published.
        /// </summary>
        public string published_at { get; set; }
        /// <summary>
        /// The resolutions of the video.
        /// </summary>
        public JObject resolutions { get; set; }
        /// <summary>
        /// The status of the video.
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// The tags for the video.
        /// </summary>
        public string tag_list { get; set; }
        /// <summary>
        /// The thumbnails of the video.
        /// </summary>
        public JObject thumbnails { get; set; }
        /// <summary>
        /// The title of the video.
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// The url of the video.
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// Whether the video is viewable.
        /// </summary>
        public string viewable { get; set; }
        /// <summary>
        /// The date the video was viewable.
        /// </summary>
        public string viewable_at { get; set; }
        /// <summary>
        /// The total number of views.
        /// </summary>
        public long views { get; set; }
    }
}
