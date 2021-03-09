using System.Collections.Generic;

namespace Trovo.Base.Models.Chat
{
    /// <summary>
    /// The types of chat messages.
    /// </summary>
    public enum ChatMessageTypeEnum
    {
        /// <summary>
        /// Normal chat messages
        /// </summary>
        Normal = 0,
        /// <summary>
        /// Spells, including: mana spells, elixir spells
        /// </summary>
        Spell = 5,
        /// <summary>
        /// Magic chat - super cap chat
        /// </summary>
        MagicChatSuperCapChat = 6,
        /// <summary>
        /// Magic chat - colorful chat
        /// </summary>
        MagicChatColorfulChat = 7,
        /// <summary>
        /// Magic chat - spell chat
        /// </summary>
        MagicChatSpellChat = 8,
        /// <summary>
        /// Magic chat - bullet screen chat
        /// </summary>
        MagicChatBulletScreenChat = 9,
        /// <summary>
        /// Subscription message. Shows when someone subscribes to the channel.
        /// </summary>
        SubscriptionAlert = 5001,
        /// <summary>
        /// System message.
        /// </summary>
        SystemMessage = 5002,
        /// <summary>
        /// Follow message. Shows when someone follows the channel.
        /// </summary>
        FollowAlert = 5003,
        /// <summary>
        /// Welcome message when viewer joins the channel.
        /// </summary>
        WelcomeMessage = 5004,
        /// <summary>
        /// Gift sub message. When a user randomly sends gift subscriptions to one or more users in the channel.
        /// </summary>
        GiftedSubscriptionSentMessage = 5005,
        /// <summary>
        /// Gift sub message. The detailed messages when a user sends a gift subscription to another user.
        /// </summary>
        GiftedSubscriptionMessage = 5006,
        /// <summary>
        /// Activity / events message. For platform level events.
        /// </summary>
        ActivityEventMessage = 5007,
        /// <summary>
        /// Welcome message when users join the channel from raid.
        /// </summary>
        WelcomeMessageFromRaid = 5008,
        /// <summary>
        /// Custom Spells
        /// </summary>
        CustomSpell = 5009
    }

    /// <summary>
    /// Information about a chat message container.
    /// </summary>
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

    /// <summary>
    /// Information about an individual chat message piece.
    /// </summary>
    public class ChatMessageModel
    {
        /// <summary>
        /// Streamer of the current channel
        /// </summary>
        public const string StreamerRole = "streamer";
        /// <summary>
        /// Moderator of the current channel
        /// </summary>
        public const string ModeratorRole = "mod";
        /// <summary>
        /// User who followed the current channel
        /// </summary>
        public const string FollowerRole = "follower";
        /// <summary>
        /// User who subscribed the current channel
        /// </summary>
        public const string SubscriberRole = "subscriber";
        /// <summary>
        /// Admin of Trovo platform, across all channels.
        /// </summary>
        public const string AdminRole = "admin";
        /// <summary>
        /// Warden of Trovo platform, across all channels, who helps to maintain the platform order.
        /// </summary>
        public const string WardenRole = "warden";

        /// <summary>
        /// The ID of the message.
        /// </summary>
        public string message_id { get; set; }

        /// <summary>
        /// The ID of the sender.
        /// </summary>
        public string sender_id { get; set; }

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
        /// The roles of the sender.
        /// </summary>
        public List<string> roles { get; set; } = new List<string>();

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
