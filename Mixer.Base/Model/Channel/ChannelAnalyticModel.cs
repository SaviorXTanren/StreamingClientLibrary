using Mixer.Base.Util;
using System;

namespace Mixer.Base.Model.Channel
{
    public class ChannelAnalyticModel
    {
        public uint channel { get; set; }
        public string time { get; set; }
        public DateTimeOffset dateTime { get { return DateTimeHelper.ISO8601StringToDateTimeOffset(this.time); } }
    }
}
