using Glimesh.Base.Models.Users;

namespace Glimesh.Base.Models.Channels
{
    /// <summary>
    /// A moderator for a channel.
    /// </summary>
    public class ChannelModeratorModel
    {
        /// <summary>
        /// Basic fields for a GraphQL query.
        /// </summary>
        public const string BasicFields = "id, canDelete, canShortTimeout, canLongTimeout, canUnTimeout, canBan, canUnban";

        /// <summary>
        /// All fields for a GraphQL query.
        /// </summary>
        public static readonly string AllFields = $"{ChannelModeratorModel.BasicFields}, user {{ {UserModel.BasicFields} }}";

        /// <summary>
        /// The ID of the moderator and channel combination.
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// The user information for the moderator.
        /// </summary>
        public UserModel user { get; set; }

        /// <summary>
        /// Whether the moderator can delete messages.
        /// </summary>
        public bool canDelete { get; set; }

        /// <summary>
        /// Whether the moderator can perform a short timeout.
        /// </summary>
        public bool canShortTimeout { get; set; }

        /// <summary>
        /// Whether the moderator can perform a long timeout.
        /// </summary>
        public bool canLongTimeout { get; set; }

        /// <summary>
        /// Whether the moderator can un-timeout users.
        /// </summary>
        public bool canUnTimeout { get; set; }

        /// <summary>
        /// Whether the moderator can ban users.
        /// </summary>
        public bool canBan { get; set; }

        /// <summary>
        /// Whether the moderator can unban users.
        /// </summary>
        public bool canUnban { get; set; }
    }
}
