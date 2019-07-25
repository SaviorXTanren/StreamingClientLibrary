namespace Mixer.Base.Model.Chat
{
    /// <summary>
    /// A user that was purged in chat.
    /// </summary>
    public class ChatPurgeMessageEventModel
    {
        /// <summary>
        /// The ID of the user who was purged.
        /// </summary>
        public uint user_id { get; set; }
        /// <summary>
        /// The user who performed the purge.
        /// </summary>
        public ChatMessageUserModel moderator { get; set; }
    }
}
