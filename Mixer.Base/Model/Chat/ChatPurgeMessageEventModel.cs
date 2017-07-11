namespace Mixer.Base.Model.Chat
{
    public class ChatPurgeMessageEventModel
    {
        public uint user_id { get; set; }
        public ChatClearMessagesEventModel moderator { get; set; }
    }
}
