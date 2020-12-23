namespace Glimesh.Base.Models.Channels
{
    /// <summary>
    /// Information about a stream.
    /// </summary>
    public class StreamModel
    {
        /// <summary>
        /// All fields for a GraphQL query.
        /// </summary>
        public const string AllFields = "avgChatters, avgViewers, countChatters, countViewers, endedAt, id, newSubscribers, peakChatters, peakViewers, resubSubscribers, startedAt, title";

        /// <summary>
        /// All fields with socials for a GraphQL query.
        /// </summary>
        public static readonly string AllFieldsWithChannel = $"{StreamModel.AllFields}, channel {{ {ChannelModel.AllFields} }}";

        /// <summary>
        /// The ID of the stream.
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// The title of the stream.
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// The datetime of the start of the stream.
        /// </summary>
        public string startedAt { get; set; }

        /// <summary>
        /// The datetime of the end of the stream.
        /// </summary>
        public string endedAt { get; set; }

        /// <summary>
        /// The average number of chatter for the stream.
        /// </summary>
        public int avgChatters { get; set; }

        /// <summary>
        /// The average number of viewers for the stream.
        /// </summary>
        public int avgViewers { get; set; }

        /// <summary>
        /// The current number of chatter for the stream.
        /// </summary>
        public int countChatters { get; set; }

        /// <summary>
        /// The current number of viewers for the stream.
        /// </summary>
        public int countViewers { get; set; }

        /// <summary>
        /// The peak number of chatter for the stream.
        /// </summary>
        public int peakChatters { get; set; }

        /// <summary>
        /// The peak number of viewers for the stream.
        /// </summary>
        public int peakViewers { get; set; }

        /// <summary>
        /// The number of new subscribers for the stream.
        /// </summary>
        public int newSubscribers { get; set; }

        /// <summary>
        /// The number of resubscribers for the stream.
        /// </summary>
        public int resubSubscribers { get; set; }

        /// <summary>
        /// The channel information for the stream.
        /// </summary>
        public ChannelModel channel { get; set; }
    }
}
