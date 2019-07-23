namespace Mixer.Base.Model.Channel
{
    /// <summary>
    /// A VOD is a recorded broadcast which can be watched again on Mixer.
    /// </summary>
    public class ChannelVODModel : TimeStampedModel
    {
        /// <summary>
        /// The unique ID of the VOD.
        /// </summary>
        public uint id { get; set; }
        /// <summary>
        /// The base URL of a VOD
        /// </summary>
        public string baseUrl { get; set; }
        /// <summary>
        /// The format of the recording.
        /// </summary>
        public string format { get; set; }
        /// <summary>
        /// Format-specific information about the VOD. Is null when type is chat.
        /// </summary>
        public ChannelVODDataModel data { get; set; }
        /// <summary>
        /// Id of the parent recording.
        /// </summary>
        public uint recordingId { get; set; }
    }
}
