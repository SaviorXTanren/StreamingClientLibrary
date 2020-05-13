using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Twitch.Base.Models.V5.Channel;

namespace Twitch.Base.Models.V5.Streams
{
    /// <summary>
    /// Information about a stream.
    /// </summary>
    public class StreamModel
    {
        /// <summary>
        /// The ID of the team.
        /// </summary>
        [JsonProperty("_id")]
        public long id { get; set; }
        /// <summary>
        /// The average FPS of the stream.
        /// </summary>
        public double average_fps { get; set; }
        /// <summary>
        /// The channel information for the stream.
        /// </summary>
        public ChannelModel channel { get; set; }
        /// <summary>
        /// The date the stream was created at.
        /// </summary>
        public string created_at { get; set; }
        /// <summary>
        /// The video delay of the stream.
        /// </summary>
        public int delay { get; set; }
        /// <summary>
        /// The name of the game for the stream.
        /// </summary>
        public string game { get; set; }
        /// <summary>
        /// Whether the stream is a playlist.
        /// </summary>
        public bool is_playlist { get; set; }
        /// <summary>
        /// Preview images for the stream.
        /// </summary>
        public JObject preview { get; set; }
        /// <summary>
        /// The video height of the stream.
        /// </summary>
        public int video_height { get; set; }
        /// <summary>
        /// The total number of viewers for the stream.
        /// </summary>
        public long viewers { get; set; }

        /// <summary>
        /// Whether the stream is currently live or not.
        /// </summary>
        [JsonIgnore]
        public bool IsLive { get { return this.id > 0 && !this.is_playlist; } }
    }
}
