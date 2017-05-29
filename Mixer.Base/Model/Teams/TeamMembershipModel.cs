namespace Mixer.Base.Model.Teams
{
    public class TeamMembershipModel : TimeStampedModel
    {
        public uint id { get; set; }
        public uint teamId { get; set; }
        public uint userId { get; set; }
        public bool accepted { get; set; }
    }
}
