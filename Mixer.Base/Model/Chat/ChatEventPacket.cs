using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mixer.Base.Model.Chat
{
    public class ChatEventPacket : ChatPacket
    {
        public ChatEventPacket() { this.type = "event"; }

        [JsonProperty("event")]
        public string eventName { get; set; }
        public JObject data { get; set; }
        public JObject NestedData
        {
            get
            {
                string modifiedData = this.data.ToString();
                modifiedData = modifiedData.Substring(1);
                modifiedData = modifiedData.Substring(0, modifiedData.Length - 1);
                return JObject.Parse(modifiedData);
            }
        }
    }
}
