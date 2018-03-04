namespace Mixer.Base.Model.Channel
{
    public class ChannelDetailsModel : ExpandedChannelModel
    {
        public string streamKey { get; set; }
        public uint numSubscribers { get; set; }
        public uint maxConcurrentSubscribers { get; set; }
        public uint totalUniqueSubscribers { get; set; }
    }
}
