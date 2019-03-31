using Mixer.Base.Model.User;

namespace Mixer.Base.Model.Teams
{
    /// <summary>
    /// Expanded information for a team membership.
    /// </summary>
    public class TeamMembershipExpandedModel : TeamMembershipModel
    {
        /// <summary>
        /// Whether this is the primary team for the user.
        /// </summary>
        public bool isPrimary { get; set; }
        /// <summary>
        /// The owner of the team.
        /// </summary>
        public UserModel owner { get; set; }
        /// <summary>
        /// The membership information for the team.
        /// </summary>
        public TeamMembershipModel teamMembership { get; set; }
    }
}
