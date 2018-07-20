using System;
using System.Collections.Generic;
using System.Text;

namespace Mixer.Base.Model.Channel
{
    public class ChannelRecordingModel : TimeStampedModel
    {
        public uint id { get; set; }
        public uint channelId { get; set; }
        public string state { get; set; }
        public uint viewsTotal { get; set; }
        public DateTime expiresAt { get; set; }
        public List<ChannelVODModel> vods { get; set; }
        public bool? viewed { get; set; }
        public string name { get; set; }
        public uint typeId { get; set; }
        public uint duration { get; set; }
        public bool? seen { get; set; }
    }
}
