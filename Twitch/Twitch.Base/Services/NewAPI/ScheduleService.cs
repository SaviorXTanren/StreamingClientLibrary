using StreamingClient.Base.Util;
using System.Threading.Tasks;
using Twitch.Base.Models.NewAPI.Schedule;
using Twitch.Base.Models.NewAPI.Users;

namespace Twitch.Base.Services.NewAPI
{
    /// <summary>
    /// APIs for schedule-based services
    /// </summary>
    public class ScheduleService : NewTwitchAPIServiceBase
    {
        /// <summary>
        /// Creates an instance of the ScheduleService.
        /// </summary>
        /// <param name="connection">The Twitch connection to use</param>
        public ScheduleService(TwitchConnection connection) : base(connection) { }

        /// <summary>
        /// Get stream schedule for broadcaster
        /// </summary>
        /// <param name="broadcaster">broadcaster</param>
        /// <param name="maxResults">Maximum schedule segment results to return</param>
        /// <returns>Broadcaster's schedule details</returns>
        public async Task<ScheduleModel> GetSchedule(UserModel broadcaster, int maxResults=1)
        {
            Validator.ValidateVariable(broadcaster, "broadcaster");
            return await this.GetPagedDataSegmentsResultAsync<ScheduleModel>("schedule?broadcaster_id=" + broadcaster.id, maxResults);
        }
    }
}
