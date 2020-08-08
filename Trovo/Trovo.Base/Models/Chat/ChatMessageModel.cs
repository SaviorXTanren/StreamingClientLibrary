using Newtonsoft.Json.Linq;

namespace Trovo.Base.Models.Chat
{
    public class ChatMessageModel
    {
        public string type { get; set; }
        public string nonce { get; set; }
        public string error { get; set; }
        public string data { get; set; }

        public JObject JSONData { get { return (!string.IsNullOrEmpty(this.data)) ? new JObject(this.data) : new JObject(); } }
    }
}
