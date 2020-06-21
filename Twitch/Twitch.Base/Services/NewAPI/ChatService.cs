using Newtonsoft.Json.Linq;
using StreamingClient.Base.Util;
using System.Collections.Generic;
using System.Threading.Tasks;
using Twitch.Base.Models.NewAPI.Chat;
using Twitch.Base.Models.NewAPI.Users;

namespace Twitch.Base.Services.NewAPI
{
    /// <summary>
    /// The APIs for Chat-based services.
    /// </summary>
    public class ChatService : NewTwitchAPIServiceBase
    {
        /// <summary>
        /// Creates an instance of the ChatService.
        /// </summary>
        /// <param name="connection">The Twitch connection to use</param>
        public ChatService(TwitchConnection connection) : base(connection) { }

        /// <summary>
        /// Gets the chat badges for the specified channel.
        /// </summary>
        /// <param name="channel">The channel to get chat badges for</param>
        /// <returns>The chat badges for the channel</returns>
        public async Task<IEnumerable<ChatBadgeSetModel>> GetChannelChatBadges(UserModel channel)
        {
            Validator.ValidateVariable(channel, "channel");
            return this.ProcessChatBadges(await this.GetJObjectAsync(string.Format("https://badges.twitch.tv/v1/badges/channels/{0}/display?language=en", channel.id)));
        }

        /// <summary>
        /// Gets the chat badges available globally.
        /// </summary>
        /// <returns>The global chat badges</returns>
        public async Task<IEnumerable<ChatBadgeSetModel>> GetGlobalChatBadges()
        {
            return this.ProcessChatBadges(await this.GetJObjectAsync("https://badges.twitch.tv/v1/badges/global/display?language=en"));
        }

        private IEnumerable<ChatBadgeSetModel> ProcessChatBadges(JObject jobj)
        {
            List<ChatBadgeSetModel> results = new List<ChatBadgeSetModel>();
            if (jobj.ContainsKey("badge_sets"))
            {
                jobj = (JObject)jobj["badge_sets"];
                foreach (var setKVP in jobj)
                {
                    ChatBadgeSetModel set = new ChatBadgeSetModel()
                    {
                        id = setKVP.Key
                    };

                    JObject setJObj = (JObject)setKVP.Value;
                    if (setJObj.ContainsKey("versions"))
                    {
                        setJObj = (JObject)setJObj["versions"];
                        foreach (var versionKVP in setJObj)
                        {
                            ChatBadgeModel badge = versionKVP.Value.ToObject<ChatBadgeModel>();
                            badge.versionID = versionKVP.Key;
                            set.versions[badge.versionID] = badge;
                        }
                    }

                    results.Add(set);
                }
            }
            return results;
        }
    }
}
