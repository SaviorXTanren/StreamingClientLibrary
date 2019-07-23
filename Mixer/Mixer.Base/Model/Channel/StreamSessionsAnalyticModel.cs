namespace Mixer.Base.Model.Channel
{
    /// <summary>
    /// Metric showing a channel going online or offline.
    /// </summary>
    public class StreamSessionsAnalyticModel : ChannelAnalyticModel
    {
        /// <summary>
        /// Whether this event is the channel going online, or offline.
        /// </summary>
        public bool online { get; set; }
        /// <summary>
        /// The length of the last stream in seconds. This is only included in offline events.
        /// </summary>
        public uint? duration { get; set; }
        /// <summary>
        /// The ID of the game the stream was playing. This is only included in offline events.
        /// </summary>
        public uint? type { get; set; }
        /// <summary>
        /// Whether the channel was partnered at the time of this data point.
        /// </summary>
        public bool? partnered { get; set; }
    }
}
