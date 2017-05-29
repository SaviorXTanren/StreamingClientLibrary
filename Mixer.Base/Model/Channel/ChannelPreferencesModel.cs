using Newtonsoft.Json;

namespace Mixer.Base.Model.Channel
{
    public class ChannelPreferencesModel
    {
        public string sharetext { get; set; }
        [JsonProperty("channel:links:allowed")]
        public bool? linkedAllowed { get; set; }
        [JsonProperty("channel:links:clickable")]
        public bool? linksClickable { get; set; }
        [JsonProperty("channel:slowchat")]
        public int? slowChat { get; set; }
        [JsonProperty("channel:notify:subscribemessage")]
        public string notifySubscribeMessage { get; set; }
        [JsonProperty("channel:notify:subscribe")]
        public bool notifySubscribe { get; set; }
        [JsonProperty("channel:notify:followmessage")]
        public string notifyFollowMessage { get; set; }
        [JsonProperty("channel:notify:follow")]
        public bool? notifyFollow { get; set; }
        [JsonProperty("channel:notify:hostedBy")]
        public string notifyHostedBy { get; set; }
        [JsonProperty("channel:notify:hosting")]
        public string notifyHosting { get; set; }
        [JsonProperty("channel:partner:submail")]
        public string partnerSubmail { get; set; }
        [JsonProperty("channel:player:muteOwn")]
        public bool? playerMuteOwn { get; set; }
        [JsonProperty("channel:tweet:enabled")]
        public bool? tweetEnabled { get; set; }
        [JsonProperty("channel:tweet:body")]
        public string tweetBody { get; set; }
        [JsonProperty("costream:allow")]
        public string costreamAllow { get; set; }
    }
}
