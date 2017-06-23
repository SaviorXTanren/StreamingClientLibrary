using Newtonsoft.Json;

namespace Mixer.Base.Model.Interactive
{
    public class InteractiveMethodPacket : InteractivePacket
    {
        public InteractiveMethodPacket() { this.type = "method"; }

        public bool discard { get; set; }
        public string method { get; set; }
        [JsonProperty("params")]
        public object parameters { get; set; }
    }
}
