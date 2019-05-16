namespace Twitch.Base.Models.Clients.Chat
{
    /// <summary>
    /// Information about a chat user state packet.
    /// </summary>
    public class ChatUserStatePacketModel : ChatPacketModelBase
    {
        /// <summary>
        /// The ID of the command for a chat user state.
        /// </summary>
        public const string CommandID = "USERSTATE";

        /// <summary>
        /// The user's display name.
        /// </summary>
        public string UserDisplayName { get; set; }

        /// <summary>
        /// The user's badge information.
        /// </summary>
        public string UserBadgeInfo { get; set; }

        /// <summary>
        /// The user's badges.
        /// </summary>
        public string UserBadges { get; set; }

        /// <summary>
        /// Indicates whether the user is a moderator.
        /// </summary>
        public bool Moderator { get; set; }

        /// <summary>
        /// Hexadecimal RGB color code of the message, if any.
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// Information to the user's emote sets.
        /// </summary>
        public string EmoteSets { get; set; }

        /// <summary>
        /// Creates a new instance of the ChatUserStatePacketModel class.
        /// </summary>
        /// <param name="packet">The Chat packet</param>
        public ChatUserStatePacketModel(ChatRawPacketModel packet)
            : base(packet)
        {
            this.UserDisplayName = packet.GetTagString("display-name");
            this.UserBadgeInfo = packet.GetTagString("badge-info");
            this.UserBadges = packet.GetTagString("badges");
            this.Moderator = packet.GetTagBool("mod");

            this.Color = packet.GetTagString("color");
            this.EmoteSets = packet.GetTagString("emote-sets");
        }
    }
}
