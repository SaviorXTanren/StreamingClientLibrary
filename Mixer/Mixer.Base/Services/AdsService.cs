using Mixer.Base.Model.Channel;
using Newtonsoft.Json.Linq;
using StreamingClient.Base.Util;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Mixer.Base.Services
{
    /// <summary>
    /// The APIs for ads-based services.
    /// </summary>
    public class AdsService : MixerServiceBase
    {
        /// <summary>
        /// Creates an instance of the AdsService.
        /// </summary>
        /// <param name="connection">The Mixer connection to use</param>
        public AdsService(MixerConnection connection) : base(connection, 2) { }

        /// <summary>
        /// Runs an ad on the specified channel.
        /// </summary>
        /// <param name="channel">The channel to run the ad for</param>
        /// <returns>Whether the ad was successfully initiated</returns>
        public async Task<bool> RunAd(ChannelModel channel)
        {
            JObject body = new JObject();
            body["requestId"] = Guid.NewGuid().ToString();
            HttpResponseMessage response = await this.PostAsync("ads/channels/" + channel.id, new StringContent(body.ToString()));
            if (!response.IsSuccessStatusCode)
            {
                Logger.Log(new HttpRestRequestException(response));
            }
            return response.IsSuccessStatusCode;
        }
    }
}