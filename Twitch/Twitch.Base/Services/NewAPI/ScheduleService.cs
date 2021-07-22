using StreamingClient.Base.Util;
using System.Collections.Generic;
using System.Threading.Tasks;
using Twitch.Base.Models.NewAPI;
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
        /// <param name="maxResults">Maximum schedule segment results to return. Will be either that amount or slightly more.</param>
        /// <returns>Broadcaster's schedule details</returns>
        public async Task<ScheduleModel> GetSchedule(UserModel broadcaster, int maxResults)
        {
            Validator.ValidateVariable(broadcaster, "broadcaster");
            string cursor = null;
            ScheduleModel scheduleModel = null;
            NewTwitchAPISingleDataRestResult<ScheduleModel> results;

            do
            {
                int totalSegmentCount = (scheduleModel?.segments?.Count ?? 0);
                int itemsToRetrieve = maxResults - totalSegmentCount > 25 ? 25 : maxResults - totalSegmentCount;
                results = await this.GetPagedSingleDataResultAsync<ScheduleModel>("schedule?broadcaster_id=" + broadcaster.id, itemsToRetrieve, cursor);
                cursor = results.Cursor;

                if (results.data != null)
                {
                    if (scheduleModel == null)
                        scheduleModel = results.data;
                    else
                    {
                        var segments = results.data.segments;
                        scheduleModel.segments.AddRange(segments);
                    }
                }
            }
            while (scheduleModel!=null && scheduleModel.segments.Count < maxResults && cursor!=null);

            return scheduleModel;
        }
    }
}
