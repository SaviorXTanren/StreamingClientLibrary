using Mixer.Base.Model.Channel;
using Mixer.Base.Model.Interactive;
using Mixer.Base.Util;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mixer.Base.Services
{
    /// <summary>
    /// The APIs for interactive-based services.
    /// </summary>
    public class InteractiveService : MixerServiceBase
    {
        /// <summary>
        /// Creates an instance of the InteractiveService.
        /// </summary>
        /// <param name="connection">The Mixer connection to use</param>
        public InteractiveService(MixerConnection connection) : base(connection) { }

        /// <summary>
        /// Gets the interactive connection information for the specified channel.
        /// </summary>
        /// <param name="channel">The channel to get interactive connection information for</param>
        /// <returns>The interactive connection information</returns>
        public async Task<InteractiveConnectionInfoModel> GetInteractiveConnectionInfo(ChannelModel channel)
        {
            Validator.ValidateVariable(channel, "channel");
            return await this.GetAsync<InteractiveConnectionInfoModel>("interactive/" + channel.id);
        }

        /// <summary>
        /// Gets the robot interactive connection information for the specified channel.
        /// </summary>
        /// <param name="channel">The channel to get robot interactive connection information for</param>
        /// <returns>The interactive connection information</returns>
        public async Task<InteractiveRobotConnectionModel> GetInteractiveRobotConnectionInfo(ChannelModel channel)
        {
            Validator.ValidateVariable(channel, "channel");
            return await this.GetAsync<InteractiveRobotConnectionModel>("interactive/" + channel.id + "/robot");
        }

        /// <summary>
        /// Gets the interactive host connection addresses.
        /// </summary>
        /// <returns>The interactive host connection addresses</returns>
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

        /// <summary>
        /// Gets all of the owned games for the specified channel.
        /// </summary>
        /// <param name="channel">The channel to get games for</param>
        /// <returns>The games owned by the channel</returns>
        public async Task<IEnumerable<InteractiveGameListingModel>> GetOwnedInteractiveGames(ChannelModel channel)
        {
            Validator.ValidateVariable(channel, "channel");
            return await this.GetPagedAsync<InteractiveGameListingModel>("interactive/games/owned?user=" + channel.userId);
        }

        /// <summary>
        /// Gets all of the shared games for the specified channel.
        /// </summary>
        /// <param name="channel">The channel to get games for</param>
        /// <returns>The games shared with the channel</returns>
        public async Task<IEnumerable<InteractiveGameListingModel>> GetSharedInteractiveGames(ChannelModel channel)
        {
            Validator.ValidateVariable(channel, "channel");
            return await this.GetPagedAsync<InteractiveGameListingModel>("interactive/games/shared?user=" + channel.userId);
        }

        /// <summary>
        /// Gets all of the games where the channel owner has editor permissions.
        /// </summary>
        /// <param name="channel">The channel to get games for</param>
        /// <returns>The games that have editor permissions set for the Channel Owner</returns>
        public async Task<IEnumerable<InteractiveGameListingModel>> GetEditorInteractiveGames(ChannelModel channel)
        {
            Validator.ValidateVariable(channel, "channel");
            return await this.GetPagedAsync<InteractiveGameListingModel>("interactive/games/editor?user=" + channel.userId);
        }


        /// <summary>
        /// Creates the specified interactive game.
        /// </summary>
        /// <param name="game">The interactive game to create</param>
        /// <returns>The created interactive game</returns>
        public async Task<InteractiveGameModel> CreateInteractiveGame(InteractiveGameModel game)
        {
            Validator.ValidateVariable(game, "game");
            return await this.PostAsync<InteractiveGameModel>("interactive/games", this.CreateContentFromObject(game));
        }

        /// <summary>
        /// Gets the specified interactive game.
        /// </summary>
        /// <param name="game">The interactive game to get</param>
        /// <returns></returns>
        public async Task<InteractiveGameModel> GetInteractiveGame(InteractiveGameModel game)
        {
            Validator.ValidateVariable(game, "game");
            return await this.GetInteractiveGame(game.id);
        }

        /// <summary>
        /// Gets the specified interactive game.
        /// </summary>
        /// <param name="gameID">The ID of the interactive game to get</param>
        /// <returns></returns>
        public async Task<InteractiveGameModel> GetInteractiveGame(uint gameID)
        {
            return await this.GetAsync<InteractiveGameModel>("interactive/games/" + gameID);
        }

        /// <summary>
        /// Updates the specified interactive game.
        /// </summary>
        /// <param name="game">The interactive game to update</param>
        /// <returns>The updated interactive game</returns>
        public async Task<InteractiveGameModel> UpdateInteractiveGame(InteractiveGameModel game)
        {
            Validator.ValidateVariable(game, "game");
            return await this.PutAsync<InteractiveGameModel>("interactive/games/" + game.id, this.CreateContentFromObject(game));
        }

        /// <summary>
        /// Deletes the specified interactive game.
        /// </summary>
        /// <param name="game">The interactive game to delete</param>
        /// <returns>Whether the operation succeeded</returns>
        public async Task<bool> DeleteInteractiveGame(InteractiveGameModel game)
        {
            Validator.ValidateVariable(game, "game");
            return await this.DeleteAsync("interactive/games/" + game.id);
        }

        /// <summary>
        /// Gets the interactive game versions for the specified game.
        /// </summary>
        /// <param name="game">The interactive game to get versions for</param>
        /// <returns>The interactive game versions</returns>
        public async Task<IEnumerable<InteractiveGameVersionModel>> GetInteractiveGameVersions(InteractiveGameModel game)
        {
            Validator.ValidateVariable(game, "game");
            return await this.GetPagedAsync<InteractiveGameVersionModel>("interactive/games/" + game.id + "/versions");
        }

        /// <summary>
        /// Creates the specified interactive game version.
        /// </summary>
        /// <param name="version">The interactive game version to create</param>
        /// <returns>The created interactive game version</returns>
        public async Task<InteractiveGameVersionModel> CreateInteractiveGameVersion(InteractiveGameVersionModel version)
        {
            Validator.ValidateVariable(version, "version");
            return await this.PostAsync<InteractiveGameVersionModel>("interactive/versions", this.CreateContentFromObject(version));
        }

        /// <summary>
        /// Gets the specified interactive game version.
        /// </summary>
        /// <param name="version">The interactive game version to get</param>
        /// <returns>The interactive game version</returns>
        public async Task<InteractiveGameVersionModel> GetInteractiveGameVersion(InteractiveGameVersionModel version)
        {
            Validator.ValidateVariable(version, "version");
            return await this.GetInteractiveGameVersion(version.id);
        }

        /// <summary>
        /// Gets the specified interactive game version.
        /// </summary>
        /// <param name="versionID">The ID of the interactive game version to get</param>
        /// <returns>The interactive game version</returns>
        public async Task<InteractiveGameVersionModel> GetInteractiveGameVersion(uint versionID)
        {
            return await this.GetAsync<InteractiveGameVersionModel>("interactive/versions/" + versionID);
        }

        /// <summary>
        /// Updates the specified interactive game version.
        /// </summary>
        /// <param name="version">The interactive game version to update</param>
        /// <returns>The updated interactive game version</returns>
        public async Task<InteractiveGameVersionModel> UpdateInteractiveGameVersion(InteractiveGameVersionModel version)
        {
            Validator.ValidateVariable(version, "version");

            // Need to strip out all of the non-updateable fields in order for the API to not return a 403 error
            InteractiveGameVersionUpdateableModel updateableVersion = JsonHelper.ConvertToDifferentType<InteractiveGameVersionUpdateableModel>(version);
            updateableVersion.controls = version.controls;

            return await this.PutAsync<InteractiveGameVersionModel>("interactive/versions/" + version.id, this.CreateContentFromObject(updateableVersion));
        }

        /// <summary>
        /// Deletes the specified interactive game version.
        /// </summary>
        /// <param name="version">The interactive game version to delete</param>
        /// <returns>Whether the operation succeeded</returns>
        public async Task<bool> DeleteInteractiveGameVersion(InteractiveGameVersionModel version)
        {
            Validator.ValidateVariable(version, "version");
            return await this.DeleteAsync("interactive/versions/" + version.id);
        }
    }
}
