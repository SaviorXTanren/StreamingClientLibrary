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
        /// Basic fields for a GraphQL query.
        /// </summary>
        public const string BasicFields = "avatarUrl, confirmedAt, displayname, id, username";

        /// <summary>
        /// All fields for a GraphQL query.
        /// </summary>
        public static readonly string AllFields = $"{UserModel.BasicFields}, socials {{ {UserSocialModel.AllFields} }}";

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
        public string avatarUrl { get; set; }

        /// <summary>
        /// The date of account confirmation.
        /// </summary>
        public string confirmedAt { get; set; }

        /// <summary>
        /// The socials for the user.
        /// </summary>
        public List<UserSocialModel> socials { get; set; } = new List<UserSocialModel>();
    }
}
