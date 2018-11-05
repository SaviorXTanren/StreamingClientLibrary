using Mixer.Base.Model.Channel;
using Mixer.Base.Model.Patronage;
using Mixer.Base.Util;
using System;
using System.Threading.Tasks;

namespace Mixer.Base.Services
{
    /// <summary>
    /// The APIs for patronage-based services.
    /// </summary>
    public class PatronageService : MixerServiceBase
    {
        /// <summary>
        /// Creates an instance of the PatronageService.
        /// </summary>
        /// <param name="connection">The Mixer connection to use</param>
        public PatronageService(MixerConnection connection) : base(connection, version: 2) { }

        /// <summary>
        /// Gets the patronage status of the specified channel.
        /// </summary>
        /// <param name="channel">The channel to get patronage status for</param>
        /// <returns>The patronage status</returns>
        public async Task<PatronageStatusModel> GetPatronageStatus(ChannelModel channel)
        {
            Validator.ValidateVariable(channel, "channel");

            return await this.GetAsync<PatronageStatusModel>("levels/patronage/channels/" + channel.id + "/status");
        }

        /// <summary>
        /// Gets the details of the specified patronage period.
        /// </summary>
        /// <param name="patronagePeriodID">The ID of the patronage period</param>
        /// <returns>The details of the patronage period</returns>
        public async Task<PatronagePeriodModel> GetPatronagePeriod(Guid patronagePeriodID)
        {
            Validator.ValidateVariable(patronagePeriodID, "patronagePeriodID");

            return await this.GetAsync<PatronagePeriodModel>("levels/patronage/resources/" + patronagePeriodID);
        }
    }
}
