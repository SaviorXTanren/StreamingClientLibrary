using StreamingClient.Base.Util;

namespace Trovo.Base.Models.Channels
{
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

    public class ChannelModel
    {
        public string channel_id { get; set; }
        public string Streamer_info { get; set; }
        public string profile_pic { get; set; }
        public string channel_url { get; set; }
        public string language_code { get; set; }
        public string audi_type { get; set; }
        public bool Is_live { get; set; }
        public string live_title { get; set; }
        public long Current_viewers { get; set; }
        public long Followers { get; set; }
        public string category_id { get; set; }
        public string category_name { get; set; }
        public string thumbnail { get; set; }

        public ChannelAudienceTypeEnum Audience { get { return EnumHelper.GetEnumValueFromString<ChannelAudienceTypeEnum>(this.audi_type); } }
    }
}
