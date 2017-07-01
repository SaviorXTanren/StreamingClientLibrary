using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mixer.Base.Model.Client
{
    public class MethodPacket : WebSocketPacket
    {
        public MethodPacket()
        {
            this.type = "method";
            this.arguments = new JArray();
        }

        public string method { get; set; }

        public bool discard { get; set; }

        public JArray arguments { get; set; }

        [JsonProperty("params")]
        public JObject parameters { get; set; }
    }
}
