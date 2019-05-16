namespace Twitch.Base.Models.V5.Users
{
    /// <summary>
    /// Information about a user follow.
    /// </summary>
    public class UserFollowModel
    {
        /// <summary>
        /// The date of the follow.
        /// </summary>
        public string created_at { get; set; }
        /// <summary>
        /// Whether notifications are enabled.
        /// </summary>
        public bool notifications { get; set; }
        /// <summary>
        /// Information about the user.
        /// </summary>
        public UserModel user { get; set; }
    }
}
