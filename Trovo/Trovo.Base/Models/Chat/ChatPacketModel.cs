using Newtonsoft.Json.Linq;
using System;

namespace Trovo.Base.Models.Chat
{
    public class ChatPacketModel
    {
        public string type { get; set; }
        public string nonce { get; set; }
        public string error { get; set; }
        public JObject data { get; set; }

        public ChatPacketModel() { }

        public ChatPacketModel(string type)
        {
            this.type = type;
            this.nonce = Guid.NewGuid().ToString();
        }

        public ChatPacketModel(string type, JObject data)
            : this(type)
        {
            this.data = data;
        }
    }
}
