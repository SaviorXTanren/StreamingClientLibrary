namespace Twitch.Base.Models.NewAPI.Users
{
    /// <summary>
    /// Information about a user who follows another user.
    /// </summary>
    public class UserFollowModel
    {
        /// <summary>
        /// The ID of the user who is following.
        /// </summary>
        public string from_id { get; set; }
        /// <summary>
        /// The login name of the user who is following.
        /// </summary>
        public string from_name { get; set; }
        /// <summary>
        /// The ID of the user that is followed.
        /// </summary>
        public string to_id { get; set; }
        /// <summary>
        /// The login name of the user who is followed.
        /// </summary>
        public string to_name { get; set; }
        /// <summary>
        /// The date that the follow occurred on.
        /// </summary>
        public string followed_at { get; set; }
    }
}
