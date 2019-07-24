using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mixer.Base.Model.Client
{
    /// <summary>
    /// A web socket event packet.
    /// </summary>
    public class EventPacket : WebSocketPacket
    {
        /// <summary>
        /// Creates a new instance of the EventPacket class.
        /// </summary>
        public EventPacket() { this.type = "event"; }

        /// <summary>
        /// The name of the event.
        /// </summary>
        [JsonProperty("event")]
        public string eventName { get; set; }

        /// <summary>
        /// The data of the event.
        /// </summary>
        public JObject data { get; set; }
    }
}
