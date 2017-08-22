using Mixer.Base.Model.Channel;
using Mixer.Base.Model.User;
using Mixer.Base.Util;
using System.Collections.Generic;
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

        public async Task<IEnumerable<UserWithChannelModel>> GetFollowers(ChannelModel channel)
        {
            Validator.ValidateVariable(channel, "channel");
            return await this.GetPagedAsync<UserWithChannelModel>("channels/" + channel.id + "/follow");
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

        public async Task<IEnumerable<ChannelAdvancedModel>> GetHosters(ChannelModel channel)
        {
            Validator.ValidateVariable(channel, "channel");
            return await this.GetPagedAsync<ChannelAdvancedModel>("channels/" + channel.id + "/hosters");
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

        public async Task<IEnumerable<UserWithGroupsModel>> GetUsersWithRoles(ChannelModel channel)
        {
            Validator.ValidateVariable(channel, "channel");
            return await this.GetPagedAsync<UserWithGroupsModel>("channels/" + channel.id + "/users");
        }

        public async Task<IEnumerable<UserWithGroupsModel>> GetUsersWithRoles(ChannelModel channel, string role)
        {
            Validator.ValidateVariable(channel, "channel");
            Validator.ValidateString(role, "role");
            return await this.GetPagedAsync<UserWithGroupsModel>("channels/" + channel.id + "/users/" + role);
        }
    }
}
