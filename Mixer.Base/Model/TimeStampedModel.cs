using System;

namespace Mixer.Base.Model
{
    public class TimeStampedModel
    {
        public DateTimeOffset? createdAt { get; set; }
        public DateTimeOffset? updatedAt { get; set; }
        public DateTimeOffset? deletedAt { get; set; }
    }
}
