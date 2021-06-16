using Newtonsoft.Json.Linq;

namespace Twitch.Base.Models.NewAPI.Chat
{
    /// <summary>
    /// Information about a chat emote.
    /// </summary>
    public class ChatEmoteModel
    {
        private const string imageUrlSize1 = "url_1x";
        private const string imageUrlSize2 = "url_2x";
        private const string imageUrlSize4 = "url_4x";

        /// <summary>
        /// The ID of the emote.
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// The name of the emote.
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// The images for the emote.
        /// </summary>
        public JObject images { get; set; } = new JObject();

        /// <summary>
        /// The subscription tier required for this emote if it's a subscription emote.
        /// </summary>
        public string tier { get; set; }

        /// <summary>
        /// The type of emote. The most common values for custom channel emotes are:
        /// 
        /// subscriptions: Indicates a custom subscriber emote.
        /// 
        /// bitstier: Indicates a custom Bits tier emote.
        /// 
        /// follower: Indicates a custom follower emote.
        /// </summary>
        public string emote_type { get; set; }

        /// <summary>
        /// The ID of the set that this emote belongs to.
        /// </summary>
        public string emote_set_id { get; set; }

        /// <summary>
        /// The ID of the broadcaster that this emote belongs to.
        /// </summary>
        public string owner_id { get; set; }

        /// <summary>
        /// The size 1 image url of the emote.
        /// </summary>
        public string Size1URL { get { return this.images.ContainsKey(imageUrlSize1) ? this.images[imageUrlSize1].ToString() : null; } }
        /// <summary>
        /// The size 2 image url of the emote.
        /// </summary>
        public string Size2URL { get { return this.images.ContainsKey(imageUrlSize2) ? this.images[imageUrlSize2].ToString() : null; } }
        /// <summary>
        /// The size 4 image url of the emote.
        /// </summary>
        public string Size4URL { get { return this.images.ContainsKey(imageUrlSize4) ? this.images[imageUrlSize4].ToString() : null; } }
    }
}
