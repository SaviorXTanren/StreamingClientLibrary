using Newtonsoft.Json.Linq;

namespace Mixer.Base.Model.Client
{
    public class ReplyPacket : WebSocketPacket
    {
        public ReplyPacket() { this.type = "reply"; }

        public JObject result { get; set; }
        public JObject error { get; set; }
        public JObject data { get; set; }
    }
}
