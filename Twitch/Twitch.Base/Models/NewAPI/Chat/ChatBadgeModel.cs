using System.Collections.Generic;

namespace Twitch.Base.Models.NewAPI.Chat
{
    /// <summary>
    /// Information about a set of chat badges.
    /// </summary>
    public class ChatBadgeSetModel
    {
        /// <summary>
        /// The id of the set of chat badges.
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// The versions of the chat badges.
        /// </summary>
        public Dictionary<string, ChatBadgeModel> versions { get; set; } = new Dictionary<string, ChatBadgeModel>();
    }

    /// <summary>
    /// Information about a chat badge.
    /// </summary>
    public class ChatBadgeModel
    {
        /// <summary>
        /// The version ID of the chat badge.
        /// </summary>
        public string versionID { get; set; }

        /// <summary>
        /// The title of the chat badge.
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// The description of the chat badge.
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// The 1x size image of the chat badge.
        /// </summary>
        public string image_url_1x { get; set; }

        /// <summary>
        /// The 2x size image of the chat badge.
        /// </summary>
        public string image_url_2x { get; set; }

        /// <summary>
        /// The 4x size image of the chat badge.
        /// </summary>
        public string image_url_4x { get; set; }
    }
}
