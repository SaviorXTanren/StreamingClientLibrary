using StreamingClient.Base.Util;
using System.Runtime.Serialization;

namespace Twitch.Extensions.Base.Models
{
    /// <summary>
    /// The configuration data for an extension.
    /// </summary>
    [DataContract]
    public class ConfigurationModel
    {
        public const string GlobalConfigurationSegmentValue = "global";
        public const string DeveloperConfigurationSegmentValue = "developer";
        public const string BroadcasterConfigurationSegmentValue = "broadcaster";

        /// <summary>
        /// The segment type of the configuration data.
        /// </summary>
        [DataMember]
        public string segment { get; set; }
        /// <summary>
        /// The version of the configuration data.
        /// </summary>
        [DataMember]
        public string version { get; set; }     
        /// <summary>
        /// The content of the configuration.
        /// </summary>
        [DataMember]
        public string content { get; set; }

        /// <summary>
        /// Creates a new instance of the ConfigurationModel class.
        /// </summary>
        public ConfigurationModel() { }

        /// <summary>
        /// Creates a new instance of the ConfigurationModel class.
        /// </summary>
        /// <param name="segment">The segment type of the configuration data</param>
        /// <param name="version">The version of the configuration data</param>
        /// <param name="content">The content of the configuration</param>
        public ConfigurationModel(string segment, string version, string content)
        {
            this.segment = segment;
            this.version = version;
            this.content = content;
        }

        internal ConfigurationModel(ConfigurationResultModel config) : this(config.segment.channel_id, config.record.version, config.record.content) { }

        public T GetContent<T>()
        {
            if (!string.IsNullOrEmpty(this.content))
            {
                return JSONSerializerHelper.DeserializeFromString<T>(this.content);
            }
            return default(T);
        }
    }

    /// <summary>
    /// The channel configuration data for an extension.
    /// </summary>
    [DataContract]
    public class ChannelConfigurationModel : ConfigurationModel
    {
        /// <summary>
        /// The ID of the channel of the configuration data.
        /// </summary>
        [DataMember]
        public string channel_id { get; set; }

        /// <summary>
        /// Creates a new instance of the ChannelConfigurationModel class.
        /// </summary>
        public ChannelConfigurationModel() : base() { }

        /// <summary>
        /// Creates a new instance of the ChannelConfigurationModel class.
        /// </summary>
        /// <param name="channelID">he ID of the channel of the configuration data</param>
        /// <param name="segment">The segment type of the configuration data</param>
        /// <param name="version">The version of the configuration data</param>
        /// <param name="content">The content of the configuration</param>
        public ChannelConfigurationModel(string channelID, string segment, string version, string content)
            : base(segment, version, content)
        {
            this.channel_id = channelID;
        }

        internal ChannelConfigurationModel(ConfigurationResultModel config) : this(config.segment.channel_id, config.segment.segment_type, config.record.version, config.record.content) { }
    }

    [DataContract]
    internal class ConfigurationResultModel
    {
        [DataContract]
        internal class ConfigurationSegmentResultModel
        {
            [DataMember]
            public string segment_type { get; set; }
            [DataMember]
            public string channel_id { get; set; }
        }

        [DataMember]
        public ConfigurationSegmentResultModel segment { get; set; }

        [DataContract]
        internal class ConfigurationRecordResultModel
        {
            [DataMember]
            public string content { get; set; }
            [DataMember]
            public string version { get; set; }
        }

        [DataMember]
        public ConfigurationRecordResultModel record { get; set; }
    }
}
