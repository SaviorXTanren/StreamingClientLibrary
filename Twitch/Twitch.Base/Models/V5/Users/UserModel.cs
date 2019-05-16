using Newtonsoft.Json;

namespace Twitch.Base.Models.V5.Users
{
    /// <summary>
    /// Information about a user's notification settings.
    /// </summary>
    public class UserNotificationsModel
    {
        /// <summary>
        /// Whether email notifications are enabled.
        /// </summary>
        public bool email { get; set; }
        /// <summary>
        /// Whether push notifications are enabled.
        /// </summary>
        public bool push { get; set; }
    }

    /// <summary>
    /// Information about a user.
    /// </summary>
    public class UserModel
    {
        /// <summary>
        /// The ID of the user.
        /// </summary>
        [JsonProperty("_id")]
        public string id { get; set; }
        /// <summary>
        /// The user description.
        /// </summary>
        public string bio { get; set; }
        /// <summary>
        /// The date the user account was created.
        /// </summary>
        public string created_at { get; set; }
        /// <summary>
        /// The user's display name.
        /// </summary>
        public string display_name { get; set; }
        /// <summary>
        /// The user's email address.
        /// </summary>
        public string email { get; set; }
        /// <summary>
        /// Whether the user's email has been verified.
        /// </summary>
        public bool email_verified { get; set; }
        /// <summary>
        /// The user's logo.
        /// </summary>
        public string logo { get; set; }
        /// <summary>
        /// The user's account name.
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// The user's notification settings.
        /// </summary>
        public UserNotificationsModel notifications { get; set; }
        /// <summary>
        /// If the user is partnered.
        /// </summary>
        public bool partnered { get; set; }
        /// <summary>
        /// If the user's twitter is connected.
        /// </summary>
        public bool twitter_connected { get; set; }
        /// <summary>
        /// The type of user account.
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// The date the account was last updated.
        /// </summary>
        public string updated_at { get; set; }
    }
}
