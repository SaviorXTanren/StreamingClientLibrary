using Newtonsoft.Json.Linq;

namespace Mixer.Base.Model.Client
{
    public class ReplyPacket : WebSocketPacket
    {
        public ReplyPacket() { this.type = "reply"; }

        public int seq { get; set; }

        public JToken result { get; set; }
        public JToken error { get; set; }
        public JToken data { get; set; }

        public JObject resultObject { get { return (this.result != null && this.result.HasValues) ? JObject.Parse(this.result.ToString()) : null; } }
        public JObject errorObject { get { return (this.error != null && this.error.HasValues) ? JObject.Parse(this.error.ToString()) : null; } }
        public JObject dataObject { get { return (this.data != null && this.data.HasValues) ? JObject.Parse(this.data.ToString()) : null; } }
    }
}
