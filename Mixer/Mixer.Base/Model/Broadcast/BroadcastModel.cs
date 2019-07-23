using System;

namespace Mixer.Base.Model.Broadcast
{
    /// <summary>
    /// A broadcast represents a single broadcast from a Channel.
    /// </summary>
    public class BroadcastModel
    {
        /// <summary>
        /// Unique ID for this broadcast.
        /// </summary>
        public Guid id { get; set; }
        /// <summary>
        /// ID of the Channel this broadcast belongs to.
        /// </summary>
        public uint channelId { get; set; }
        /// <summary>
        /// True if this broadcast is online and in progress.
        /// </summary>
        public bool online { get; set; }
        /// <summary>
        /// True if this broadcast is running in test stream mode.
        /// </summary>
        public bool isTestStream { get; set; }
        /// <summary>
        /// The date that this broadcast started at.
        /// </summary>
        public DateTimeOffset startedAt { get; set; }
    }
}
