using Mixer.Base.Model.User;

namespace Mixer.Base.Model.Teams
{
    public class TeamMembershipExpandedModel : TeamMembershipModel
    {
        public bool isPrimary { get; set; }
        public UserModel owner { get; set; }
        public TeamMembershipModel teamMembership { get; set; }
    }
}
