using Newtonsoft.Json;

namespace Mixer.Base.Model.User
{
    public class UserPreferencesModel
    {
        [JsonProperty("chat:sounds:html5")]
        public bool? chatSoundsHTML5 { get; set; }
        [JsonProperty("chat:sounds:play")]
        public string chatSoundsPlay { get; set; }
        [JsonProperty("chat:whispers")]
        public bool? chatWhispers { get; set; }
        [JsonProperty("chat:timestamps")]
        public bool? chatTimestamps { get; set; }
        [JsonProperty("chat:chromakey")]
        public bool? chatChromakey { get; set; }
        [JsonProperty("chat:tagging")]
        public bool? chatTagging { get; set; }
        [JsonProperty("chat:sounds:volume")]
        public double? chatSoundsVolume { get; set; }
        [JsonProperty("chat:colors")]
        public bool? chatColors { get; set; }
        [JsonProperty("chat:lurkmode")]
        public bool? chatLurkMode { get; set; }
        [JsonProperty("channel:mature:allowed")]
        public bool? channelMatureAllowed { get; set; }
    }
}
