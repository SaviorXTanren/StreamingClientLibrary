using Newtonsoft.Json.Linq;
using StreamingClient.Base.Model.OAuth;
using StreamingClient.Base.Util;
using StreamingClient.Base.Web;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Twitch.Base.Services.V5API
{
    /// <summary>
    /// REST services against the Twitch API v5
    /// </summary>
    public class V5APIServiceBase : TwitchServiceBase
    {
        /// <summary>
        /// The New Twitch API base address.
        /// </summary>
        public const string BASE_ADDRESS = "https://api.twitch.tv/kraken/";

        /// <summary>
        /// Creates an instance of the V5APIServiceBase.
        /// </summary>
        /// <param name="connection">The Twitch connection to use</param>
        public V5APIServiceBase(TwitchConnection connection) : base(connection, BASE_ADDRESS) { }

        /// <summary>
        /// Performs a GET REST request using the provided request URI for New Twitch API-wrapped data.
        /// </summary>
        /// <param name="requestUri">The request URI to use</param>
        /// <param name="valueName">The name of the value to look for in the results</param>
        /// <param name="maxResults">The maximum number of results. Will be either that amount or slightly more</param>
        /// <returns>A type-casted object of the contents of the response</returns>
        public async Task<IEnumerable<T>> GetCursorPagedResultAsync<T>(string requestUri, string valueName, int maxResults = 1)
        {
            if (!requestUri.Contains("?"))
            {
                requestUri += "?";
            }
            else
            {
                requestUri += "&";
            }

            Dictionary<string, string> queryParameters = new Dictionary<string, string>();
            queryParameters.Add("limit", ((maxResults > 100) ? 100 : maxResults).ToString());

            List<T> results = new List<T>();
            string cursor = null;
            do
            {
                if (!string.IsNullOrEmpty(cursor))
                {
                    queryParameters["cursor"] = cursor;
                }
                JObject data = await this.GetJObjectAsync(requestUri + string.Join("&", queryParameters.Select(kvp => kvp.Key + "=" + kvp.Value)));

                cursor = null;
                if (data != null && data.ContainsKey("_cursor") && data.ContainsKey(valueName))
                {
                    JArray array = (JArray)data[valueName];
                    results.AddRange(array.ToTypedArray<T>());
                    cursor = data["_cursor"].ToString();
                }
            }
            while (results.Count < maxResults && !string.IsNullOrEmpty(cursor));

            return results;
        }

        /// <summary>
        /// Performs a GET REST request using the provided request URI for New Twitch API-wrapped data.
        /// </summary>
        /// <param name="requestUri">The request URI to use</param>
        /// <param name="valueName">The name of the value to look for in the results</param>
        /// <param name="maxResults">The maximum number of results. Will be either that amount or slightly more</param>
        /// <returns>A type-casted object of the contents of the response</returns>
        public async Task<IEnumerable<T>> GetOffsetPagedResultAsync<T>(string requestUri, string valueName, int maxResults = 1)
        {
            if (!requestUri.Contains("?"))
            {
                requestUri += "?";
            }
            else
            {
                requestUri += "&";
            }

            Dictionary<string, string> queryParameters = new Dictionary<string, string>();
            queryParameters.Add("limit", ((maxResults > 100) ? 100 : maxResults).ToString());

            List<T> results = new List<T>();
            int total = 0;
            int currentTotal = 0;
            do
            {
                if (total > 0)
                {
                    queryParameters["offset"] = total.ToString();
                }
                JObject data = await this.GetJObjectAsync(requestUri + string.Join("&", queryParameters.Select(kvp => kvp.Key + "=" + kvp.Value)));

                currentTotal = 0;
                if (data != null && data.ContainsKey("_total") && data.ContainsKey(valueName))
                {
                    JArray array = (JArray)data[valueName];
                    results.AddRange(array.ToTypedArray<T>());
                    currentTotal = (int)data["_total"];
                    total += currentTotal;
                }
            }
            while (results.Count < maxResults && currentTotal > 0);

            return results;
        }

        /// <summary>
        /// Performs a GET REST request using the provided request URI and returns the list of elements from the specified array name.
        /// </summary>
        /// <param name="requestUri">The request URI to use</param>
        /// <param name="arrayName">The name of the array to look for in the results</param>
        /// <returns>A type-casted object of the contents of the response</returns>
        public async Task<IEnumerable<T>> GetNamedArrayAsync<T>(string requestUri, string arrayName)
        {
            JObject jobj = await this.GetJObjectAsync(requestUri);

            List<T> results = new List<T>();
            if (jobj != null && jobj.ContainsKey(arrayName))
            {
                JArray array = (JArray)jobj[arrayName];
                results.AddRange(array.ToTypedArray<T>());
            }
            return results;
        }

        /// <summary>
        /// Gets the HttpClient using the OAuth for the connection of this service.
        /// </summary>
        /// <param name="autoRefreshToken">Whether to automatically refresh the OAuth token or not if it has to be</param>
        /// <returns>The HttpClient for the connection</returns>
        protected override async Task<AdvancedHttpClient> GetHttpClient(bool autoRefreshToken = true)
        {
            OAuthTokenModel token = await this.GetOAuthToken(autoRefreshToken);

            AdvancedHttpClient client = new AdvancedHttpClient(this.GetBaseAddress());
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.twitchtv.v5+json"));
            client.DefaultRequestHeaders.Add("Client-ID", token.clientID);
            if (token != null)
            {
                client.DefaultRequestHeaders.Add("Authorization", "OAuth " + token.accessToken);
            }

            return client;
        }
    }
}
