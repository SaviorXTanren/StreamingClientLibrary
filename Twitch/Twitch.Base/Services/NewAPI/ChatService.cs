using Newtonsoft.Json.Linq;
using StreamingClient.Base.Util;
using StreamingClient.Base.Web;

using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// Gets all global emotes.
        /// </summary>
        /// <returns>The global emotes</returns>
        public async Task<IEnumerable<ChatEmoteModel>> GetGlobalEmotes()
        {
            return await this.GetDataResultAsync<ChatEmoteModel>("chat/emotes/global");
        }

        /// <summary>
        /// Gets the emotes for the specified channel.
        /// </summary>
        /// <param name="channel">The channel to get emotes for</param>
        /// <returns>The emotes for the channel</returns>
        public async Task<IEnumerable<ChatEmoteModel>> GetChannelEmotes(UserModel channel)
        {
            Validator.ValidateVariable(channel, "channel");
            return await this.GetChannelEmotes(channel.id);
        }

        /// <summary>
        /// Gets the emotes for the specified channel ID.
        /// </summary>
        /// <param name="channelID">The channel ID to get emotes for</param>
        /// <returns>The emotes for the channel</returns>
        public async Task<IEnumerable<ChatEmoteModel>> GetChannelEmotes(string channelID)
        {
            Validator.ValidateString(channelID, "channelID");
            return await this.GetDataResultAsync<ChatEmoteModel>("chat/emotes?broadcaster_id=" + channelID);
        }

        /// <summary>
        /// Gets all global emotes.
        /// </summary>
        /// <returns>The global emotes</returns>
        public async Task<IEnumerable<ChatEmoteModel>> GetEmoteSets(IEnumerable<string> emoteSetIDs)
        {
            Validator.ValidateList(emoteSetIDs, "emoteSetIDs");

            List<ChatEmoteModel> results = new List<ChatEmoteModel>();

            for (int i = 0; i < emoteSetIDs.Count(); i = i + 10)
            {
                results.AddRange(await this.GetDataResultAsync<ChatEmoteModel>("chat/emotes/set?" + string.Join("&", emoteSetIDs.Skip(i).Take(10).Select(id => "emote_set_id=" + id))));
            }

            return results;
        }

        /// <summary>
        /// Sends an announcement to the broadcaster’s chat room.
        /// <param name="channelID">The channel ID send the announcement to</param>
        /// <param name="announcement">The announcement data to send</param>
        /// </summary>
        public async Task SendChatAnnouncement(string channelID, AnnouncementModel announcement)
        {
            Validator.ValidateString(channelID, "channelID");

            await this.PostAsync("chat/announcements?broadcaster_id=" + channelID + "&moderator_id=" + channelID, AdvancedHttpClient.CreateContentFromObject(announcement));
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
