namespace Mixer.Base.Model.Chat
{
    public class ChatUserTimeoutEventModel
    {
        public ChatUserEventModel user { get; set; }
        public uint duration { get; set; }
    }
}
