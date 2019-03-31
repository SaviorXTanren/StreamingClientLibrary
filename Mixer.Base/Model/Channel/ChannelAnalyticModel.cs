using StreamingClient.Base.Util;
using System;

namespace Mixer.Base.Model.Channel
{
    public class ChannelAnalyticModel
    {
        public uint channel { get; set; }
        public string time { get; set; }

        public DateTimeOffset DateTime { get { return DateTimeOffsetExtensions.FromUTCISO8601String(this.time); } }
    }
}
