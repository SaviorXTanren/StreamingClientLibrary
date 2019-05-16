using System.Collections.Generic;
using System.Threading.Tasks;
using Twitch.Base.Models.V5.Bits;
using Twitch.Base.Models.V5.Channel;

namespace Twitch.Base.Services.V5API
{
    /// <summary>
    /// The APIs for Bits-based services.
    /// </summary>
    public class BitsService : V5APIServiceBase
    {
        /// <summary>
        /// Creates an instance of the BitsService.
        /// </summary>
        /// <param name="connection">The Twitch connection to use</param>
        public BitsService(TwitchConnection connection) : base(connection) { }

        /// <summary>
        /// Gets the list of available cheermotes, including ones for the optionally specified channel.
        /// </summary>
        /// <param name="channel">The optional channel to get cheermotes for</param>
        /// <returns>All of the available cheermotes</returns>
        public async Task<IEnumerable<BitCheermoteModel>> GetActions(ChannelModel channel = null)
        {
            return await this.GetNamedArrayAsync<BitCheermoteModel>("bits/actions" + ((channel != null) ? "?channel_id=" + channel.id : string.Empty), "actions");
        }
    }
}
