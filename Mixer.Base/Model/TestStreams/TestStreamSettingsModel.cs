using System;

namespace Mixer.Base.Model.TestStreams
{
    public class TestStreamSettingsModel
    {
        public uint id { get; set; }
        public bool? isActive { get; set; }
        public int hoursQuota { get; set; }
        public int hoursRemaining { get; set; }
        public uint hoursResetIntervalInDays { get; set; }
        public DateTimeOffset? hoursLastReset { get; set; }
    }
}
