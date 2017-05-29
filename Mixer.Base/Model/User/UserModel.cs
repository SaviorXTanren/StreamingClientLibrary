namespace Mixer.Base.Model.User
{
    public class UserModel : TimeStampedModel
    {
        public uint id { get; set; }
        public uint level { get; set; }
        public SocialInfoModel social { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public bool verified { get; set; }
        public uint experience { get; set; }
        public uint sparks { get; set; }
        public string avatarUrl { get; set; }
        public string bio { get; set; }
        public uint? primaryTeam { get; set; }
        public uint? transcodingProfileId { get; set; }
        public bool? hasTranscodes { get; set; }
    }
}
