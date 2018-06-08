using Mixer.Base.Model.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mixer.Base.Model.Interactive
{
    public class InteractiveGiveInputModel : MethodPacket
    {
        public string participantID { get; set; }
        public string transactionID { get; set; }
        public InteractiveInputModel input { get; set; }
    }

    public class InteractiveInputModel
    {
        public string controlID { get; set; }
        [JsonProperty("event")]
        public string eventType { get; set; }

        public int button { get; set; }

        public double x { get; set; }
        public double y { get; set; }

        public string value { get; set; }

        public JObject meta { get; set; }
    }
}
