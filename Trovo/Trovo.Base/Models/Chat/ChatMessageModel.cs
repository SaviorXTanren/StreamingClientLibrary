using System.Collections.Generic;

namespace Trovo.Base.Models.Chat
{
    public enum ChatMessageTypeEnum
    {
        Normal = 0,
        Spell = 5,
        SubscriptionAlert = 5001,
        FollowAlert = 5003,
        WelcomeMessage = 5004,
    }

    public class ChatMessageContainerModel
    {
        /// <summary>
        /// ID of the message
        /// </summary>
        public string eid { get; set; }

        /// <summary>
        /// A list of chats. One chat message may contain multiple chats.
        /// </summary>
        public List<ChatMessageModel> chats { get; set; } = new List<ChatMessageModel>();
    }

    public class ChatMessageModel
    {
        /// <summary>
        /// Type of chat message.
        /// </summary>
        public ChatMessageTypeEnum type { get; set; }

        /// <summary>
        /// Content of the message
        /// </summary>
        public string content { get; set; }

        /// <summary>
        /// Display name of the sender
        /// </summary>
        public string nick_name { get; set; }

        /// <summary>
        /// URL of the sender’s profile picture
        /// </summary>
        public string avatar { get; set; }

        /// <summary>
        /// The subscription level of the user in the channel. “sub_L1” for tier 1 subscriber.
        /// </summary>
        public string sub_lv { get; set; }

        /// <summary>
        /// The list of badge names of the sender.
        /// </summary>
        public List<string> medals { get; set; } = new List<string>();

        /// <summary>
        /// The list of decoration names of sender.
        /// </summary>
        public List<string> decos { get; set; } = new List<string>();

        /// <summary>
        /// Name of the spell. Only for chat messages of spell (type = 5), in the content field.
        /// </summary>
        public string gift { get; set; }

        /// <summary>
        /// Number of spells. Only for chat messages of spell (type = 5), in the content field.
        /// </summary>
        public int num { get; set; }
    }
}
