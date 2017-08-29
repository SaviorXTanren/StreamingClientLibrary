using Mixer.Base.Model.Teams;
using Mixer.Base.Model.User;
using Mixer.Base.Util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mixer.Base.Services
{
    /// <summary>
    /// The APIs for team-based services.
    /// </summary>
    public class TeamsService : ServiceBase
    {
        public TeamsService(MixerConnection connection) : base(connection) { }

        public async Task<IEnumerable<TeamModel>> GetTeams(uint maxResults = 0)
        {
            return await this.GetPagedAsync<TeamModel>("teams", maxResults);
        }

        public async Task<TeamModel> GetTeam(uint id)
        {
            Validator.ValidateVariable(id, "id");
            return await this.GetAsync<TeamModel>("teams/" + id.ToString());
        }

        public async Task<IEnumerable<UserWithChannelModel>> GetTeamUsers(TeamModel team, uint maxResults = 0)
        {
            return await this.GetPagedAsync<UserWithChannelModel>("teams/" + team.id.ToString() + "/users", maxResults);
        }
    }
}
