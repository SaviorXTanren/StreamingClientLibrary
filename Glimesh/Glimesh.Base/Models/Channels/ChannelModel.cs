using Glimesh.Base.Models.GraphQL;
using Glimesh.Base.Models.Users;
using Newtonsoft.Json;
using System;

namespace Glimesh.Base.Models.Channels
{
    /// <summary>
    /// Information about a channel.
    /// </summary>
    public class ChannelModel
    {
        /// <summary>
        /// Basic fields for a GraphQL query.
        /// </summary>
        public static readonly string BasicFields = $"id, language, status, title, updatedAt";

        /// <summary>
        /// Basic fields with streamer for a GraphQL query.
        /// </summary>
        public static readonly string BasicFieldsWithStreamer = $"{ChannelModel.BasicFields}, streamer {{ {UserModel.BasicFields} }}";

        /// <summary>
        /// All fields for a GraphQL query.
        /// </summary>
        public static readonly string AllFields = $"{ChannelModel.BasicFieldsWithStreamer}, moderators(first: 100) {{ edges {{ cursor, node {{ {ChannelModeratorModel.AllFields} }} }} }}, stream {{ {StreamModel.BasicFields} }}";

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
        /// The moderators of the channel.
        /// </summary>
        public GraphQLEdgeArrayModel<ChannelModeratorModel> moderators { get; set; } = new GraphQLEdgeArrayModel<ChannelModeratorModel>();

        /// <summary>
        /// The user of the channel.
        /// </summary>
        public UserModel streamer { get; set; }

        /// <summary>
        /// The current stream of the channel.
        /// </summary>
        public StreamModel stream { get; set; }

        /// <summary>
        /// When the channel was last updated.
        /// </summary>
        public string updatedAt { get; set; }

        /// <summary>
        /// Whether the channel is live.
        /// </summary>
        [JsonIgnore]
        public bool IsLive { get { return string.Equals(this.status, "LIVE", StringComparison.OrdinalIgnoreCase); } }
    }
}
