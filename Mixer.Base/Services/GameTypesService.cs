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

        /// <summary>
        /// Gets all known game types. The search can be limited to a maximum number of results to speed
        /// up the operation as it can take a long time on large channels. This maximum number is a lower threshold and slightly
        /// more than the maximum number may be returned.
        /// </summary>
        /// <param name="name">The name of the game to search for</param>
        /// <param name="maxResults">The maximum number of results. Will be either that amount or slightly more</param>
        /// <returns>All game types</returns>
        public async Task<IEnumerable<GameTypeModel>> GetGameTypes(uint maxResults = 0)
        {
            return await this.GetPagedAsync<GameTypeModel>("types", maxResults);
        }

        /// <summary>
        /// Gets all known game types using the specified name. The search can be limited to a maximum number of results to speed
        /// up the operation as it can take a long time on large channels. This maximum number is a lower threshold and slightly
        /// more than the maximum number may be returned.
        /// </summary>
        /// <param name="name">The name of the game to search for</param>
        /// <param name="maxResults">The maximum number of results. Will be either that amount or slightly more</param>
        /// <returns>All game types</returns>
        public async Task<IEnumerable<GameTypeModel>> GetGameTypes(string name, uint maxResults = 0)
        {
            Validator.ValidateString(name, "name");

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("query", name);
            FormUrlEncodedContent content = new FormUrlEncodedContent(parameters.AsEnumerable());

            return await this.GetPagedAsync<GameTypeModel>("types?" + await content.ReadAsStringAsync(), maxResults);
        }

        /// <summary>
        /// Gets all channels playing the specified game type. The search can be limited to a maximum number of results to speed
        /// up the operation as it can take a long time on large channels. This maximum number is a lower threshold and slightly
        /// more than the maximum number may be returned.
        /// </summary>
        /// <param name="gameType">The game type to search for</param>
        /// <param name="maxResults">The maximum number of results. Will be either that amount or slightly more</param>
        /// <returns>All channels with the specified game type</returns>
        public async Task<IEnumerable<ChannelModel>> GetChannelsByGameType(GameTypeSimpleModel gameType, uint maxResults = 0)
        {
            Validator.ValidateVariable(gameType, "gameType");

            return await this.GetPagedAsync<ChannelModel>("types/" + gameType.id + "/channels", maxResults);
        }
    }
}
