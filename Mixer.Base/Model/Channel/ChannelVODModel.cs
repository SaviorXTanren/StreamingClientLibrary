using System;
using System.Collections.Generic;
using System.Text;

namespace Mixer.Base.Model.Channel
{
    public class ChannelVODModel : TimeStampedModel
    {
        public uint id { get; set; }
        public string baseUrl { get; set; }
        public string format { get; set; }
        public ChannelVODDataModel data { get; set; }
        public uint recordingId { get; set; }
    }
}
