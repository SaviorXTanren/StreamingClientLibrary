using Mixer.Base.Model.Channel;
using Mixer.Base.Model.TestStreams;
using Mixer.Base.Util;
using System.Threading.Tasks;

namespace Mixer.Base.Services
{
    /// <summary>
    /// The APIs for test stream-based services.
    /// </summary>
    public class TestStreamsService : MixerServiceBase
    {
        /// <summary>
        /// Creates an instance of the TestStreamsService.
        /// </summary>
        /// <param name="connection">The Mixer connection to use</param>
        public TestStreamsService(MixerConnection connection) : base(connection) { }

        /// <summary>
        /// Gets the current test stream settings for the specified channel.
        /// </summary>
        /// <param name="channel">The channel to get test stream settings for</param>
        /// <returns>The test stream settings for the specified channel</returns>
        public async Task<TestStreamSettingsModel> GetSettings(ChannelModel channel)
        {
            Validator.ValidateVariable(channel, "channel");
            return await this.GetAsync<TestStreamSettingsModel>("testStreams/" + channel.id);
        }

        /// <summary>
        /// Updates the test stream settings for the specified channel.
        /// </summary>
        /// <param name="channel">The channel to update test stream settings for</param>
        /// <param name="settings">The test stream settings to set for the channel</param>
        /// <returns>The updated test stream settings for the specified channel</returns>
        public async Task<TestStreamSettingsModel> UpdateSettings(ChannelModel channel, TestStreamSettingsModel settings)
        {
            Validator.ValidateVariable(channel, "channel");
            Validator.ValidateVariable(settings, "settings");
            return await this.PutAsync<TestStreamSettingsModel>("testStreams/" + channel.id, this.CreateContentFromObject(settings));
        }
    }
}
