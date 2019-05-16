using Mixer.Base.Model.User;

namespace Mixer.Base.Model.Teams
{
    /// <summary>
    /// Information for a team.
    /// </summary>
    public class TeamModel : TimeStampedModel
    {
        /// <summary>
        /// The ID of the team.
        /// </summary>
        public uint id { get; set; }
        /// <summary>
        /// The user ID of the team owner.
        /// </summary>
        public uint ownerId { get; set; }
        /// <summary>
        /// The searchable token name.
        /// </summary>
        public string token { get; set; }
        /// <summary>
        /// The name of the team.
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// The team description.
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// The URL for the team logo.
        /// </summary>
        public string logoUrl { get; set; }
        /// <summary>
        /// The URL for the team background.
        /// </summary>
        public string backgroundUrl { get; set; }
        /// <summary>
        /// The social information for the team.
        /// </summary>
        public SocialInfoModel social { get; set; }
    }
}
