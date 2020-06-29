using System.Collections.Generic;
using System.Threading.Tasks;
using Twitch.Base.Models.V5.Teams;

namespace Twitch.Base.Services.V5API
{
    /// <summary>
    /// The APIs for Teams-based services.
    /// </summary>
    public class TeamsService : V5APIServiceBase
    {
        /// <summary>
        /// Creates an instance of the TeamsService.
        /// </summary>
        /// <param name="connection">The Twitch connection to use</param>
        public TeamsService(TwitchConnection connection) : base(connection) { }

        /// <summary>
        /// Gets all available teams.
        /// </summary>
        /// <param name="maxResults">The maximum number of results. Will be either that amount or slightly more</param>
        /// <returns>The set of teams</returns>
        public async Task<IEnumerable<TeamModel>> GetTeams(int maxResults = 1)
        {
            return await this.GetOffsetPagedResultAsync<TeamModel>("teams", "teams", maxResults);
        }

        /// <summary>
        /// Gets a specific team and it's members
        /// </summary>
        /// <param name="team">The team to get information for</param>
        /// <returns>The team</returns>
        public async Task<TeamDetailsModel> GetTeam(TeamModel team)
        {
            return await this.GetAsync<TeamDetailsModel>("teams/" + team.name);
        }
    }
}
