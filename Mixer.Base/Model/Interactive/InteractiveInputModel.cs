using Newtonsoft.Json;

namespace Mixer.Base.Model.Interactive
{
    public class InteractiveInputModel
    {
        [JsonProperty("event")]
        public string eventType { get; set; }
        public int button { get; set; }
        public string controlID { get; set; }
        public double x { get; set; }
        public double y { get; set; }
    }
}
