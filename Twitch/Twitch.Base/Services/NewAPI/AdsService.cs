using Newtonsoft.Json.Linq;
using StreamingClient.Base.Util;
using StreamingClient.Base.Web;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Twitch.Base.Models.NewAPI.Ads;
using Twitch.Base.Models.NewAPI.Users;

namespace Twitch.Base.Services.NewAPI
{
    /// <summary>
    /// The APIs for Ads-based services.
    /// </summary>
    public class AdsService : NewTwitchAPIServiceBase
    {
        /// <summary>
        /// Creates an instance of the AdsService.
        /// </summary>
        /// <param name="connection">The Twitch connection to use</param>
        public AdsService(TwitchConnection connection) : base(connection) { }

        /// <summary>
        /// Runs an add for the specified channel.
        /// </summary>
        /// <param name="channel">The channel to run the add on</param>
        /// <param name="length">Desired length of the commercial in seconds. Valid options are 30, 60, 90, 120, 150, 180.</param>
        /// <returns></returns>
        public async Task<AdResponseModel> RunAd(UserModel channel, int length)
        {
            Validator.ValidateVariable(channel, "channel");
            Validator.Validate(length > 0, "length");

            JObject body = new JObject();
            body["broadcaster_id"] = channel.id;
            body["length"] = length;

            JObject result = await this.PostAsync<JObject>("channels/commercial", AdvancedHttpClient.CreateContentFromObject(body));
            if (result != null && result.ContainsKey("data"))
            {
                JArray array = (JArray)result["data"];
                IEnumerable<AdResponseModel> adResult = array.ToTypedArray<AdResponseModel>();
                if (adResult != null)
                {
                    return adResult.FirstOrDefault();
                }
            }
            return null;
        }
    }
}
