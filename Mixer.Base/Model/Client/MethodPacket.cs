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

        public MethodPacket(string method) : this() { this.method = method; }

        public string method { get; set; }

        public int seq { get; set; }

        public bool discard { get; set; }

        public JArray arguments { get; set; }

        [JsonProperty("params")]
        public JObject parameters { get; set; }
    }

    public class MethodArgPacket : MethodPacket
    {
        public MethodArgPacket(string method, JArray arguments)
            : base()
        {
            this.method = method;
            this.arguments = arguments;
        }
    }

    public class MethodParamsPacket : MethodPacket
    {
        public MethodParamsPacket(string method, JObject parameters)
            : base()
        {
            this.method = method;
            this.parameters = parameters;
        }
    }
}
