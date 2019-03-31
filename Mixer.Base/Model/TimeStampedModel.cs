using System;

namespace Mixer.Base.Model
{
    /// <summary>
    /// Timestamp information for a model.
    /// </summary>
    public class TimeStampedModel
    {
        /// <summary>
        /// The date the model was created.
        /// </summary>
        public DateTimeOffset? createdAt { get; set; }
        /// <summary>
        /// The date the model was last updated.
        /// </summary>
        public DateTimeOffset? updatedAt { get; set; }
        /// <summary>
        /// The date the model was deleted.
        /// </summary>
        public DateTimeOffset? deletedAt { get; set; }
    }
}
