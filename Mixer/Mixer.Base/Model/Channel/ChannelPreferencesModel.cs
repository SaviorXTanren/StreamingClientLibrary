using Newtonsoft.Json;

namespace Mixer.Base.Model.Channel
{
    /// <summary>
    /// Channel preferences are a list of options and attributes which control behaviour for the channel. Please see each property for more details.
    /// </summary>
    public class ChannelPreferencesModel
    {
        /// <summary>
        /// The text used when sharing the stream. The template parameter %URL% will be replaced with the channel's URL. The template parameter %USER% will be replaced with the channel's name.
        /// </summary>
        public string sharetext { get; set; }
        /// <summary>
        /// Specifies whether links are allowed in the chat.
        /// </summary>
        [JsonProperty("channel:links:allowed")]
        public bool? linkedAllowed { get; set; }
        /// <summary>
        /// Specifies whether links are clickable in the chat.
        /// </summary>
        [JsonProperty("channel:links:clickable")]
        public bool? linksClickable { get; set; }
        /// <summary>
        /// Interval required between each chat message.
        /// </summary>
        [JsonProperty("channel:slowchat")]
        public int? slowChat { get; set; }
        /// <summary>
        /// The message to be sent when a user completes a direct purchase on the channel. The template parameter %USER% will be replaced with the name of the user who completed direct purchase. The template parameter %CHANNEL% will be replaced with the name of the channel. The template parameter %GAMETITLE% will be replaced with the name of the game purchased.
        /// </summary>
        [JsonProperty("channel:notify:directPurchaseMessage")]
        public string notifydirectPurchaseMessage { get; set; }
        /// <summary>
        /// The message to be sent when a user subscribes to the channel. The template parameter %USER% will be replaced with the subscriber's name.
        /// </summary>
        [JsonProperty("channel:notify:subscribemessage")]
        public string notifySubscribeMessage { get; set; }
        /// <summary>
        /// Indicates whether a notification should be shown upon subscription.
        /// </summary>
        [JsonProperty("channel:notify:subscribe")]
        public bool notifySubscribe { get; set; }
        /// <summary>
        /// The message to be sent when a user follows the channel. The template parameter "%USER%" will be replaced with the follower's name.
        /// </summary>
        [JsonProperty("channel:notify:followmessage")]
        public string notifyFollowMessage { get; set; }
        /// <summary>
        /// Indicates whether a notification should be shown upon follow.
        /// </summary>
        [JsonProperty("channel:notify:follow")]
        public bool? notifyFollow { get; set; }
        /// <summary>
        /// The message to be sent when a user hosts the channel. The template parameter "%USER%" will be replaced with the hoster's name.
        /// </summary>
        [JsonProperty("channel:notify:hostedBy")]
        public string notifyHostedBy { get; set; }
        /// <summary>
        /// The message to be sent when the channel hosts another. The template parameter "%USER%" will be replaced with the hostee's name.
        /// </summary>
        [JsonProperty("channel:notify:hosting")]
        public string notifyHosting { get; set; }
        /// <summary>
        /// The text to be added to the subscription email.
        /// </summary>
        [JsonProperty("channel:partner:submail")]
        public string partnerSubmail { get; set; }
        /// <summary>
        /// Indicates whether to mute when the streamer opens his own stream.
        /// </summary>
        [JsonProperty("channel:player:muteOwn")]
        public bool? playerMuteOwn { get; set; }
        /// <summary>
        /// Indicates whether the tweet button should be shown.
        /// </summary>
        [JsonProperty("channel:tweet:enabled")]
        public bool? tweetEnabled { get; set; }
        /// <summary>
        /// The message to be used when a user tweets about the channel. The template parameter %URL% will be replaced with the share url.
        /// </summary>
        [JsonProperty("channel:tweet:body")]
        public string tweetBody { get; set; }
        /// <summary>
        /// Indicates if the channel allows HypeZone to host it.
        /// </summary>
        [JsonProperty("hypezone:allow")]
        public string hypezoneAllow { get; set; }
        /// <summary>
        /// Indicates if the channel allows other channels to host it.
        /// </summary>
        [JsonProperty("hosting:allow")]
        public string hostingAllow { get; set; }
        /// <summary>
        /// Allows other streamers to join you in a costream.
        /// </summary>
        [JsonProperty("costream:allow")]
        public string costreamAllow { get; set; }
        /// <summary>
        /// When a user visits the channel while the channel is offline, the most recent VOD will be automatically played if this preference is enabled.
        /// </summary>
        [JsonProperty("channel:offline:autoplayVod")]
        public string offlineAutoplayVod { get; set; }
    }
}
