namespace Mixer.Base.Model.Channel
{
    public class SubscriberAnalyticModel : ChannelAnalyticModel
    {
        public uint total { get; set; }
        public int delta { get; set; }
        public uint user { get; set; }
    }
}
