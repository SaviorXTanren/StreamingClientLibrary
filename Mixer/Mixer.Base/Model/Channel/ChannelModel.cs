using System;

namespace Mixer.Base.Model.Channel
{
    /// <summary>
    /// A single channel within Mixer that can be updated. Each channel is owned by a user, and a channel can be broadcasted to.
    /// </summary>
    public class ChannelUpdateableModel : TimeStampedModel
    {
        /// <summary>
        /// The id of the transcoding profile.
        /// </summary>
        public uint? transcodingProfileId { get; set; }
        /// <summary>
        /// The title of the channel.
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// The target audience of the channel.
        /// </summary>
        public string audience { get; set; }
        /// <summary>
        /// The description of the channel, can contain HTML.
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// The ID of the game type.
        /// </summary>
        public uint? typeId { get; set; }
        /// <summary>
        /// Indicates if that channel is interactive.
        /// </summary>
        public bool interactive { get; set; }
        /// <summary>
        /// The ID of the interactive game used.
        /// </summary>
        public uint? interactiveGameId { get; set; }
        /// <summary>
        /// Indicates if the channel has vod recording enabled.
        /// </summary>
        public bool vodsEnabled { get; set; }
    }

    /// <summary>
    /// A single channel within Mixer. Each channel is owned by a user, and a channel can be broadcasted to.
    /// </summary>
    public class ChannelModel : ChannelUpdateableModel
    {
        /// <summary>
        /// The unique ID of the channel.
        /// </summary>
        public uint id { get; set; }
        /// <summary>
        /// The ID of the user owning the channel.
        /// </summary>
        public uint userId { get; set; }
        /// <summary>
        /// The name and url of the channel.
        /// </summary>
        public string token { get; set; }
        /// <summary>
        /// Indicates if the channel is streaming.
        /// </summary>
        public bool online { get; set; }
        /// <summary>
        /// True if featureLevel is > 0.
        /// </summary>
        public bool featured { get; set; }
        /// <summary>
        /// The featured level for this channel. Its value controls the position and order of channels in the featured carousel.
        /// </summary>
        public int featureLevel { get; set; }
        /// <summary>
        /// Indicates if the channel is partnered.
        /// </summary>
        public bool partnered { get; set; }
        /// <summary>
        /// Indicates if the channel is suspended.
        /// </summary>
        public bool suspended { get; set; }
        /// <summary>
        /// Amount of unique viewers that ever viewed this channel.
        /// </summary>
        public uint viewersTotal { get; set; }
        /// <summary>
        /// Amount of current viewers.
        /// </summary>
        public uint viewersCurrent { get; set; }
        /// <summary>
        /// Amount of followers.
        /// </summary>
        public uint numFollowers { get; set; }
        /// <summary>
        /// The ftl stream id.
        /// </summary>
        public int? ftl { get; set; }
        /// <summary>
        /// Indicates if the channel has vod saved.
        /// </summary>
        public bool hasVod { get; set; }
        /// <summary>
        /// ISO 639 language id.
        /// </summary>
        public string languageId { get; set; }
        /// <summary>
        /// The ID of the cover resource.
        /// </summary>
        public uint? coverId { get; set; }
        /// <summary>
        /// The resource ID of the thumbnail.
        /// </summary>
        public uint? thumbnailId { get; set; }
        /// <summary>
        /// The resource ID of the subscriber badge.
        /// </summary>
        public uint? badgeId { get; set; }
        /// <summary>
        /// The URL of the the banner image.
        /// </summary>
        public string bannerUrl { get; set; }
        /// <summary>
        /// The costream that the channel is in, if any.
        /// </summary>
        public Guid? costreamId { get; set; }
        /// <summary>
        /// The ID of the hostee channel.
        /// </summary>
        public uint? hosteeId { get; set; }
    }
}
