using Newtonsoft.Json;
using System.Collections.Generic;
using Twitch.Base.Models.V5.Users;

namespace Twitch.Base.Models.V5.Teams
{
    /// <summary>
    /// Information about a team.
    /// </summary>
    public class TeamModel
    {
        /// <summary>
        /// The ID of the team.
        /// </summary>
        [JsonProperty("_id")]
        public string id { get; set; }
        /// <summary>
        /// The url for the background.
        /// </summary>
        public string background { get; set; }
        /// <summary>
        /// The url for the banner.
        /// </summary>
        public string banner { get; set; }
        /// <summary>
        /// The date the team was created.
        /// </summary>
        public string created_at { get; set; }
        /// <summary>
        /// The display name for the team.
        /// </summary>
        public string display_name { get; set; }
        /// <summary>
        /// The information for the team.
        /// </summary>
        public string info { get; set; }
        /// <summary>
        /// The url for the logo.
        /// </summary>
        public string logo { get; set; }
        /// <summary>
        /// The name of the team.
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// The date the team was updated.
        /// </summary>
        public string updated_at { get; set; }
    }

    /// <summary>
    /// Detailed information about a team
    /// </summary>
    public class TeamDetailsModel : TeamModel
    {
        /// <summary>
        /// The members of the team.
        /// </summary>
        public List<UserModel> users { get; set; } = new List<UserModel>();
    }
}
