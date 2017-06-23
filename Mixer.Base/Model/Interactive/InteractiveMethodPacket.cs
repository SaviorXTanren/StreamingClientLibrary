using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mixer.Base.Model.Interactive
{
    public class InteractiveMethodPacket : InteractivePacket
    {
        public InteractiveMethodPacket()
        {
            this.type = "method";
            this.parameters = new JObject();
        }

        public bool discard { get; set; }
        public string method { get; set; }
        [JsonProperty("params")]
        public JObject parameters { get; set; }
    }
}
