using Glimesh.Base.Models.Users;

namespace Glimesh.Base.Models.Channels
{
    /// <summary>
    /// Information about a channel.
    /// </summary>
    public class ChannelModel
    {
        /// <summary>
        /// All fields for a GraphQL query.
        /// </summary>
        public static readonly string AllFields = $"id, language, status, thumbnail, title, category {{ {CategoryModel.AllFields} }}";

        /// <summary>
        /// All fields with socials for a GraphQL query.
        /// </summary>
        public static readonly string AllFieldsWithStreamer = $"{ChannelModel.AllFields}, streamer {{ {UserModel.AllFields} }}";

        /// <summary>
        /// All fields with socials for a GraphQL query.
        /// </summary>
        public static readonly string AllFieldsWithStreamerAndStream = $"{ChannelModel.AllFieldsWithStreamer}, stream {{ {StreamModel.AllFields} }}";

        /// <summary>
        /// The ID of the channel.
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// The language of the channel.
        /// </summary>
        public string langauge { get; set; }

        /// <summary>
        /// The status of the channel.
        /// </summary>
        public string status { get; set; }

        /// <summary>
        /// The title of the channel.
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// The thumbnail image of the channel.
        /// </summary>
        public string thumbnail { get; set; }

        /// <summary>
        /// The stream key of the channel.
        /// </summary>
        public string streamKey { get; set; }

        /// <summary>
        /// The category of the channel.
        /// </summary>
        public CategoryModel category { get; set; }

        /// <summary>
        /// The user of the channel.
        /// </summary>
        public UserModel streamer { get; set; }

        /// <summary>
        /// The current stream of the channel.
        /// </summary>
        public StreamModel stream { get; set; }
    }
}
