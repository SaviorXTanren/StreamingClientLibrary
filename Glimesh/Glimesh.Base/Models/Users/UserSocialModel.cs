namespace Glimesh.Base.Models.Users
{
    /// <summary>
    /// Information about a user's social.
    /// </summary>
    public class UserSocialModel
    {
        /// <summary>
        /// All fields for a GraphQL query.
        /// </summary>
        public const string AllFields = "id, identifier, platform, username";

        /// <summary>
        /// The ID of the social.
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// The identifier for the social.
        /// </summary>
        public string identifier { get; set; }

        /// <summary>
        /// The name of the social platform.
        /// </summary>
        public string platform { get; set; }

        /// <summary>
        /// The username for the social.
        /// </summary>
        public string username { get; set; }
    }
}
