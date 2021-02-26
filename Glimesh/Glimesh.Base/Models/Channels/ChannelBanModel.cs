using Glimesh.Base.Models.Users;

namespace Glimesh.Base.Models.Channels
{
    /// <summary>
    /// Information for a channel ban or timeout.
    /// </summary>
    public class ChannelBanModel
    {
        /// <summary>
        /// All fields for a GraphQL query.
        /// </summary>
        public static readonly string AllFields = $"user {{ id username }} reason insertedAt updatedAt expiresAt";

        /// <summary>
        /// The user that was banned or timed out.
        /// </summary>
        public UserModel user { get; set; }

        /// <summary>
        /// The reason for the ban or timeout.
        /// </summary>
        public string reason { get; set; }

        /// <summary>
        /// When the ban or timeout occurred.
        /// </summary>
        public string insertedAt { get; set; }

        /// <summary>
        /// When the ban or timeout was last updated.
        /// </summary>
        public string updatedAt { get; set; }

        /// <summary>
        /// When the timeout expires, if it was a timeout.
        /// </summary>
        public string expiresAt { get; set; }
    }
}
