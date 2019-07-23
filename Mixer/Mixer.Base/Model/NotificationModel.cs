using Newtonsoft.Json.Linq;
using System;

namespace Mixer.Base.Model
{
    /// <summary>
    /// A single notification addressed to a user.
    /// </summary>
    public class NotificationModel
    {
        /// <summary>
        /// The user ID that this notification belongs to.
        /// </summary>
        public uint userId { get; set; }
        /// <summary>
        /// The time at which the notification was sent.
        /// </summary>
        public DateTimeOffset sentAt { get; set; }
        /// <summary>
        /// The event that triggered the notification.
        /// </summary>
        public string trigger { get; set; }
        /// <summary>
        /// An generally unstructured object containing information about the event. Events of the same type will share the same structure.
        /// </summary>
        public JObject payload { get; set; }
    }
}
