namespace Glimesh.Base.Models.Channels
{
    /// <summary>
    /// Information about a stream.
    /// </summary>
    public class StreamModel
    {
        /// <summary>
        /// Basic fields for a GraphQL query.
        /// </summary>
        public static readonly string BasicFields = $"category {{ {CategoryModel.AllFields} }}, countViewers, endedAt, id, insertedAt, peakViewers, startedAt, thumbnailUrl, title";

        /// <summary>
        /// Basic fields with channel for a GraphQL query.
        /// </summary>
        public static readonly string BasicFieldsWithChannel = $"{StreamModel.BasicFields}, channel {{ {ChannelModel.BasicFields} }}";

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
        /// The category of the channel.
        /// </summary>
        public CategoryModel category { get; set; }

        /// <summary>
        /// The thumbnail image of the channel.
        /// </summary>
        public string thumbnailUrl { get; set; }

        /// <summary>
        /// The current number of viewers for the stream.
        /// </summary>
        public int? countViewers { get; set; }

        /// <summary>
        /// The peak number of viewers for the stream.
        /// </summary>
        public int? peakViewers { get; set; }

        /// <summary>
        /// The number of new subscribers for the stream.
        /// </summary>
        public int? newSubscribers { get; set; }

        /// <summary>
        /// The number of resubscribers for the stream.
        /// </summary>
        public int? resubSubscribers { get; set; }

        /// <summary>
        /// The channel information for the stream.
        /// </summary>
        public ChannelModel channel { get; set; }
    }
}
