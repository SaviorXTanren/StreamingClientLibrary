using Newtonsoft.Json.Linq;

namespace Mixer.Base.Model.Chat
{
    public class ChatReplyPacket : ChatPacket
    {
        public ChatReplyPacket() { this.type = "reply"; }

        public string error { get; set; }
        public JObject data { get; set; }
    }
}
