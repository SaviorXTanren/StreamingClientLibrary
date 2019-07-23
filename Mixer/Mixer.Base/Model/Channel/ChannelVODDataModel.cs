namespace Mixer.Base.Model.Channel
{
    /// <summary>
    /// Format-specific information about the VOD.
    /// </summary>
    public class ChannelVODDataModel
    {
        /// <summary>
        /// Present for hls, raw, dash and thumbnail.
        /// </summary>
        public uint Width { get; set; }
        /// <summary>
        /// Present for hls, raw, dash and thumbnail.
        /// </summary>
        public uint Height { get; set; }
        /// <summary>
        /// Present for hls, raw and dash.
        /// </summary>
        public uint? Fps { get; set; }
        /// <summary>
        /// Present for hls, raw and dash.
        /// </summary>
        public uint? Bitrate { get; set; }
    }
}
