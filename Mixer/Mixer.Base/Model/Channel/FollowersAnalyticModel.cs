namespace Mixer.Base.Model.Channel
{
    public class FollowersAnalyticModel : ChannelAnalyticModel
    {
        public uint total { get; set; }
        public int delta { get; set; }
        public int? user { get; set; }
    }
}
