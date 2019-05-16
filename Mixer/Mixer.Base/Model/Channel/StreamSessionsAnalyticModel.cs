namespace Mixer.Base.Model.Channel
{
    public class StreamSessionsAnalyticModel : ChannelAnalyticModel
    {
        public bool online { get; set; }
        public uint? duration { get; set; }
        public uint? type { get; set; }
    }
}
