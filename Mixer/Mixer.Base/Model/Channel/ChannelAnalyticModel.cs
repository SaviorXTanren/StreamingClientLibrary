using StreamingClient.Base.Util;
using System;

namespace Mixer.Base.Model.Channel
{
    /// <summary>
    /// A base analytics type for channel based analytical metrics.
    /// </summary>
    public class ChannelAnalyticModel
    {
        /// <summary>
        /// The channel.
        /// </summary>
        public uint channel { get; set; }
        /// <summary>
        /// Timestamp for when this metric was recorded.
        /// </summary>
        public string time { get; set; }

        /// <summary>
        /// Date time for when this metric was recorded.
        /// </summary>
        public DateTimeOffset DateTime { get { return DateTimeOffsetExtensions.FromUTCISO8601String(this.time); } }
    }
}
