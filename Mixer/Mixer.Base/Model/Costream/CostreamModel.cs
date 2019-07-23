using System;

namespace Mixer.Base.Model.Costream
{
    /// <summary>
    /// A co-stream is a set of channels grouped together and displayed on Mixer together.
    /// </summary>
    public class CostreamModel
    {
        /// <summary>
        /// The members of the costream. The first member in this array is the "host" and has some special permissions regarding kicking other members and changing layout.
        /// </summary>
        public CostreamChannelModel[] channels { get; set; }
        /// <summary>
        /// The time the costream started.
        /// </summary>
        public DateTimeOffset startedAt { get; set; }
        /// <summary>
        /// The total number of people who may join this costream.
        /// </summary>
        public uint capacity { get; set; }
        /// <summary>
        /// The layout of a costream, configurable by the host or inviter.
        /// </summary>
        public PlayerLayoutModel layout { get; set; }
    }
}
