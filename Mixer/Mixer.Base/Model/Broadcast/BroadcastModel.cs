using System;

namespace Mixer.Base.Model.Broadcast
{
    public class BroadcastModel
    {
        public Guid id { get; set; }
        public uint channelId { get; set; }
        public bool online { get; set; }
        public bool isTestStream { get; set; }
        public DateTimeOffset startedAt { get; set; }
    }
}
