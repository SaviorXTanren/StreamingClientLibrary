using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Mixer.Base.Model.User
{
    public class UserLogModel
    {
        public uint id { get; set; }
        public uint userId { get; set; }
        [JsonProperty("event")]
        public string eventId { get; set; }
        public JObject eventData { get; set; }
        public string source { get; set; }
        public JObject sourceData { get; set; }
        public DateTimeOffset createdAt { get; set; }
    }
}
