namespace Mixer.Base.Model.Channel
{
    public class ChannelUpdateableModel : TimeStampedModel
    {
        public uint? transcodingProfileId { get; set; }
        public string name { get; set; }
        public string audience { get; set; }
        public string description { get; set; }
        public uint? typeId { get; set; }
        public bool interactive { get; set; }
        public uint? interactiveGameId { get; set; }
        public bool vodsEnabled { get; set; }
    }
}
