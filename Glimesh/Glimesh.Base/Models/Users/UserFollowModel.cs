namespace Glimesh.Base.Models.Users
{
    /// <summary>
    /// Information about a user follow.
    /// </summary>
    public class UserFollowModel
    {
        /// <summary>
        /// All fields for a GraphQL query.
        /// </summary>
        public const string AllFields = "id, insertedAt, updatedAt";

        /// <summary>
        /// All fields for a GraphQL query.
        /// </summary>
        public static readonly string AllFieldsWithStreamerAndUser = $"{UserFollowModel.AllFields}, streamer {{ {UserModel.AllFields} }}, user {{ {UserModel.AllFields} }}";

        /// <summary>
        /// The ID of the follow.
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// The datetime of the follow.
        /// </summary>
        public string insertedAt { get; set; }

        /// <summary>
        /// The datetime of the follow update.
        /// </summary>
        public string updatedAt { get; set; }

        /// <summary>
        /// The streamer who was followed.
        /// </summary>
        public UserModel streamer { get; set; }

        /// <summary>
        /// The user who followed.
        /// </summary>
        public UserModel user { get; set; }
    }
}
