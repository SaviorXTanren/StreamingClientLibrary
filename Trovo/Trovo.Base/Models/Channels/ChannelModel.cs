using StreamingClient.Base.Util;

namespace Trovo.Base.Models.Channels
{
    /// <summary>
    /// The audiences for a channel.
    /// </summary>
    public enum ChannelAudienceTypeEnum
    {
        /// <summary>
        /// All
        /// </summary>
        CHANNEL_AUDIENCE_TYPE_FAMILYFRIENDLY,
        /// <summary>
        /// 13+
        /// </summary>
        CHANNEL_AUDIENCE_TYPE_TEEN,
        /// <summary>
        /// 18+
        /// </summary>
        CHANNEL_AUDIENCE_TYPE_EIGHTEENPLUS
    }

    /// <summary>
    /// Information about a channel.
    /// </summary>
    public class ChannelModel
    {
        /// <summary>
        /// The ID of the channel.
        /// </summary>
        public string channel_id { get; set; }
        /// <summary>
        /// The profile information of the channel.
        /// </summary>
        public string streamer_info { get; set; }
        /// <summary>
        /// The profile picture of the channel.
        /// </summary>
        public string profile_pic { get; set; }
        /// <summary>
        /// The URL of the channel.
        /// </summary>
        public string channel_url { get; set; }
        /// <summary>
        /// The language code of the channel.
        /// </summary>
        public string language_code { get; set; }
        /// <summary>
        /// The audience type of the channel.
        /// </summary>
        public string audi_type { get; set; }
        /// <summary>
        /// Whether the channel is currently live.
        /// </summary>
        public bool is_live { get; set; }
        /// <summary>
        /// The title of the channel.
        /// </summary>
        public string live_title { get; set; }
        /// <summary>
        /// The current number of viewers for the channel.
        /// </summary>
        public long current_viewers { get; set; }
        /// <summary>
        /// The current number of followers for the channel.
        /// </summary>
        public long followers { get; set; }
        /// <summary>
        /// The ID of the category for the channel.
        /// </summary>
        public string category_id { get; set; }
        /// <summary>
        /// The name of the category for the channel.
        /// </summary>
        public string category_name { get; set; }
        /// <summary>
        /// The thumbnail URL for the channel.
        /// </summary>
        public string thumbnail { get; set; }

        /// <summary>
        /// The audience enum for the channel.
        /// </summary>
        public ChannelAudienceTypeEnum Audience { get { return EnumHelper.GetEnumValueFromString<ChannelAudienceTypeEnum>(this.audi_type); } }
    }
}
