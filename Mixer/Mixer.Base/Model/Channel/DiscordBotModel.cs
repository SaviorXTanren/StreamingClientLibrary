using Newtonsoft.Json.Linq;

namespace Mixer.Base.Model.Channel
{
    /// <summary>
    /// Lists the configuration for this Discord bot.
    /// </summary>
    public class DiscordBotModel
    {
        /// <summary>
        /// The unique ID of the Discord bot.
        /// </summary>
        public uint id { get; set; }
        /// <summary>
        /// The channel ID linked to this Discord integration.
        /// </summary>
        public uint channelId { get; set; }
        /// <summary>
        /// The server ID that this bot belongs to.
        /// </summary>
        public string guildId { get; set; }
        /// <summary>
        /// Determines which users can get invites to the Discord channel. It can be set to "everyone", "followers", "subs", or "noone" to disable invites. Defaults to "noone".
        /// </summary>
        public string inviteSettings { get; set; }
        /// <summary>
        /// The ID of the server channel that new users are invited to.
        /// </summary>
        public string inviteChannel { get; set; }
        /// <summary>
        /// Whether the user state should be changed to "Playing GAME on Mixer" when the user goes live.
        /// </summary>
        public string liveUpdateState { get; set; }
        /// <summary>
        /// Channel ID that chat messages will be mirrored to by the bot. If this is null, chat messages will not be mirrored.
        /// </summary>
        public string liveChatChannel { get; set; }
        /// <summary>
        /// Channel ID that chat announcements will be mirrored to by the bot. If this is null, chat messages will not be mirrored.
        /// </summary>
        public string liveAnnounceChannel { get; set; }
        /// <summary>
        /// A list of Discord roles who will be able to use subscriber emotes. (This only has an effect for partnered channels.)
        /// </summary>
        public uint[] syncEmoteRoles { get; set; }
        /// <summary>
        /// A list of roles which this DiscordBot controls.
        /// </summary>
        public JArray roles { get; set; }
    }
}
