namespace Mixer.Base.Model.Costream
{
    /// <summary>
    /// The members of the costream.
    /// </summary>
    public class CostreamChannelModel
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
    }
}
