using System;
using System.Collections.Generic;

namespace Mixer.Base.Model.Clips
{
    public class ClipLocatorModel
    {
        public string locatorType { get; set; }
        public string uri { get; set; }
    }

    public class ClipModel
    {
        public Guid contentId { get; set; }
        public List<ClipLocatorModel> contentLocators { get; set; }
        public uint durationInSeconds { get; set; }
        public DateTimeOffset expirationDate { get; set; }
        public string title { get; set; }
        public uint ownerChannelId { get; set; }
        public uint typeId { get; set; }
        public string shareableId { get; set; }
        public uint streamerChannelId { get; set; }
        public DateTimeOffset uploadDate { get; set; }
        public uint viewCount { get; set; }
        public uint contentMaturity { get; set; }
        public List<string> tags { get; set; }

        public ClipModel()
        {
            this.contentLocators = new List<ClipLocatorModel>();
            this.tags = new List<string>();
        }
    }
}
