using Newtonsoft.Json.Linq;

namespace Mixer.Base.Model.Interactive
{
    public class InteractiveReplyPacket : InteractivePacket
    {
        public InteractiveReplyPacket() { this.type = "reply"; }

        public JObject result { get; set; }
        public InteractiveError error { get; set; }
    }
}
