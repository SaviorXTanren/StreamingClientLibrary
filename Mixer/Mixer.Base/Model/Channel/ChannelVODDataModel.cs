using System;
using System.Collections.Generic;
using System.Text;

namespace Mixer.Base.Model.Channel
{
    public class ChannelVODDataModel
    {
        public uint Width { get; set; }
        public uint Height { get; set; }
        public uint? Fps { get; set; }
        public uint? Bitrate { get; set; }
    }
}
