using Newtonsoft.Json.Linq;

namespace Mixer.Base.Model.Client
{
    public class ReplyErrorModel
    {
        public uint code { get; set; }
        public string message { get; set; }
        public JArray stacktrace { get; set; }
        public JObject data { get; set; }
        public string path { get; set; }
    }
}
