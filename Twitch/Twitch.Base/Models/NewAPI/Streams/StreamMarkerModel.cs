namespace Twitch.Base.Models.NewAPI.Streams
{
    /// <summary>
    /// Information about a stream marker.
    /// </summary>
    public class StreamMarkerModel
    {
        /// <summary>
        /// The ID of the stream marker.
        /// </summary>
        public long id { get; set; }
        /// <summary>
        /// The date the stream marker was created at.
        /// </summary>
        public string created_at { get; set; }
        /// <summary>
        /// The description of the stream marker.
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// The number of seconds into the stream.
        /// </summary>
        public long position_seconds { get; set; }
        /// <summary>
        /// The url to the stream marker.
        /// </summary>
        public string URL { get; set; }
    }
}
