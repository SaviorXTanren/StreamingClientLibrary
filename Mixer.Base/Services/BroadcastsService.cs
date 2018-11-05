using Mixer.Base.Model.Broadcast;
using System;
using System.Threading.Tasks;

namespace Mixer.Base.Services
{
    /// <summary>
    /// The APIs for broadcasts-based services.
    /// </summary>
    public class BroadcastsService : MixerServiceBase
    {
        /// <summary>
        /// Creates an instance of the BroadcastsService.
        /// </summary>
        /// <param name="connection">The Mixer connection to use</param>
        public BroadcastsService(MixerConnection connection) : base(connection) { }

        /// <summary>
        /// Gets latest or ongoing broadcast from the current user.
        /// </summary>
        /// <returns>The latest or ongoing broadcast for the current user</returns>
        public async Task<BroadcastModel> GetCurrentBroadcast()
        {
            return await this.GetAsync<BroadcastModel>("broadcasts/current");
        }

        /// <summary>
        /// Gets the broadcast with the specified ID.
        /// </summary>
        /// <param name="broadcastID">The ID of the broadcast</param>
        /// <returns>The broadcast with the specified ID</returns>
        public async Task<BroadcastModel> GetBroadcast(Guid broadcastID)
        {
            return await this.GetAsync<BroadcastModel>("broadcasts/" + broadcastID.ToString());
        }
    }
}
