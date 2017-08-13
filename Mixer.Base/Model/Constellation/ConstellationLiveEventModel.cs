using Newtonsoft.Json.Linq;

namespace Mixer.Base.Model.Constellation
{
    public class ConstellationLiveEventModel
    {
        public string channel { get; set; }

        public JObject payload { get; set; }
    }
}
