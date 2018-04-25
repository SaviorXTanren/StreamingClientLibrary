using System;

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

    public class ChannelModel : ChannelUpdateableModel
    {
        public uint id { get; set; }
        public uint userId { get; set; }
        public string token { get; set; }
        public bool online { get; set; }
        public bool featured { get; set; }
        public int featureLevel { get; set; }
        public bool partnered { get; set; }
        public bool suspended { get; set; }
        public uint viewersTotal { get; set; }
        public uint viewersCurrent { get; set; }
        public uint numFollowers { get; set; }
        public int ftl { get; set; }
        public bool hasVod { get; set; }
        public string languageId { get; set; }
        public uint? coverId { get; set; }
        public uint? thumbnailId { get; set; }
        public Guid? costreamId { get; set; }
        public uint? hosteeId { get; set; }
    }
}
