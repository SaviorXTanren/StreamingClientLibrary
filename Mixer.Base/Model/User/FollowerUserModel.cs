namespace Mixer.Base.Model.User
{
    public class FollowerUserModel : UserWithChannelModel
    {
        public TimeStampedModel followed { get; set; }
    }
}
