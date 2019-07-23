using System;
using System.Collections.Generic;

namespace Mixer.Base.Model.Channel
{
    /// <summary>
    /// Details about a recorded Broadcast from a channel.
    /// </summary>
    public class ChannelRecordingModel : TimeStampedModel
    {
        /// <summary>
        /// The unique ID of the recording.
        /// </summary>
        public uint id { get; set; }
        /// <summary>
        /// The channel ID this recording is a video of.
        /// </summary>
        public uint channelId { get; set; }
        /// <summary>
        /// The state of this recording, which may be one of:
        /// </summary>
        public string state { get; set; }
        /// <summary>
        /// The number of users who have viewed this recording.
        /// </summary>
        public uint viewsTotal { get; set; }
        /// <summary>
        /// The date this recording will be deleted at.
        /// </summary>
        public DateTime expiresAt { get; set; }
        /// <summary>
        /// The video formats available for this recording.
        /// </summary>
        public List<ChannelVODModel> vods { get; set; }
        /// <summary>
        /// Whether the current user has viewed the recording. This will generally only appear when the recording is looked up from an endpoint with user in the query string.
        /// </summary>
        public bool? viewed { get; set; }
        /// <summary>
        /// Name of the Recording, usually the title of the channel at the time of the recording unless the creator changed it.
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// Type id of the recording, usually the type id of the channel at the time of the recording, unless the creator changed it.
        /// </summary>
        public uint typeId { get; set; }
        /// <summary>
        /// Duration of the recording in seconds.
        /// </summary>
        public uint duration { get; set; }
        /// <summary>
        /// Defines if the video was seen by the user. Only present when the user context was given.
        /// </summary>
        public bool? seen { get; set; }
    }
}
