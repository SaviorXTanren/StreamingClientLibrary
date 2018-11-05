using Newtonsoft.Json.Linq;
using System;

namespace Mixer.Base.Model.Patronage
{
    /// <summary>
    /// The milestone of a patronage period.
    /// </summary>
    public class PatronageMilestoneModel
    {
        /// <summary>
        /// The ID of the milestone.
        /// </summary>
        public uint id { get; set; }
        /// <summary>
        /// The target amount for the milestone.
        /// </summary>
        public uint target { get; set; }
        /// <summary>
        /// The reward amount for the milestone.
        /// </summary>
        public uint reward { get; set; }
    }

    /// <summary>
    /// The milestones of a patronage period.
    /// </summary>
    public class PatronageMilestoneGroupModel
    {
        /// <summary>
        /// The ID of the milestone group.
        /// </summary>
        public uint id { get; set; }
        /// <summary>
        /// The initial target amount to enter this milestone.
        /// </summary>
        public uint milestoneTargetBase { get; set; }
        /// <summary>
        /// The milestones for the group.
        /// </summary>
        public PatronageMilestoneModel[] milestones { get; set; }
        /// <summary>
        /// The UI components of the milestone group.
        /// </summary>
        public JObject uiComponents { get; set; }
    }

    /// <summary>
    /// The details of a patronage period.
    /// </summary>
    public class PatronagePeriodModel
    {
        /// <summary>
        /// The time period ID for the patronage.
        /// </summary>
        public Guid id { get; set; }
        /// <summary>
        /// The start date time of the patronage period.
        /// </summary>
        public DateTimeOffset? startTime { get; set; }
        /// <summary>
        /// The end date time of the patronage period.
        /// </summary>
        public DateTimeOffset? endTime { get; set; }
        /// <summary>
        /// The milestone groups for the patronage period.
        /// </summary>
        public PatronageMilestoneGroupModel[] milestoneGroups { get; set; }
    }
}
