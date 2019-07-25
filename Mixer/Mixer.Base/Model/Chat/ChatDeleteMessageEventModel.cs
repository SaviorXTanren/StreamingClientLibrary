using System;

namespace Mixer.Base.Model.Chat
{
    /// <summary>
    /// A message that was deleted from chat.
    /// </summary>
    public class ChatDeleteMessageEventModel
    {
        /// <summary>
        /// The ID of the message.
        /// </summary>
        public Guid id { get; set; }
        /// <summary>
        /// The user who deleted the message.
        /// </summary>
        public ChatMessageUserModel moderator { get; set; }
    }
}
