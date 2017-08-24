using Mixer.Base.Model.User;

namespace Mixer.Base.Model.Teams
{
    public class TeamModel : TimeStampedModel
    {
        public uint id { get; set; }
        public uint ownerId { get; set; }
        public string token { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string logoUrl { get; set; }
        public string backgroundUrl { get; set; }
        public SocialInfoModel social { get; set; }
    }
}
