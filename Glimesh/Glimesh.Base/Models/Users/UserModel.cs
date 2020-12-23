using Newtonsoft.Json;
using System.Collections.Generic;

namespace Glimesh.Base.Models.Users
{
    /// <summary>
    /// Information about a user
    /// </summary>
    public class UserModel
    {
        /// <summary>
        /// All fields for a GraphQL query.
        /// </summary>
        public const string AllFields = "avatar, confirmedAt, displayname, id, username";

        /// <summary>
        /// All fields with socials for a GraphQL query.
        /// </summary>
        public static readonly string AllFieldsWithSocials = $"{UserModel.AllFields}, socials {{ {UserSocialModel.AllFields} }}";

        /// <summary>
        /// The ID of the user.
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// The name of the user.
        /// </summary>
        public string username { get; set; }

        /// <summary>
        /// The display name of the user.
        /// </summary>
        public string displayname { get; set; }

        /// <summary>
        /// The avatar URL of the user.
        /// </summary>
        public string avatar { get; set; }

        /// <summary>
        /// The date of account confirmation.
        /// </summary>
        public string confirmedAt { get; set; }

        /// <summary>
        /// The socials for the user.
        /// </summary>
        public List<UserSocialModel> socials { get; set; } = new List<UserSocialModel>();

        /// <summary>
        /// The full URL for the user's avatar.
        /// </summary>
        [JsonIgnore]
        public string FullAvatarURL { get { return "https://glimesh.tv/" + this.avatar; } }
    }
}
