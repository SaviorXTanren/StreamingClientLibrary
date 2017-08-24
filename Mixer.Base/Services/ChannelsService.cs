using Mixer.Base.Model.Channel;
using Mixer.Base.Model.User;
using Mixer.Base.Util;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Mixer.Base.Services
{
    public class ChannelsService : ServiceBase
    {
        public ChannelsService(MixerConnection connection) : base(connection) { }

        public async Task<ExpandedChannelModel> GetChannel(string channelName)
        {
            Validator.ValidateString(channelName, "channelName");
            return await this.GetAsync<ExpandedChannelModel>("channels/" + channelName);
        }

        public async Task<ChannelModel> UpdateChannel(ChannelModel channel)
        {
            Validator.ValidateVariable(channel, "channel");

            // Need to strip out all of the non-updateable fields in order for the API to not return a 403 error
            ChannelUpdateableModel updateableChannel = JsonHelper.ConvertToDifferentType<ChannelUpdateableModel>(channel);

            return await this.PatchAsync<ChannelModel>("channels/" + channel.id, this.CreateContentFromObject(updateableChannel));
        }

        public async Task<IEnumerable<UserWithChannelModel>> GetFollowers(ChannelModel channel, uint maxResults = 0)
        {
            Validator.ValidateVariable(channel, "channel");
            return await this.GetPagedAsync<UserWithChannelModel>("channels/" + channel.id + "/follow", maxResults);
        }

        public async Task<ChannelAdvancedModel> GetHostedChannel(ChannelModel channel)
        {
            Validator.ValidateVariable(channel, "channel");
            return await this.GetAsync<ChannelAdvancedModel>("channels/" + channel.id + "/hostee");
        }

        public async Task<ChannelModel> SetHostChannel(ChannelModel channel, ChannelModel hosteeChannel)
        {
            Validator.ValidateVariable(channel, "channel");
            Validator.ValidateVariable(hosteeChannel, "hosteeChannel");
            return await this.PutAsync<ChannelModel>("channels/" + channel.id + "/hostee", this.CreateContentFromObject(hosteeChannel));
        }

        public async Task<bool> UnhostChannel(ChannelModel channel)
        {
            Validator.ValidateVariable(channel, "channel");
            return await this.DeleteAsync("channels/" + channel.id + "/hostee");
        }

        public async Task<IEnumerable<ChannelAdvancedModel>> GetHosters(ChannelModel channel, uint maxResults = 0)
        {
            Validator.ValidateVariable(channel, "channel");
            return await this.GetPagedAsync<ChannelAdvancedModel>("channels/" + channel.id + "/hosters", maxResults);
        }

        public async Task<ChannelPreferencesModel> GetPreferences(ChannelModel channel)
        {
            Validator.ValidateVariable(channel, "channel");
            return await this.GetAsync<ChannelPreferencesModel>("channels/" + channel.id + "/preferences");
        }

        public async Task<ChannelPreferencesModel> UpdatePreferences(ChannelModel channel, ChannelPreferencesModel preferences)
        {
            Validator.ValidateVariable(channel, "channel");
            Validator.ValidateVariable(preferences, "preferences");
            return await this.PostAsync<ChannelPreferencesModel>("channels/" + channel.id + "/preferences", this.CreateContentFromObject(preferences));
        }

        public async Task<IEnumerable<UserWithGroupsModel>> GetUsersWithRoles(ChannelModel channel, uint maxResults = 0)
        {
            Validator.ValidateVariable(channel, "channel");
            return await this.GetPagedAsync<UserWithGroupsModel>("channels/" + channel.id + "/users", maxResults);
        }

        public async Task<IEnumerable<UserWithGroupsModel>> GetUsersWithRoles(ChannelModel channel, string role, uint maxResults = 0)
        {
            Validator.ValidateVariable(channel, "channel");
            Validator.ValidateString(role, "role");
            return await this.GetPagedAsync<UserWithGroupsModel>("channels/" + channel.id + "/users/" + role, maxResults);
        }

        public async Task<DiscordBotModel> GetDiscord(ChannelModel channel)
        {
            Validator.ValidateVariable(channel, "channel");
            return await this.GetAsync<DiscordBotModel>("channels/" + channel.id + "/discord");
        }

        public async Task<DiscordBotModel> UpdateDiscord(ChannelModel channel, DiscordBotModel discord)
        {
            Validator.ValidateVariable(channel, "channel");
            Validator.ValidateVariable(discord, "discord");
            return await this.PutAsync<DiscordBotModel>("channels/" + channel.id + "/discord", this.CreateContentFromObject(discord));
        }

        public async Task<IEnumerable<DiscordChannelModel>> GetDiscordChannels(ChannelModel channel)
        {
            Validator.ValidateVariable(channel, "channel");
            return await this.GetAsync<IEnumerable<DiscordChannelModel>>("channels/" + channel.id + "/discord/channels");
        }

        public async Task<IEnumerable<DiscordRoleModel>> GetDiscordRoles(ChannelModel channel)
        {
            Validator.ValidateVariable(channel, "channel");
            return await this.GetAsync<IEnumerable<DiscordRoleModel>>("channels/" + channel.id + "/discord/roles");
        }

        public async Task<bool> CanUserGetDiscordInvite(ChannelModel channel, UserModel user)
        {
            Validator.ValidateVariable(channel, "channel");
            HttpResponseMessage response = await this.GetAsync("channels/" + channel.id + "/discord/roles?user=" + user.id);
            return (response.StatusCode == System.Net.HttpStatusCode.NoContent);
        }

        public async Task<string> GetUserDiscordInvite(ChannelModel channel, UserModel user)
        {
            Validator.ValidateVariable(channel, "channel");
            JObject jobj = await this.GetJObjectAsync("channels/" + channel.id + "/discord/roles?user=" + user.id);
            JToken invite;
            if (jobj.TryGetValue("redirectUri", out invite))
            {
                return invite.ToString();
            }
            return null;
        }
    }
}
