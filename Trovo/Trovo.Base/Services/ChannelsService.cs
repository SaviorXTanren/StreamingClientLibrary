using Newtonsoft.Json.Linq;
using StreamingClient.Base.Util;
using StreamingClient.Base.Web;
using System.Net.Http;
using System.Threading.Tasks;
using Trovo.Base.Models.Channels;

namespace Trovo.Base.Services
{
    /// <summary>
    /// The APIs for channel-based services.
    /// </summary>
    public class ChannelsService : TrovoServiceBase
    {
        /// <summary>
        /// Creates an instance of the ChannelService.
        /// </summary>
        /// <param name="connection">The Trovo connection to use</param>
        public ChannelsService(TrovoConnection connection) : base(connection) { }

        public async Task<PrivateChannelModel> GetCurrentChannel()
        {
            return await this.GetAsync<PrivateChannelModel>("channel");
        }

        public async Task<ChannelModel> GetChannel(string id)
        {
            Validator.ValidateString(id, "id");
            ChannelModel channel = await this.GetAsync<ChannelModel>("channels/" + id);
            if (channel != null)
            {
                channel.channel_id = id;
            }
            return channel;
        }

        public async Task<bool> UpdateChannel(string id, string title = null, string categoryID = null, string langaugeCode = null, ChannelAudienceTypeEnum? audience = null)
        {
            Validator.ValidateString(id, "id");

            JObject jobj = new JObject();
            jobj["channel_id"] = id;
            if (!string.IsNullOrEmpty(title)) { jobj["live_title"] = title; }
            if (!string.IsNullOrEmpty(categoryID)) { jobj["category_id"] = categoryID; }
            if (!string.IsNullOrEmpty(langaugeCode)) { jobj["language_code"] = langaugeCode; }
            if (audience != null) { jobj["audi_type"] = audience.ToString(); }

            HttpResponseMessage response = await this.PostAsync("channels/update", AdvancedHttpClient.CreateContentFromObject(jobj));
            return response.IsSuccessStatusCode;
        }
    }
}
