using Newtonsoft.Json;

namespace Mixer.Base.Model.Channel
{
    public class DiscordChannelModel
    {
        public string id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        [JsonProperty("private")]
        public bool isPrivate { get; set; }
    }
}
