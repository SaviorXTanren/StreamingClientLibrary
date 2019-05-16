using Newtonsoft.Json.Linq;
using System;

namespace Mixer.Base.Model
{
    public class NotificationModel
    {
        public uint userId { get; set; }
        public DateTimeOffset sentAt { get; set; }
        public string trigger { get; set; }
        public JObject payload { get; set; }
    }
}
