namespace Mixer.Base.Model.Chat
{
    public class ChatPurgeMessageEventModel
    {
        public uint user_id { get; set; }
        public ChatMessageUserModel moderator { get; set; }
    }
}
