namespace Mixer.Base.Model.Chat
{
    /// <summary>
    /// A user timeout event in chat.
    /// </summary>
    public class ChatUserTimeoutEventModel
    {
        /// <summary>
        /// The user that was timed out.
        /// </summary>
        public ChatUserEventModel user { get; set; }
        /// <summary>
        /// The duration of the timeout.
        /// </summary>
        public uint duration { get; set; }
    }
}
