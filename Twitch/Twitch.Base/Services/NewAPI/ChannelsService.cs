using Newtonsoft.Json.Linq;
using StreamingClient.Base.Util;
using StreamingClient.Base.Web;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Twitch.Base.Models.NewAPI.Channels;
using Twitch.Base.Models.NewAPI.Users;

namespace Twitch.Base.Services.NewAPI
{
    /// <summary>
    /// The APIs for Channels-based services.
    /// </summary>
    public class ChannelsService : NewTwitchAPIServiceBase
    {
        /// <summary>
        /// Creates an instance of the ChannelsService.
        /// </summary>
        /// <param name="connection">The Twitch connection to use</param>
        public ChannelsService(TwitchConnection connection) : base(connection) { }

        /// <summary>
        /// Gets channel information for the specified user.
        /// </summary>
        /// <param name="user">The user to get channel information for</param>
        /// <returns>The channel information</returns>
        public async Task<ChannelInformationModel> GetChannelInformation(UserModel user)
        {
            Validator.ValidateVariable(user, "user");
            IEnumerable<ChannelInformationModel> results = await this.GetDataResultAsync<ChannelInformationModel>("channels?broadcaster_id=" + user.id);
            return results.FirstOrDefault();
        }

        /// <summary>
        /// Updates channel information for the specified channel information.
        /// </summary>
        /// <param name="channelInformation">The channel to update channel information for</param>
        /// <param name="title">The optional title to update to</param>
        /// <param name="gameID">The optional game ID to update to</param>
        /// <param name="broadcasterLanguage">The optional broadcast language to update to</param>
        /// <returns>Whether the update was successful or not</returns>
        public async Task<bool> UpdateChannelInformation(ChannelInformationModel channelInformation, string title = null, string gameID = null, string broadcasterLanguage = null)
        {
            Validator.ValidateVariable(channelInformation, "channelInformation");
            return await this.UpdateChannelInformation(channelInformation.broadcaster_id, title, gameID, broadcasterLanguage);
        }

        /// <summary>
        /// Updates channel information for the specified user.
        /// </summary>
        /// <param name="channel">The channel to update information for</param>
        /// <param name="title">The optional title to update to</param>
        /// <param name="gameID">The optional game ID to update to</param>
        /// <param name="broadcasterLanguage">The optional broadcast language to update to</param>
        /// <returns>Whether the update was successful or not</returns>
        public async Task<bool> UpdateChannelInformation(UserModel channel, string title = null, string gameID = null, string broadcasterLanguage = null)
        {
            Validator.ValidateVariable(channel, "channel");
            return await this.UpdateChannelInformation(channel.id, title, gameID, broadcasterLanguage);
        }

        private async Task<bool> UpdateChannelInformation(string broadcasterID, string title = null, string gameID = null, string broadcasterLanguage = null)
        {
            JObject jobj = new JObject();
            if (!string.IsNullOrEmpty(title)) { jobj["title"] = title; }
            if (!string.IsNullOrEmpty(gameID)) { jobj["game_id"] = gameID; }
            if (!string.IsNullOrEmpty(broadcasterLanguage)) { jobj["broadcaster_language"] = broadcasterLanguage; }
            HttpResponseMessage response = await this.PatchAsync("channels?broadcaster_id=" + broadcasterID, AdvancedHttpClient.CreateContentFromObject(jobj));
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Gets the most recent banned events for the specified channel.
        /// </summary>
        /// <param name="channel">The channel to get banned events for</param>
        /// <param name="maxResults">The maximum number of results. Will be either that amount or slightly more</param>
        /// <returns>The set of banned events</returns>
        public async Task<IEnumerable<ChannelBannedEventModel>> GetChannelBannedEvents(UserModel channel, int maxResults = 1)
        {
            Validator.ValidateVariable(channel, "channel");
            return await this.GetPagedDataResultAsync<ChannelBannedEventModel>("moderation/banned/events?broadcaster_id=" + channel.id);
        }

        /// <summary>
        /// Gets the information of the most recent Hype Train of the given channel ID.
        /// </summary>
        /// <param name="channel">The channel to get Hype Train data for</param>
        /// <returns>The most recent Hype Train</returns>
        public async Task<ChannelHypeTrainModel> GetHypeTrainEvents(UserModel channel)
        {
            Validator.ValidateVariable(channel, "channel");
            IEnumerable<ChannelHypeTrainModel> results = await this.GetDataResultAsync<ChannelHypeTrainModel>($"hypetrain/events?broadcaster_id={channel.id}&first=1");
            return results.FirstOrDefault();
        }
    }
}
