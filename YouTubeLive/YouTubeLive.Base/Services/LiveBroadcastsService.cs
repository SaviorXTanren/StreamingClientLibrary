using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Google.Apis.YouTubePartner.v1;
using Google.Apis.YouTubePartner.v1.Data;
using StreamingClient.Base.Util;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Google.Apis.YouTube.v3.LiveBroadcastsResource.ListRequest;

namespace YouTubeLive.Base.Services
{
    /// <summary>
    /// The APIs for Live Broadcast-based services.
    /// </summary>
    public class LiveBroadcastsService : YouTubeLiveServiceBase
    {
        /// <summary>
        /// Creates an instance of the LiveBroadcastsService.
        /// </summary>
        /// <param name="connection">The YouTube Live connection to use</param>
        public LiveBroadcastsService(YouTubeLiveConnection connection) : base(connection) { }

        /// <summary>
        /// Gets the live broadcasts associated with the currently authenticated account.
        /// </summary>
        /// <param name="broadcastStatus">The status for the broadcasts to get</param>
        /// <param name="maxResults">The maximum results to return</param>
        /// <returns>The list of live broadcasts</returns>
        public async Task<IEnumerable<LiveBroadcast>> GetMyBroadcasts(BroadcastStatusEnum broadcastStatus = BroadcastStatusEnum.All, int maxResults = 1)
        {
            return await this.YouTubeServiceWrapper(async () =>
            {
                LiveBroadcastsResource.ListRequest request = this.connection.GoogleYouTubeService.LiveBroadcasts.List("snippet,contentDetails,status");
                request.BroadcastStatus = broadcastStatus;
                request.BroadcastType = BroadcastTypeEnum.All;
                request.Mine = true;
                request.MaxResults = 1;

                LiveBroadcastListResponse response = await request.ExecuteAsync();
                if (response.Items.Count > 0)
                {
                    return response.Items;
                }
                return new List<LiveBroadcast>();
            });
        }

        /// <summary>
        /// Updates the specified broadcast.
        /// </summary>
        /// <param name="broadcast">The broadcast to update</param>
        /// <returns>The updated broadcast</returns>
        public async Task<LiveBroadcast> UpdateBroadcast(LiveBroadcast broadcast)
        {
            Validator.ValidateVariable(broadcast, "broadcast");
            return await this.YouTubeServiceWrapper(async () =>
            {
                LiveBroadcastsResource.UpdateRequest request = this.connection.GoogleYouTubeService.LiveBroadcasts.Update(broadcast, "id,snippet,contentDetails,status");
                return await request.ExecuteAsync();
            });
        }

        /// <summary>
        /// Enables/Disables the slate for a live broadcast.
        /// </summary>
        /// <param name="broadcast">The broadcast to update</param>
        /// <param name="showSlate">Indicates whether to show or hide the slate</param>
        /// <returns>An awaitable Task</returns>
        public async Task EnableDisableSlate(LiveBroadcast broadcast, bool showSlate)
        {
            Validator.ValidateVariable(broadcast, "broadcast");
            await this.YouTubeServiceWrapper<object>(async () =>
            {
                LiveBroadcastsResource.ControlRequest request = this.connection.GoogleYouTubeService.LiveBroadcasts.Control(broadcast.Id, "snippet,contentDetails");
                request.DisplaySlate = showSlate;
                await request.ExecuteAsync();
                return null;
            });
        }

        /// <summary>
        /// Starts an ad break for the live broadcast.
        /// </summary>
        /// <param name="broadcast">The broadcast of the live broadcast</param>
        /// <param name="duration">The duration of the ad break</param>
        /// <returns>Information about the ad break</returns>
        public async Task<LiveCuepoint> StartAdBreak(LiveBroadcast broadcast, long duration)
        {
            Validator.ValidateVariable(broadcast, "broadcast");
            return await this.YouTubeServiceWrapper(async () =>
            {
                LiveCuepoint liveCuepoint = new LiveCuepoint();
                liveCuepoint.BroadcastId = broadcast.Id;
                liveCuepoint.Settings = new CuepointSettings();
                liveCuepoint.Settings.CueType = "ad";
                liveCuepoint.Settings.DurationSecs = duration;

                LiveCuepointsResource.InsertRequest request = this.connection.GoogleYouTubePartnerService.LiveCuepoints.Insert(liveCuepoint, broadcast.Snippet.ChannelId);
                return await request.ExecuteAsync();
            });
        }
    }
}
