using Newtonsoft.Json.Linq;

namespace Twitch.Base.Models.Clients.PubSub.Messages
{
    /// <summary>
    /// Information about a Commerce purchase.
    /// </summary>
    public class PubSubCommerceEventModel
    {
        /// <summary>
        /// The ID of the user who made the purchase.
        /// </summary>
        public string user_id { get; set; }
        /// <summary>
        /// The name of the user who made the purchase.
        /// </summary>
        public string user_name { get; set; }
        /// <summary>
        /// The display name of the user who made the purchase.
        /// </summary>
        public string display_name { get; set; }
        /// <summary>
        /// The ID of the channel the purchase took place in.
        /// </summary>
        public string channel_id { get; set; }
        /// <summary>
        /// The name of the channel the purchase took place in.
        /// </summary>
        public string channel_name { get; set; }
        /// <summary>
        /// The time the purchase took place.
        /// </summary>
        public string time { get; set; }
        /// <summary>
        /// The url to the image of the item.
        /// </summary>
        public string item_image_url { get; set; }
        /// <summary>
        /// The item description.
        /// </summary>
        public string item_description { get; set; }
        /// <summary>
        /// Whether the purchase supports the channel.
        /// </summary>
        public bool supports_channel { get; set; }
        /// <summary>
        /// Information about the message with the purchase.
        /// </summary>
        public JObject purchase_message { get; set; }
    }
}
