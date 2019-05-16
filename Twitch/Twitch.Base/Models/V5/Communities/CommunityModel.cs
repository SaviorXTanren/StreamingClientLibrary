using Newtonsoft.Json;

namespace Twitch.Base.Models.V5.Communities
{
    /// <summary>
    /// Information about a community.
    /// </summary>
    public class CommunityModel
    {
        /// <summary>
        /// The ID of the user.
        /// </summary>
        [JsonProperty("_id")]
        public string id { get; set; }
        /// <summary>
        /// The user ID of the owner.
        /// </summary>
        public string owner_id { get; set; }
        /// <summary>
        /// The name of the community.
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// The display name of the community.
        /// </summary>
        public string display_name { get; set; }
        /// <summary>
        /// The url of the avatar.
        /// </summary>
        public string avatar_image_url { get; set; }
        /// <summary>
        /// The url of the cover.
        /// </summary>
        public string cover_image_url { get; set; }
        /// <summary>
        /// The description of the community.
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// The HTML description of the community.
        /// </summary>
        public string description_html { get; set; }
        /// <summary>
        /// The rules of the community.
        /// </summary>
        public string rules { get; set; }
        /// <summary>
        /// The HTML rules of the community.
        /// </summary>
        public string rules_html { get; set; }
        /// <summary>
        /// The language type
        /// </summary>
        public string language { get; set; }
        /// <summary>
        /// The summary of the community.
        /// </summary>
        public string summary { get; set; }
    }
}
