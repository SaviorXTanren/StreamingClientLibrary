using Newtonsoft.Json.Linq;

namespace Twitch.Base.Models.Clients.PubSub.Messages
{
    /// <summary>
    /// Information about a user whisper.
    /// </summary>
    public class PubSubWhisperEventModel
    {
        /// <summary>
        /// The type of message.
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// The data of the message.
        /// </summary>
        public JObject data { get; set; }
        /// <summary>
        /// The ID
        /// </summary>
        public string thread_id { get; set; }
        /// <summary>
        /// The contents of the message.
        /// </summary>
        public string body { get; set; }
        /// <summary>
        /// The timestamp of when the message was sent.
        /// </summary>
        public long sent_ts { get; set; }
        /// <summary>
        /// The ID of the user who sent the message.
        /// </summary>
        public long from_id { get; set; }
        /// <summary>
        /// The tags of the message.
        /// </summary>
        public JObject tags { get; set; }
        /// <summary>
        /// Information about the recipient.
        /// </summary>
        public JObject recipient { get; set; }
        /// <summary>
        /// The nonce tracker ID.
        /// </summary>
        public string nonce { get; set; }
    }
}
