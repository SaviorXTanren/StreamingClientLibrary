using Mixer.Base.Model.Channel;
using Mixer.Base.Model.Interactive;
using Mixer.Base.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mixer.Base.Services
{
    public class InteractiveService : ServiceBase
    {
        public InteractiveService(MixerConnection connection) : base(connection) { }

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
            Validator.ValidateVariable(channel, "channel");
            return await this.GetAsync<IEnumerable<InteractiveGameListingModel>>("interactive/games/owned?user=" + channel.userId);
        }

        public async Task<IEnumerable<InteractiveGameListingModel>> GetSharedInteractiveGames(ChannelModel channel)
        {
            Validator.ValidateVariable(channel, "channel");
            return await this.GetAsync<IEnumerable<InteractiveGameListingModel>>("interactive/games/shared?user=" + channel.userId);
        }

        public async Task<InteractiveVersionModel> GetInteractiveVersionInfo(InteractiveVersionModel version)
        {
            Validator.ValidateVariable(version, "version");
            return await this.GetAsync<InteractiveVersionModel>("interactive/versions/" + version.id);
        }

        public async Task<InteractiveGameModel> CreateInteractiveGame(InteractiveGameModel game)
        {
            Validator.ValidateVariable(game, "game");
            return await this.PostAsync<InteractiveGameModel>("interactive/games", this.CreateContentFromObject(game));
        }

        public async Task<InteractiveGameModel> UpdateInteractiveGame(InteractiveGameModel game)
        {
            Validator.ValidateVariable(game, "game");
            return await this.PutAsync<InteractiveGameModel>("interactive/games/" + game.id, this.CreateContentFromObject(game));
        }

        public async Task<bool> DeleteInteractiveGame(InteractiveGameModel game)
        {
            Validator.ValidateVariable(game, "game");
            return await this.DeleteAsync("interactive/games/" + game.id);
        }

        public async Task<InteractiveVersionModel> CreateInteractiveGameVersion(InteractiveVersionModel version)
        {
            Validator.ValidateVariable(version, "version");
            return await this.PostAsync<InteractiveVersionModel>("interactive/versions", this.CreateContentFromObject(version));
        }

        public async Task<InteractiveVersionModel> GetInteractiveGameVersion(InteractiveVersionModel version)
        {
            Validator.ValidateVariable(version, "version");
            return await this.GetAsync<InteractiveVersionModel>("interactive/versions/" + version.id);
        }

        public async Task<InteractiveVersionModel> UpdateInteractiveGameVersion(InteractiveVersionModel version)
        {
            Validator.ValidateVariable(version, "version");

            // Need to strip out all of the non-updateable fields in order for the API to not return a 403 error
            string json = JsonConvert.SerializeObject(version);
            JObject jobj = JObject.Parse(json);
            InteractiveVersionUpdateableModel updateableVersion = jobj.ToObject<InteractiveVersionUpdateableModel>();
            updateableVersion.controls = version.controls;

            return await this.PutAsync<InteractiveVersionModel>("interactive/versions/" + version.id, this.CreateContentFromObject(updateableVersion));
        }

        public async Task<bool> DeleteInteractiveGameVersion(InteractiveVersionModel version)
        {
            Validator.ValidateVariable(version, "version");
            return await this.DeleteAsync("interactive/versions/" + version.id);
        }
    }
}
