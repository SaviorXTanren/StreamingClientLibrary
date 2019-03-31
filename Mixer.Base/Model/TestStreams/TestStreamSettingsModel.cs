using System;

namespace Mixer.Base.Model.TestStreams
{
    /// <summary>
    /// Test Stream settings information.
    /// </summary>
    public class TestStreamSettingsModel
    {
        /// <summary>
        /// The ID of the test stream.
        /// </summary>
        public uint id { get; set; }
        /// <summary>
        /// Whether the test stream is active.
        /// </summary>
        public bool? isActive { get; set; }
        /// <summary>
        /// The amount of hours allowed.
        /// </summary>
        public double hoursQuota { get; set; }
        /// <summary>
        /// The amount of hours remaining.
        /// </summary>
        public double hoursRemaining { get; set; }
        /// <summary>
        /// The number of days until the hours reset.
        /// </summary>
        public uint hoursResetIntervalInDays { get; set; }
        /// <summary>
        /// The last time hours were reset.
        /// </summary>
        public DateTimeOffset? hoursLastReset { get; set; }
    }
}
