using Newtonsoft.Json.Linq;
using StreamingClient.Base.Util;
using System.Collections.Generic;
using System.Threading.Tasks;
using Twitch.Base.Models.V5.Channel;
using Twitch.Base.Models.V5.Chat;
using Twitch.Base.Models.V5.Emotes;

namespace Twitch.Base.Services.V5API
{
    /// <summary>
    /// The APIs for Chat-based services.
    /// </summary>
    public class ChatService : V5APIServiceBase
    {
        /// <summary>
        /// Creates an instance of the ChatService.
        /// </summary>
        /// <param name="connection">The Twitch connection to use</param>
        public ChatService(TwitchConnection connection) : base(connection) { }

        /// <summary>
        /// Gets the chat badges for a channel.
        /// </summary>
        /// <param name="channel">The channel to get chat badges for</param>
        /// <returns>The chat badges</returns>
        public async Task<ChannelChatBadgesModel> GetChannelChatBadges(ChannelModel channel)
        {
            Validator.ValidateVariable(channel, "channel");
            return await this.GetAsync<ChannelChatBadgesModel>("chat/" + channel.id + "/badges");
        }

        /// <summary>
        /// Gets all chat emoticons (not including their images).
        /// 
        /// Caution: This endpoint returns a large amount of data.
        /// </summary>
        /// <returns>The chat badges</returns>
        public async Task<IEnumerable<EmoteModel>> GetChatEmoticons()
        {
            JObject jobj = await this.GetJObjectAsync("chat/emoticon_images");
            List<EmoteModel> results = new List<EmoteModel>();
            if (jobj != null && jobj.ContainsKey("emoticons"))
            {
                JArray emoticons = (JArray)jobj["emoticons"];
                foreach (JObject emoticon in emoticons)
                {
                    EmoteModel emote = emoticon.ToObject<EmoteModel>();
                    emote.setID = emoticon["emoticon_set"].ToString();
                    results.Add(emote);
                }
            }
            return results;
        }

        /// <summary>
        /// Gets all chat emoticons (not including their images) in one or more specified sets.
        /// </summary>
        /// <returns>The chat badges</returns>
        public async Task<IEnumerable<EmoteModel>> GetChatEmoticonsForSet(List<int> sets)
        {
            Validator.ValidateList(sets, "sets");

            JObject jobj = await this.GetJObjectAsync("chat/emoticon_images?emotesets=" + string.Join(",", sets));
            List<EmoteModel> results = new List<EmoteModel>();
            if (jobj != null && jobj.ContainsKey("emoticon_sets"))
            {
                JObject emoticonSets = (JObject)jobj["emoticon_sets"];
                foreach (var kvp in emoticonSets)
                {
                    string setID = kvp.Key;
                    foreach (JToken token in (JArray)kvp.Value)
                    {
                        EmoteModel emote = token.ToObject<EmoteModel>();
                        emote.setID = setID;
                        results.Add(emote);
                    }
                }
            }
            return results;
        }
    }
}
