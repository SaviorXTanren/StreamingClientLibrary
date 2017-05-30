using Newtonsoft.Json.Linq;

namespace Mixer.Base.Model.Chat
{
    public class ChatPollEventModel
    {
        public string q { get; set; }
        public string[] answers { get; set; }
        public ChatUserEventModel author { get; set; }
        public uint duration { get; set; }
        public long endsAt { get; set; }
        public uint voters { get; set; }
        public JObject responses { get; set; }
    }
}
