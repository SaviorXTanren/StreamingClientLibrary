namespace Mixer.Base.Model.Clips
{
    /// <summary>
    /// Information required to create a clip.
    /// </summary>
    public class ClipRequestModel
    {
        /// <summary>
        /// Unique id for the broadcast being clipped
        /// </summary>
        public string broadcastId { get; set; }
        /// <summary>
        /// Title of the clip being created (default is the broadcast title)
        /// </summary>
        public string highlightTitle { get; set; }
        /// <summary>
        /// Length of the clip to create (default 30s, min 15s, max 300s)
        /// </summary>
        public int clipDurationInSeconds { get; set; }
        /// <summary>
        /// The stream time at the end of the clip being created (default is the latest stream time)
        /// </summary>
        public long finalStreamTimeMs { get; set; }
    }
}
