using Newtonsoft.Json.Linq;
using StreamingClient.Base.Util;
using StreamingClient.Base.Web;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Twitch.Base.Models.NewAPI;
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
        /// Sends a whisper from the broadcaster to the specified user.
        /// <param name="channelID">The channel ID to send the whisper from</param>
        /// <param name="userID">The user ID to send the whisper to</param>
        /// <param name="message">The message to whisper</param>
        /// </summary>
        public async Task SendWhisper(string channelID, string userID, string message)
        {
            Validator.ValidateString(channelID, "channelID");
            Validator.ValidateString(userID, "userID");
            Validator.ValidateString(message, "message");

            JObject jobj = new JObject();
            jobj["message"] = message;

            await this.PostAsync("whispers?from_user_id=" + channelID + "&to_user_id=" + channelID, AdvancedHttpClient.CreateContentFromObject(jobj));
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

        /// <summary>
        /// Raids the specified target channel.
        /// </summary>
        /// <param name="channelID">The channel ID to raid from</param>
        /// <param name="targetChannelID">The channel ID to raid</param>
        public async Task RaidChannel(string channelID, string targetChannelID)
        {
            Validator.ValidateString(channelID, "channelID");
            Validator.ValidateString(targetChannelID, "targetChannelID");

            await this.PostAsync("raids?from_broadcaster_id=" + channelID + "&to_broadcaster_id=" + targetChannelID, AdvancedHttpClient.CreateEmptyContent());
        }

        /// <summary>
        /// Deletes the specified message in the broadcaster's chat room.
        /// </summary>
        /// <param name="channelID">The channel ID to clear chat for</param>
        /// <param name="messageID">The message ID to delete</param>
        public async Task DeleteChatMessage(string channelID, string messageID)
        {
            Validator.ValidateString(channelID, "channelID");
            Validator.ValidateString(messageID, "messageID");

            await this.DeleteAsync("chat/announcements?broadcaster_id=" + channelID + "&moderator_id=" + channelID + "&message_id=" + messageID);
        }

        /// <summary>
        /// Clears chat for the broadcaster's chat room.
        /// </summary>
        /// <param name="channelID">The channel ID to clear chat for</param>
        public async Task ClearChat(string channelID)
        {
            Validator.ValidateString(channelID, "channelID");

            await this.DeleteAsync("moderation/chat?broadcaster_id=" + channelID + "&moderator_id=" + channelID);
        }

        /// <summary>
        /// Bans the specified user from the broadcaster's chat room.
        /// </summary>
        /// <param name="channelID">The channel ID to ban the user from</param>
        /// <param name="userID">The user ID to ban</param>
        /// <param name="duration">The duration to ban for</param>
        /// <param name="reason">The reason for the ban</param>
        public async Task TimeoutUser(string channelID, string userID, int duration, string reason)
        {
            Validator.ValidateString(channelID, "channelID");
            Validator.ValidateString(userID, "userID");
            Validator.ValidateVariable(duration, "duration");
            Validator.ValidateString(reason, "reason");

            JObject jobj = new JObject();
            jobj["user_id"] = userID;
            jobj["reason"] = reason ?? string.Empty;
            if (duration > 0)
            {
                jobj["duration"] = duration;
            }

            await this.PostAsync("moderation/bans?broadcaster_id=" + channelID + "&moderator_id=" + channelID, AdvancedHttpClient.CreateContentFromObject(jobj));
        }

        /// <summary>
        /// Untimeouts the specified user from the broadcaster's chat room.
        /// </summary>
        /// <param name="channelID">The channel ID to untimeout the user from</param>
        /// <param name="userID">The user ID to untimeout</param>
        public async Task UntimeoutUser(string channelID, string userID)
        {
            Validator.ValidateString(channelID, "channelID");
            Validator.ValidateString(userID, "userID");

            await this.DeleteAsync("moderation/bans?broadcaster_id=" + channelID + "&moderator_id=" + channelID + "&user_id=" + userID);
        }

        /// <summary>
        /// Bans the specified user from the broadcaster's chat room.
        /// </summary>
        /// <param name="channelID">The channel ID to ban the user from</param>
        /// <param name="userID">The user ID to ban</param>
        /// <param name="reason">The reason for the ban</param>
        public async Task BanUser(string channelID, string userID, string reason)
        {
            Validator.ValidateString(channelID, "channelID");
            Validator.ValidateString(userID, "userID");
            Validator.ValidateString(reason, "reason");

            await this.TimeoutUser(channelID, userID, 0, reason);
        }

        /// <summary>
        /// Unbans the specified user from the broadcaster's chat room.
        /// </summary>
        /// <param name="channelID">The channel ID to unban the user from</param>
        /// <param name="userID">The user ID to unban</param>
        public async Task UnbanUser(string channelID, string userID)
        {
            Validator.ValidateString(channelID, "channelID");
            Validator.ValidateString(userID, "userID");

            await this.UntimeoutUser(channelID, userID);
        }

        /// <summary>
        /// Mods the specified user in the broadcaster's chat room.
        /// </summary>
        /// <param name="channelID">The channel ID to mod the user for</param>
        /// <param name="userID">The user ID to mod</param>
        public async Task ModUser(string channelID, string userID)
        {
            Validator.ValidateString(channelID, "channelID");
            Validator.ValidateString(userID, "userID");

            await this.PostAsync("moderation/moderators?broadcaster_id=" + channelID + "&user_id=" + userID, AdvancedHttpClient.CreateEmptyContent());
        }

        /// <summary>
        /// Unbans the specified user from the broadcaster's chat room.
        /// </summary>
        /// <param name="channelID">The channel ID to unban the user from</param>
        /// <param name="userID">The user ID to unban</param>
        public async Task UnmodUser(string channelID, string userID)
        {
            Validator.ValidateString(channelID, "channelID");
            Validator.ValidateString(userID, "userID");

            await this.DeleteAsync("moderation/moderators?broadcaster_id=" + channelID + "&user_id=" + userID);
        }

        /// <summary>
        /// VIPs the specified user in the broadcaster's chat room.
        /// </summary>
        /// <param name="channelID">The channel ID to VIP the user for</param>
        /// <param name="userID">The user ID to VIP</param>
        public async Task VIPUser(string channelID, string userID)
        {
            Validator.ValidateString(channelID, "channelID");
            Validator.ValidateString(userID, "userID");

            await this.PostAsync("channels/vips?broadcaster_id=" + channelID + "&user_id=" + userID, AdvancedHttpClient.CreateEmptyContent());
        }

        /// <summary>
        /// Un-VIPs the specified user from the broadcaster's chat room.
        /// </summary>
        /// <param name="channelID">The channel ID to un-VIP the user from</param>
        /// <param name="userID">The user ID to un-VIP</param>
        public async Task UnVIPUser(string channelID, string userID)
        {
            Validator.ValidateString(channelID, "channelID");
            Validator.ValidateString(userID, "userID");

            await this.DeleteAsync("channels/vips?broadcaster_id=" + channelID + "&user_id=" + userID);
        }

        /// <summary>
        /// Updates the chat settings for the broadcster's channel.
        /// </summary>
        /// <param name="channelID">The channel ID to update chat settings for</param>
        /// <param name="settings">The settings to update for the channel</param>
        /// <returns>The updated chat settings</returns>
        public async Task<ChatSettingsModel> UpdateChatSettings(string channelID, ChatSettingsModel settings)
        {
            Validator.ValidateString(channelID, "channelID");
            Validator.ValidateVariable(settings, "settings");

            NewTwitchAPIDataRestResult<ChatSettingsModel> updatedSettings =
                await this.PatchAsync<NewTwitchAPIDataRestResult<ChatSettingsModel>>("chat/settings?broadcaster_id=" + channelID + "&moderator_id=" + channelID,
                AdvancedHttpClient.CreateContentFromObject(settings));

            if (updatedSettings != null && updatedSettings.data.Count > 0)
            {
                return updatedSettings.data.First();
            }
            return null;
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
