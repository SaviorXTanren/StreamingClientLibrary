namespace Mixer.Base.Model.Teams
{
    /// <summary>
    /// A member of a team.
    /// </summary>
    public class TeamMembershipModel : TimeStampedModel
    {
        /// <summary>
        /// The ID of the membership.
        /// </summary>
        public uint id { get; set; }
        /// <summary>
        /// The ID of the team.
        /// </summary>
        public uint teamId { get; set; }
        /// <summary>
        /// The ID of the user.
        /// </summary>
        public uint userId { get; set; }
        /// <summary>
        /// Whether the user has accepted the invitation to the team.
        /// </summary>
        public bool accepted { get; set; }
    }
}
