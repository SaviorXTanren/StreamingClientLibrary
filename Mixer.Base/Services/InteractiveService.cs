using Mixer.Base.Clients;
using Mixer.Base.Model.Channel;
using Mixer.Base.Model.Interactive;
using Mixer.Base.Util;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mixer.Base.Services
{
    public class InteractiveService : ServiceBase
    {
        public InteractiveService(MixerConnection client) : base(client) { }

        public async Task<InteractiveConnectionInfoModel> GetInteractive(ChannelModel channel)
        {
            Validator.ValidateVariable(channel, "channel");
            return await this.GetAsync<InteractiveConnectionInfoModel>("interactive/" + channel.id);
        }

        public async Task<InteractiveRobotConnectionModel> GetInteractiveRobot(ChannelModel channel)
        {
            Validator.ValidateVariable(channel, "channel");
            return await this.GetAsync<InteractiveRobotConnectionModel>("interactive/" + channel.id + "/robot");
        }

        public async Task<IEnumerable<string>> GetInteractiveHosts()
        {
            List<string> addresses = new List<string>();
            string result = await this.GetStringAsync("interactive/hosts");

            JArray array = JArray.Parse(result);
            for (int i = 0; i < array.Count; i++)
            {
                addresses.Add(array[i]["address"].ToString());
            }

            return addresses;
        }

        public async Task<IEnumerable<InteractiveGameListingModel>> GetOwnedInteractiveGames(ChannelModel channel)
        {
            return await this.GetAsync<IEnumerable<InteractiveGameListingModel>>("interactive/games/owned?user=" + channel.userId);
        }

        public async Task<IEnumerable<InteractiveGameListingModel>> GetSharedInteractiveGames(ChannelModel channel)
        {
            return await this.GetAsync<IEnumerable<InteractiveGameListingModel>>("interactive/games/shared?user=" + channel.userId);
        }

        public async Task<InteractiveVersionModel> GetInteractiveVersionInfo(InteractiveVersionModel version)
        {
            return await this.GetAsync<InteractiveVersionModel>("interactive/versions/" + version.id);
        }
    }
}
