using Newtonsoft.Json;

namespace Mixer.Base.Model.User
{
    /// <summary>
    /// The preferences for a user.
    /// </summary>
    public class UserPreferencesModel
    {
        /// <summary>
        /// The HTML 5 sounds to play
        /// </summary>
        [JsonProperty("chat:sounds:html5")]
        public bool? chatSoundsHTML5 { get; set; }
        /// <summary>
        /// Whether sounds should play.
        /// </summary>
        [JsonProperty("chat:sounds:play")]
        public string chatSoundsPlay { get; set; }
        /// <summary>
        /// Whether whispers are allowed.
        /// </summary>
        [JsonProperty("chat:whispers")]
        public bool? chatWhispers { get; set; }
        /// <summary>
        /// Whether timestamps should be shown.
        /// </summary>
        [JsonProperty("chat:timestamps")]
        public bool? chatTimestamps { get; set; }
        /// <summary>
        /// The chroma key for chat.
        /// </summary>
        [JsonProperty("chat:chromakey")]
        public bool? chatChromakey { get; set; }
        /// <summary>
        /// Whether tagging is allowed.
        /// </summary>
        [JsonProperty("chat:tagging")]
        public bool? chatTagging { get; set; }
        /// <summary>
        /// The volume level for chat sounds.
        /// </summary>
        [JsonProperty("chat:sounds:volume")]
        public double? chatSoundsVolume { get; set; }
        /// <summary>
        /// Where chat colors are applied.
        /// </summary>
        [JsonProperty("chat:colors")]
        public bool? chatColors { get; set; }
        /// <summary>
        /// Whether lurk mode is enabled.
        /// </summary>
        [JsonProperty("chat:lurkmode")]
        public bool? chatLurkMode { get; set; }
        /// <summary>
        /// Whether mature channels are allowed.
        /// </summary>
        [JsonProperty("channel:mature:allowed")]
        public bool? channelMatureAllowed { get; set; }
    }
}
