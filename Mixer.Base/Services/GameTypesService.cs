using Mixer.Base.Model.Channel;
using Mixer.Base.Model.Game;
using Mixer.Base.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Mixer.Base.Services
{
    /// <summary>
    /// The APIs for game type-based services.
    /// </summary>
    public class GameTypesService : ServiceBase
    {
        public GameTypesService(MixerConnection connection) : base(connection) { }

        public async Task<IEnumerable<GameTypeModel>> GetGameTypes(uint maxResults = 0)
        {
            return await this.GetPagedAsync<GameTypeModel>("types", maxResults);
        }

        public async Task<IEnumerable<GameTypeSimpleModel>> GetGameTypesByLookup(string applicationUserModelId = null, string knownGamesListId = null, string processName = null, int xboxTitleId = -1, string titleName = null)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(applicationUserModelId)) { parameters.Add("aumId", applicationUserModelId); }
            if (!string.IsNullOrEmpty(knownGamesListId)) { parameters.Add("kglId", knownGamesListId); }
            if (!string.IsNullOrEmpty(processName)) { parameters.Add("processName", processName); }
            if (xboxTitleId > 0) { parameters.Add("titleId", xboxTitleId.ToString()); }
            if (!string.IsNullOrEmpty(titleName)) { parameters.Add("titleName", titleName); }

            if (parameters.Count == 0)
            {
                throw new InvalidOperationException("At least one parameter must be specified");
            }

            FormUrlEncodedContent content = new FormUrlEncodedContent(parameters.AsEnumerable());

            return await this.GetAsync<IEnumerable<GameTypeSimpleModel>>("types/lookup" + "?" + await content.ReadAsStringAsync());
        }

        public async Task<IEnumerable<ChannelModel>> GetChannelsByGameType(GameTypeSimpleModel gameType, uint maxResults = 0)
        {
            Validator.ValidateVariable(gameType, "gameType");

            return await this.GetPagedAsync<ChannelModel>("types/" + gameType.id + "/channels", maxResults);
        }
    }
}
