namespace Twitch.Base.Models.V5.Chat
{
    /// <summary>
    /// Information for an individual chat badge.
    /// </summary>
    public class ChatBadgeModel
    {
        /// <summary>
        /// The url for the alpha transparent version of the badge image.
        /// </summary>
        public string alpha { get; set; }
        /// <summary>
        /// The url for the badge image.
        /// </summary>
        public string image { get; set; }
        /// <summary>
        /// The url for the svg version of the badge image.
        /// </summary>
        public string svg { get; set; }
    }

    /// <summary>
    /// Information for a channel's chat badges.
    /// </summary>
    public class ChannelChatBadgesModel
    {
        /// <summary>
        /// Badge information for admins.
        /// </summary>
        public ChatBadgeModel admin { get; set; }
        /// <summary>
        /// Badge information for broadcasters.
        /// </summary>
        public ChatBadgeModel broadcaster { get; set; }
        /// <summary>
        /// Badge information for global mods.
        /// </summary>
        public ChatBadgeModel global_mod { get; set; }
        /// <summary>
        /// Badge information for mods.
        /// </summary>
        public ChatBadgeModel mod { get; set; }
        /// <summary>
        /// Badge information for staff.
        /// </summary>
        public ChatBadgeModel staff { get; set; }
        /// <summary>
        /// Badge information for subscribers.
        /// </summary>
        public ChatBadgeModel subscriber { get; set; }
        /// <summary>
        /// Badge information for turbo users.
        /// </summary>
        public ChatBadgeModel turbo { get; set; }
    }
}
