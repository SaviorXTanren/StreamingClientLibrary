using Mixer.Base.Model;
using Mixer.Base.Web;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Mixer.Base.API
{
    public static class ChannelsService
    {
        public static async Task<Channel> GetChannel(AuthorizationToken token, string channelName)
        {
            if (token == null)
            {
                throw new ArgumentException("Token is null");
            }

            if (string.IsNullOrEmpty(channelName))
            {
                throw new ArgumentException("Channel Name must have a valid value");
            }

            using (HttpClientWrapper client = new HttpClientWrapper(token))
            {
                HttpResponseMessage response = await client.GetAsync("channels/" + channelName);
                string result = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    JObject jobject = JObject.Parse(result);
                    return null;
                }
                else
                {
                    throw new HttpRequestException(string.Format("{0} ({1}) - {2}", (int)response.StatusCode, response.ReasonPhrase, result));
                }
            }
        }
    }
}
