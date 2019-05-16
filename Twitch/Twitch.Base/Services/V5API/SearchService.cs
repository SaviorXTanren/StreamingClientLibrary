using StreamingClient.Base.Util;
using StreamingClient.Base.Web;
using System.Collections.Generic;
using System.Threading.Tasks;
using Twitch.Base.Models.V5.Channel;
using Twitch.Base.Models.V5.Games;
using Twitch.Base.Models.V5.Streams;

namespace Twitch.Base.Services.V5API
{
    /// <summary>
    /// The APIs for Search-based services.
    /// </summary>
    public class SearchService : V5APIServiceBase
    {
        /// <summary>
        /// Creates an instance of the SearchService.
        /// </summary>
        /// <param name="connection">The Twitch connection to use</param>
        public SearchService(TwitchConnection connection) : base(connection) { }

        /// <summary>
        /// Gets the list of channels that have the specified query string associated with them.
        /// </summary>
        /// <param name="query">The text to search for</param>
        /// <param name="maxResults">The maximum number of results. Will be either that amount or slightly more</param>
        /// <returns>The list of associated channels</returns>
        public async Task<IEnumerable<ChannelModel>> SearchChannels(string query, int maxResults = 1)
        {
            Validator.ValidateVariable(query, "query");
            return await this.GetOffsetPagedResultAsync<ChannelModel>("search/channels?query=" + AdvancedHttpClient.EncodeString(query), "channels", maxResults);
        }

        /// <summary>
        /// Gets the list of games that have the specified query string associated with them.
        /// </summary>
        /// <param name="query">The text to search for</param>
        /// <param name="currentlyLive">Return only games that are live on at least one channel</param>
        /// <returns>The list of associated games</returns>
        public async Task<IEnumerable<GameModel>> SearchGames(string query, bool currentlyLive = false)
        {
            Validator.ValidateVariable(query, "query");
            return await this.GetNamedArrayAsync<GameModel>("search/games?query=" + AdvancedHttpClient.EncodeString(query) + "&live=" + currentlyLive, "games");
        }

        /// <summary>
        /// Gets the list of streams that have the specified query string associated with them.
        /// </summary>
        /// <param name="query">The text to search for</param>
        /// <param name="maxResults">The maximum number of results. Will be either that amount or slightly more</param>
        /// <returns>The list of associated streams</returns>
        public async Task<IEnumerable<StreamModel>> SearchStreams(string query, int maxResults = 1)
        {
            Validator.ValidateVariable(query, "query");
            return await this.GetOffsetPagedResultAsync<StreamModel>("search/streams?query=" + AdvancedHttpClient.EncodeString(query), "streams", maxResults);
        }
    }
}
