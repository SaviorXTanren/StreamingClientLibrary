namespace Twitch.Base.Models.V5.Streams
{
    /// <summary>
    /// Information about a featured stream.
    /// </summary>
    public class FeaturedStreamModel
    {
        /// <summary>
        /// The image for the stream.
        /// </summary>
        public string image { get; set; }
        /// <summary>
        /// The priority of the stream.
        /// </summary>
        public int priority { get; set; }
        /// <summary>
        /// Whether the stream is scheduled.
        /// </summary>
        public bool scheduled { get; set; }
        /// <summary>
        /// Whether the stream is sponsored.
        /// </summary>
        public bool sponsored { get; set; }
        /// <summary>
        /// The information about the stream.
        /// </summary>
        public StreamModel stream { get; set; }
        /// <summary>
        /// The description text for the stream.
        /// </summary>
        public string text { get; set; }
        /// <summary>
        /// The title of the stream.
        /// </summary>
        public string title { get; set; }
    }
}
