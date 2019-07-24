namespace Mixer.Base.Model.Chat
{
    /// <summary>
    /// When all chat has been cleared from a channel.
    /// </summary>
    public class ChatClearMessagesEventModel
    {
        /// <summary>
        /// The user who cleared chat from the channel.
        /// </summary>
        public ChatMessageUserModel clearer { get; set; }
    }
}
