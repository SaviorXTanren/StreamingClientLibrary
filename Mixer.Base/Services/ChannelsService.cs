using Mixer.Base.Model.Channel;
using Mixer.Base.Model.User;
using Mixer.Base.Util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mixer.Base.Services
{
    public class ChannelsService : ServiceBase
    {
        public ChannelsService(MixerClient client) : base(client) { }

        public async Task<ExpandedChannelModel> GetChannel(string channelName)
        {
            Validator.ValidateString(channelName, "channelName");

            return await this.GetAsync<ExpandedChannelModel>("channels/" + channelName);
        }

        public async Task<ChannelModel> UpdateChannel(ChannelModel channel)
        {
            Validator.ValidateVariable(channel, "channel");

            return await this.PatchAsync<ChannelModel>("channels/" + channel.id, this.CreateContentFromObject(channel));
        }

        public async Task<IEnumerable<UserWithChannelModel>> GetFollowers(ChannelModel channel)
        {
            Validator.ValidateVariable(channel, "channel");

            return await this.GetPagedAsync<UserWithChannelModel>("channels/" + channel.id + "/follow");
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
    }
}
