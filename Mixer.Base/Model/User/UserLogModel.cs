using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Mixer.Base.Model.User
{
    /// <summary>
    /// Log information for a user.
    /// </summary>
    public class UserLogModel
    {
        /// <summary>
        /// The ID of the log.
        /// </summary>
        public uint id { get; set; }
        /// <summary>
        /// The user's ID.
        /// </summary>
        public uint userId { get; set; }
        /// <summary>
        /// The event ID.
        /// </summary>
        [JsonProperty("event")]
        public string eventId { get; set; }
        /// <summary>
        /// The event data.
        /// </summary>
        public JObject eventData { get; set; }
        /// <summary>
        /// The source of the event.
        /// </summary>
        public string source { get; set; }
        /// <summary>
        /// Data for the source.
        /// </summary>
        public JObject sourceData { get; set; }
        /// <summary>
        /// The date the log was created.
        /// </summary>
        public DateTimeOffset createdAt { get; set; }
    }
}
