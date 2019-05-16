using Newtonsoft.Json;

namespace Twitch.Base.Models.V5.Chat
{
    /// <summary>
    /// Information about a chat room.
    /// </summary>
    public class ChatRoomModel
    {
        /// <summary>
        /// The ID of the chat room.
        /// </summary>
        [JsonProperty("_id")]
        public string id { get; set; }
        /// <summary>
        /// The ID of the owner of the chat room.
        /// </summary>
        public string owner_id { get; set; }
        /// <summary>
        /// The name of the chat room.
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// The topic of the chat room.
        /// </summary>
        public string topic { get; set; }
        /// <summary>
        /// If the chat room is previewable.
        /// </summary>
        public bool is_previewable { get; set; }
        /// <summary>
        /// The minimum role that is allowed to join the chat room.
        /// </summary>
        public string minimum_allowed_role { get; set; }
    }
}
