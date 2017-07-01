using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mixer.Base.Model.Client
{
    public class EventPacket : WebSocketPacket
    {
        public EventPacket() { this.type = "event"; }

        [JsonProperty("event")]
        public string eventName { get; set; }

        public JObject data { get; set; }
    }
}
