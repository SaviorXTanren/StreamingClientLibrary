namespace Mixer.Base.Model.Channel
{
    public class FollowModel : TimeStampedModel
    {
        public uint user { get; set; }
        public uint channel { get; set; }
    }
}
