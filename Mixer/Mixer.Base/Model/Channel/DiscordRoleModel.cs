namespace Mixer.Base.Model.Channel
{
    /// <summary>
    /// Represents a role within a Discord server.
    /// </summary>
    public class DiscordRoleModel
    {
        /// <summary>
        /// The unique ID of the Discord role.
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// The human name of the role.
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// Hex color code for the channel, starting with a pound # symbol
        /// </summary>
        public string color { get; set; }
        /// <summary>
        /// Whether the bot has permission to assign users to this role. This depends on the role's position on Discord.
        /// </summary>
        public bool? assignable { get; set; }
    }
}
