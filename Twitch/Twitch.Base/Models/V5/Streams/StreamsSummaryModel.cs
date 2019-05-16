namespace Twitch.Base.Models.V5.Streams
{
    /// <summary>
    /// Information a summary of streams.
    /// </summary>
    public class StreamsSummaryModel
    {
        /// <summary>
        /// The total number of channels.
        /// </summary>
        public int channels { get; set; }
        /// <summary>
        /// The total number of viewers.
        /// </summary>
        public long viewers { get; set; }
    }
}
