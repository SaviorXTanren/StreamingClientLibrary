using Glimesh.Base.Models.Users;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

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
        public static readonly string BasicFields = $"id, language, status, thumbnail, title, category {{ {CategoryModel.AllFields} }} updatedAt";

        /// <summary>
        /// Basic fields with streamer for a GraphQL query.
        /// </summary>
        public static readonly string BasicFieldsWithStreamer = $"{ChannelModel.BasicFields}, streamer {{ {UserModel.BasicFields} }}";

        /// <summary>
        /// All fields for a GraphQL query.
        /// </summary>
        public static readonly string AllFields = $"{ChannelModel.BasicFieldsWithStreamer}, stream {{ {StreamModel.BasicFields} }}, bans {{ {ChannelBanModel.AllFields} }}";

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

        /// <summary>
        /// The banned and timed out users of the channel.
        /// </summary>
        public List<ChannelBanModel> bans { get; set; } = new List<ChannelBanModel>();

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
