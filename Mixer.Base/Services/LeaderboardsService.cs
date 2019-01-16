using Mixer.Base.Model.Channel;
using Mixer.Base.Model.Leaderboards;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mixer.Base.Services
{
    /// <summary>
    /// The APIs for leaderboard-based services.
    /// </summary>
    public class LeaderboardsService : MixerServiceBase
    {
        /// <summary>
        /// Creates an instance of the LeaderboardsService.
        /// </summary>
        /// <param name="connection">The Mixer connection to use</param>
        public LeaderboardsService(MixerConnection connection) : base(connection, version: 2) { }

        /// <summary>
        /// Gets the weekly sparks leaderboard for the specified channel.
        /// </summary>
        /// <param name="channel">The channel to get the leaderboard for</param>
        /// <param name="limit">The number of users to limit the results to.  Max: 100, Default: 10</param>
        /// <returns>The weekly sparks leaderboard for the channel specified</returns>
        public async Task<IEnumerable<SparksLeaderboardModel>> GetWeeklySparksLeaderboard(ChannelModel channel, int limit = 10)
        {
            return await this.GetAsync<IEnumerable<SparksLeaderboardModel>>("leaderboards/sparks-weekly/channels/" + channel.id.ToString() + "?limit=" + limit.ToString());
        }

        /// <summary>
        /// Gets the monthly sparks leaderboard for the specified channel.
        /// </summary>
        /// <param name="channel">The channel to get the leaderboard for</param>
        /// <param name="limit">The number of users to limit the results to.  Max: 100, Default: 10</param>
        /// <returns>The monthly sparks leaderboard for the channel specified</returns>
        public async Task<IEnumerable<SparksLeaderboardModel>> GetMonthlySparksLeaderboard(ChannelModel channel, int limit = 10)
        {
            return await this.GetAsync<IEnumerable<SparksLeaderboardModel>>("leaderboards/sparks-monthly/channels/" + channel.id.ToString() + "?limit=" + limit.ToString());
        }

        /// <summary>
        /// Gets the yearly sparks leaderboard for the specified channel.
        /// </summary>
        /// <param name="channel">The channel to get the leaderboard for</param>
        /// <param name="limit">The number of users to limit the results to.  Max: 100, Default: 10</param>
        /// <returns>The yearly sparks leaderboard for the channel specified</returns>
        public async Task<IEnumerable<SparksLeaderboardModel>> GetYearlySparksLeaderboard(ChannelModel channel, int limit = 10)
        {
            return await this.GetAsync<IEnumerable<SparksLeaderboardModel>>("leaderboards/sparks-yearly/channels/" + channel.id.ToString() + "?limit=" + limit.ToString());
        }

        /// <summary>
        /// Gets the all-time sparks leaderboard for the specified channel.
        /// </summary>
        /// <param name="channel">The channel to get the leaderboard for</param>
        /// <param name="limit">The number of users to limit the results to.  Max: 100, Default: 10</param>
        /// <returns>The all-time sparks leaderboard for the channel specified</returns>
        public async Task<IEnumerable<SparksLeaderboardModel>> GetAllTimeSparksLeaderboard(ChannelModel channel, int limit = 10)
        {
            return await this.GetAsync<IEnumerable<SparksLeaderboardModel>>("leaderboards/sparks-alltime/channels/" + channel.id.ToString() + "?limit=" + limit.ToString());
        }

        /// <summary>
        /// Gets the weekly embers leaderboard for the specified channel.
        /// </summary>
        /// <param name="channel">The channel to get the leaderboard for</param>
        /// <param name="limit">The number of users to limit the results to.  Max: 100, Default: 10</param>
        /// <returns>The weekly embers leaderboard for the channel specified</returns>
        public async Task<IEnumerable<EmbersLeaderboardModel>> GetWeeklyEmbersLeaderboard(ChannelModel channel, int limit = 10)
        {
            return await this.GetAsync<IEnumerable<EmbersLeaderboardModel>>("leaderboards/embers-weekly/channels/" + channel.id.ToString() + "?limit=" + limit.ToString());
        }

        /// <summary>
        /// Gets the monthly embers leaderboard for the specified channel.
        /// </summary>
        /// <param name="channel">The channel to get the leaderboard for</param>
        /// <param name="limit">The number of users to limit the results to.  Max: 100, Default: 10</param>
        /// <returns>The monthly embers leaderboard for the channel specified</returns>
        public async Task<IEnumerable<EmbersLeaderboardModel>> GetMonthlyEmbersLeaderboard(ChannelModel channel, int limit = 10)
        {
            return await this.GetAsync<IEnumerable<EmbersLeaderboardModel>>("leaderboards/embers-monthly/channels/" + channel.id.ToString() + "?limit=" + limit.ToString());
        }

        /// <summary>
        /// Gets the yearly embers leaderboard for the specified channel.
        /// </summary>
        /// <param name="channel">The channel to get the leaderboard for</param>
        /// <param name="limit">The number of users to limit the results to.  Max: 100, Default: 10</param>
        /// <returns>The yearly embers leaderboard for the channel specified</returns>
        public async Task<IEnumerable<EmbersLeaderboardModel>> GetYearlyEmbersLeaderboard(ChannelModel channel, int limit = 10)
        {
            return await this.GetAsync<IEnumerable<EmbersLeaderboardModel>>("leaderboards/embers-yearly/channels/" + channel.id.ToString() + "?limit=" + limit.ToString());
        }

        /// <summary>
        /// Gets the all-time embers leaderboard for the specified channel.
        /// </summary>
        /// <param name="channel">The channel to get the leaderboard for</param>
        /// <param name="limit">The number of users to limit the results to.  Max: 100, Default: 10</param>
        /// <returns>The all-time embers leaderboard for the channel specified</returns>
        public async Task<IEnumerable<EmbersLeaderboardModel>> GetAllTimeEmbersLeaderboard(ChannelModel channel, int limit = 10)
        {
            return await this.GetAsync<IEnumerable<EmbersLeaderboardModel>>("leaderboards/embers-alltime/channels/" + channel.id.ToString() + "?limit=" + limit.ToString());
        }
    }
}
