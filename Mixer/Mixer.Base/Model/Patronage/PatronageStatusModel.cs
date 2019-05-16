using System;

namespace Mixer.Base.Model.Patronage
{
    /// <summary>
    /// The patronage status of a channel.
    /// </summary>
    public class PatronageStatusModel
    {
        /// <summary>
        /// The time period ID for the patronage.
        /// </summary>
        public Guid patronagePeriodId { get; set; }
        /// <summary>
        /// The ID of the current milestone group.
        /// </summary>
        public uint currentMilestoneGroupId { get; set; }
        /// <summary>
        /// The ID of the current milestone in the group.
        /// </summary>
        public uint currentMilestoneId { get; set; }
        /// <summary>
        /// The total amount of patronage earned.
        /// </summary>
        public uint patronageEarned { get; set; }
    }
}
