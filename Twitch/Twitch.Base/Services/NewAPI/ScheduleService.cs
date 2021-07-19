using Newtonsoft.Json;
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
        /// <param name="maxScheduleSegments">Maximum schedule segment results to return</param>
        /// <returns>Broadcaster's schedule details</returns>
        public async Task<ScheduleModel> GetSchedule(UserModel broadcaster, int maxScheduleSegments)
        {
            Validator.ValidateVariable(broadcaster, "broadcaster");
            string cursor = null;
            ScheduleModel scheduleModel = new ScheduleModel();
            scheduleModel.segments = new List<ScheduleSegmentModel>();

            int totalScheduleSegments = 0;
            bool firstPass = true;
            NewTwitchAPISingleDataRestResult results;
            do
            {
                int itemsToRetrieve = maxScheduleSegments - totalScheduleSegments > 25 ? 25 : maxScheduleSegments - totalScheduleSegments;
                results = await this.GetPagedSingleDataResultAsync("schedule?broadcaster_id=" + broadcaster.id, itemsToRetrieve, cursor);
                cursor = results.Cursor;

                if (results.data != null)
                {
                    if (firstPass)
                    {
                        scheduleModel = results.data.ToObject<ScheduleModel>();
                        firstPass = false;
                        totalScheduleSegments += results.data.ToObject<ScheduleModel>().segments.Count;
                    }
                    else
                    {
                        var segments = results.data.ToObject<ScheduleModel>().segments;
                        scheduleModel.segments.AddRange(segments);
                        totalScheduleSegments += segments.Count;
                    }
                }

            }
            while (totalScheduleSegments < maxScheduleSegments && cursor!=null && results.data!=null);

            return scheduleModel;
        }
    }
}
