using Mixer.Base.Model.Teams;
using Mixer.Base.Model.User;
using Mixer.Base.Util;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mixer.Base.Services
{
    /// <summary>
    /// The APIs for team-based services.
    /// </summary>
    public class TeamsService : MixerServiceBase
    {
        /// <summary>
        /// Creates an instance of the TeamsService.
        /// </summary>
        /// <param name="connection">The Mixer connection to use</param>
        public TeamsService(MixerConnection connection) : base(connection) { }

        /// <summary>
        /// Gets all of the known teams. The search can be limited to a maximum number of results to speed up the operation
        /// as it can take a long time on large channels. This maximum number is a lower threshold and slightly more than the
        /// maximum number may be returned.
        /// </summary>
        /// <param name="maxResults">The maximum number of results. Will be either that amount or slightly more</param>
        /// <returns>All teams</returns>
        public async Task<IEnumerable<TeamModel>> GetTeams(uint maxResults = 1)
        {
            return await this.GetPagedAsync<TeamModel>("teams", maxResults);
        }

        /// <summary>
        /// Gets the team with the specified id.
        /// </summary>
        /// <param name="id">The id of the team</param>
        /// <returns>The team</returns>
        public async Task<TeamModel> GetTeam(uint id)
        {
            Validator.ValidateVariable(id, "id");
            return await this.GetAsync<TeamModel>("teams/" + id.ToString());
        }

        /// <summary>
        /// Gets the team with the specified name.
        /// </summary>
        /// <param name="name">The id of the team</param>
        /// <returns>The team</returns>
        public async Task<TeamModel> GetTeam(string name)
        {
            Validator.ValidateVariable(name, "name");
            IEnumerable<TeamModel> results = await this.GetPagedAsync<TeamModel>("teams?where=name:eq:" + this.EncodeString(name));
            return results.FirstOrDefault();
        }

        /// <summary>
        /// Gets all users for the specified team. The search can be limited to a maximum number of results to speed up the operation
        /// as it can take a long time on large channels. This maximum number is a lower threshold and slightly more than the
        /// maximum number may be returned.
        /// </summary>
        /// <param name="team">The team to get users for</param>
        /// <param name="maxResults">The maximum number of results. Will be either that amount or slightly more</param>
        /// <returns>The users</returns>
        public async Task<IEnumerable<UserWithChannelModel>> GetTeamUsers(TeamModel team, uint maxResults = 1)
        {
            return await this.GetPagedAsync<UserWithChannelModel>("teams/" + team.id.ToString() + "/users", maxResults);
        }
    }
}
