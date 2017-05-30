using Newtonsoft.Json.Linq;

namespace Mixer.Base.Model.Chat
{
    public class ChatMethodPacket : ChatPacket
    {
        public ChatMethodPacket() { this.type = "method"; }

        public string method { get; set; }
        public JArray arguments { get; set; }
        public uint id { get; set; }
    }
}
