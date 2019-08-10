using Mixer.Base.Model.Channel;
using Mixer.Base.Model.MixPlay;
using Newtonsoft.Json.Linq;
using StreamingClient.Base.Util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mixer.Base.Services
{
    /// <summary>
    /// The APIs for MixPlay-based services.
    /// </summary>
    public class MixPlayService : MixerServiceBase
    {
        /// <summary>
        /// Creates an instance of the MixPlayService.
        /// </summary>
        /// <param name="connection">The Mixer connection to use</param>
        public MixPlayService(MixerConnection connection) : base(connection) { }

        /// <summary>
        /// Gets the MixPlay host connection addresses.
        /// </summary>
        /// <returns>The MixPlay host connection addresses</returns>
        public async Task<IEnumerable<string>> GetMixPlayHosts()
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

        /// <summary>
        /// Gets all of the owned games for the specified channel.
        /// </summary>
        /// <param name="channel">The channel to get games for</param>
        /// <param name="maxResults">The maximum number of results. Will be either that amount or slightly more</param>
        /// <returns>The games owned by the channel</returns>
        public async Task<IEnumerable<MixPlayGameListingModel>> GetOwnedMixPlayGames(ChannelModel channel, uint maxResults = uint.MaxValue)
        {
            Validator.ValidateVariable(channel, "channel");
            return await this.GetPagedNumberAsync<MixPlayGameListingModel>("interactive/games/owned?user=" + channel.userId, maxResults);
        }

        /// <summary>
        /// Gets all of the shared games for the specified channel.
        /// </summary>
        /// <param name="channel">The channel to get games for</param>
        /// <param name="maxResults">The maximum number of results. Will be either that amount or slightly more</param>
        /// <returns>The games shared with the channel</returns>
        public async Task<IEnumerable<MixPlayGameListingModel>> GetSharedMixPlayGames(ChannelModel channel, uint maxResults = uint.MaxValue)
        {
            Validator.ValidateVariable(channel, "channel");
            return await this.GetPagedNumberAsync<MixPlayGameListingModel>("interactive/games/shared?user=" + channel.userId, maxResults);
        }

        /// <summary>
        /// Gets all of the games where the channel owner has editor permissions.
        /// </summary>
        /// <param name="channel">The channel to get games for</param>
        /// <param name="maxResults">The maximum number of results. Will be either that amount or slightly more</param>
        /// <returns>The games that have editor permissions set for the Channel Owner</returns>
        public async Task<IEnumerable<MixPlayGameListingModel>> GetEditorMixPlayGames(ChannelModel channel, uint maxResults = uint.MaxValue)
        {
            Validator.ValidateVariable(channel, "channel");
            return await this.GetPagedNumberAsync<MixPlayGameListingModel>("interactive/games/editor?user=" + channel.userId,maxResults);
        }


        /// <summary>
        /// Creates the specified MixPlay game.
        /// </summary>
        /// <param name="game">The MixPlay game to create</param>
        /// <returns>The created MixPlay game</returns>
        public async Task<MixPlayGameModel> CreateMixPlayGame(MixPlayGameModel game)
        {
            Validator.ValidateVariable(game, "game");
            return await this.PostAsync<MixPlayGameModel>("interactive/games", this.CreateContentFromObject(game));
        }

        /// <summary>
        /// Gets the specified MixPlay game.
        /// </summary>
        /// <param name="game">The MixPlay game to get</param>
        /// <returns></returns>
        public async Task<MixPlayGameModel> GetMixPlayGame(MixPlayGameModel game)
        {
            Validator.ValidateVariable(game, "game");
            return await this.GetMixPlayGame(game.id);
        }

        /// <summary>
        /// Gets the specified MixPlay game.
        /// </summary>
        /// <param name="gameID">The ID of the MixPlay game to get</param>
        /// <returns></returns>
        public async Task<MixPlayGameModel> GetMixPlayGame(uint gameID)
        {
            return await this.GetAsync<MixPlayGameModel>("interactive/games/" + gameID);
        }

        /// <summary>
        /// Updates the specified MixPlay game.
        /// </summary>
        /// <param name="game">The MixPlay game to update</param>
        /// <returns>The updated MixPlay game</returns>
        public async Task<MixPlayGameModel> UpdateMixPlayGame(MixPlayGameModel game)
        {
            Validator.ValidateVariable(game, "game");
            return await this.PutAsync<MixPlayGameModel>("interactive/games/" + game.id, this.CreateContentFromObject(game));
        }

        /// <summary>
        /// Deletes the specified MixPlay game.
        /// </summary>
        /// <param name="game">The MixPlay game to delete</param>
        /// <returns>Whether the operation succeeded</returns>
        public async Task<bool> DeleteMixPlayGame(MixPlayGameModel game)
        {
            Validator.ValidateVariable(game, "game");
            return await this.DeleteAsync("interactive/games/" + game.id);
        }

        /// <summary>
        /// Gets the MixPlay game versions for the specified game.
        /// </summary>
        /// <param name="game">The MixPlay game to get versions for</param>
        /// <param name="maxResults">The maximum number of results. Will be either that amount or slightly more</param>
        /// <returns>The MixPlay game versions</returns>
        public async Task<IEnumerable<MixPlayGameVersionModel>> GetMixPlayGameVersions(MixPlayGameModel game, uint maxResults = uint.MaxValue)
        {
            Validator.ValidateVariable(game, "game");
            return await this.GetPagedNumberAsync<MixPlayGameVersionModel>("interactive/games/" + game.id + "/versions", maxResults);
        }

        /// <summary>
        /// Creates the specified MixPlay game version.
        /// </summary>
        /// <param name="version">The MixPlay game version to create</param>
        /// <returns>The created MixPlay game version</returns>
        public async Task<MixPlayGameVersionModel> CreateMixPlayGameVersion(MixPlayGameVersionModel version)
        {
            Validator.ValidateVariable(version, "version");
            return await this.PostAsync<MixPlayGameVersionModel>("interactive/versions", this.CreateContentFromObject(version));
        }

        /// <summary>
        /// Gets the specified MixPlay game version.
        /// </summary>
        /// <param name="version">The MixPlay game version to get</param>
        /// <returns>The MixPlay game version</returns>
        public async Task<MixPlayGameVersionModel> GetMixPlayGameVersion(MixPlayGameVersionModel version)
        {
            Validator.ValidateVariable(version, "version");
            return await this.GetMixPlayGameVersion(version.id);
        }

        /// <summary>
        /// Gets the specified MixPlay game version.
        /// </summary>
        /// <param name="versionID">The ID of the MixPlay game version to get</param>
        /// <returns>The MixPlay game version</returns>
        public async Task<MixPlayGameVersionModel> GetMixPlayGameVersion(uint versionID)
        {
            return await this.GetAsync<MixPlayGameVersionModel>("interactive/versions/" + versionID);
        }

        /// <summary>
        /// Updates the specified MixPlay game version.
        /// </summary>
        /// <param name="version">The MixPlay game version to update</param>
        /// <returns>The updated MixPlay game version</returns>
        public async Task<MixPlayGameVersionModel> UpdateMixPlayGameVersion(MixPlayGameVersionModel version)
        {
            Validator.ValidateVariable(version, "version");

            // Need to strip out all of the non-updateable fields in order for the API to not return a 403 error
            MixPlayGameVersionUpdateableModel updateableVersion = JSONSerializerHelper.Clone<MixPlayGameVersionUpdateableModel>(version);
            updateableVersion.controls = version.controls;

            return await this.PutAsync<MixPlayGameVersionModel>("interactive/versions/" + version.id, this.CreateContentFromObject(updateableVersion));
        }

        /// <summary>
        /// Deletes the specified MixPlay game version.
        /// </summary>
        /// <param name="version">The MixPlay game version to delete</param>
        /// <returns>Whether the operation succeeded</returns>
        public async Task<bool> DeleteMixPlayGameVersion(MixPlayGameVersionModel version)
        {
            Validator.ValidateVariable(version, "version");
            return await this.DeleteAsync("interactive/versions/" + version.id);
        }
    }
}
