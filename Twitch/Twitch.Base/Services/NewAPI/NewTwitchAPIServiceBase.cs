using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Twitch.Base.Models.NewAPI;

namespace Twitch.Base.Services.NewAPI
{
    /// <summary>
    /// REST services against the New Twitch API
    /// </summary>
    public class NewTwitchAPIServiceBase : TwitchServiceBase
    {
        /// <summary>
        /// The New Twitch API base address.
        /// </summary>
        public const string BASE_ADDRESS = "https://api.twitch.tv/helix/";

        /// <summary>
        /// Creates an instance of the NewTwitchAPIServiceBase.
        /// </summary>
        /// <param name="connection">The Twitch connection to use</param>
        public NewTwitchAPIServiceBase(TwitchConnection connection) : base(connection, BASE_ADDRESS) { }

        /// <summary>
        /// Performs a GET REST request using the provided request URI for New Twitch API-wrapped data.
        /// </summary>
        /// <param name="requestUri">The request URI to use</param>
        /// <returns>A type-casted object of the contents of the response</returns>
        public async Task<IEnumerable<T>> GetDataResultAsync<T>(string requestUri)
        {
            NewTwitchAPIDataRestResult<T> result = await this.GetAsync<NewTwitchAPIDataRestResult<T>>(requestUri);
            if (result != null && result.data.Count > 0)
            {
                return result.data;
            }
            return new List<T>();
        }

        /// <summary>
        /// Performs a GET REST request using the provided request URI for New Twitch API-wrapped data to get total count.
        /// </summary>
        /// <param name="requestUri">The request URI to use</param>
        /// <returns>The total count of the response</returns>
        public async Task<long> GetPagedResultTotalCountAsync(string requestUri)
        {
            if (!requestUri.Contains("?"))
            {
                requestUri += "?";
            }
            else
            {
                requestUri += "&";
            }
            requestUri += "first=1";

            JObject data = await this.GetJObjectAsync(requestUri);
            if (data != null && data.ContainsKey("total"))
            {
                return (long)data["total"];
            }
            return 0;
        }

        /// <summary>
        /// Performs a GET REST request using the provided request URI for New Twitch API-wrapped data.
        /// </summary>
        /// <param name="requestUri">The request URI to use</param>
        /// <param name="maxResults">The maximum number of results. Will be either that amount or slightly more</param>
        /// <returns>A type-casted object of the contents of the response</returns>
        public async Task<IEnumerable<T>> GetPagedDataResultAsync<T>(string requestUri, int maxResults = 1)
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
            queryParameters.Add("first", ((maxResults > 100) ? 100 : maxResults).ToString());

            List<T> results = new List<T>();
            string cursor = null;
            do
            {
                if (!string.IsNullOrEmpty(cursor))
                {
                    queryParameters["after"] = cursor;
                }
                NewTwitchAPIDataRestResult<T> data = await this.GetAsync<NewTwitchAPIDataRestResult<T>>(requestUri + string.Join("&", queryParameters.Select(kvp => kvp.Key + "=" + kvp.Value)));

                cursor = null;
                if (data != null && data.data != null && data.data.Count > 0)
                {
                    results.AddRange(data.data);
                    cursor = data.Cursor;
                }
            }
            while (results.Count < maxResults && !string.IsNullOrEmpty(cursor));

            return results;
        }

        /// <summary>
        /// Performs a GET REST request using the provided request URI for New Twitch API-wrapped data that has array of segments.
        /// </summary>
        /// <param name="requestUri">The request URI to use</param>
        /// <param name="maxResults">The maximum number of results. Will be either that amount or slightly more</param>
        /// <returns>A type-casted object of the contents of the response</returns>
        public async Task<T> GetPagedDataSegmentsResultAsync<T>(string requestUri, int maxResults = 1)
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
            queryParameters.Add("first", ((maxResults > 100) ? 100 : maxResults).ToString());

            dynamic results = (T)Activator.CreateInstance(typeof(T));
            bool firstPass = true;
            PropertyInfo segmentsInfo = null;
            bool segmentsIsList = false;
            string cursor = null;

            do
            {
                if (!string.IsNullOrEmpty(cursor))
                {
                    queryParameters["after"] = cursor;
                }
                dynamic data = await this.GetAsync<NewTwitchAPIDataRestResult2<T>>(requestUri + string.Join("&", queryParameters.Select(kvp => kvp.Key + "=" + kvp.Value)));

                if (firstPass)
                {
                    results = data.data;
                    segmentsInfo = results.GetType().GetProperty("segments");
                    segmentsIsList = segmentsInfo.PropertyType.IsGenericType && segmentsInfo.PropertyType.GetGenericTypeDefinition() == typeof(List<>);
                    firstPass = false;
                    if (segmentsInfo == null && !segmentsIsList)
                        break;
                }
                else
                {
                    dynamic segments = segmentsInfo.GetValue(data.data);
                    cursor = null;
                    if (data != null && data.data.segments != null && data.data.segments.Count > 0)
                    {
                        results.segments.AddRange(data.data.segments);
                        cursor = data.Cursor;
                    }
                }
            }
            while (results.segments.Count < maxResults && !string.IsNullOrEmpty(cursor));

            return results;
        }

        /// <summary>
        /// Performs a POST REST request using the provided request URI for New Twitch API-wrapped data.
        /// </summary>
        /// <param name="requestUri">The request URI to use</param>
        /// <returns>A type-casted object of the contents of the response</returns>
        public async Task<T[]> PostDataResultAsync<T>(string requestUri)
        {
            NewTwitchAPIDataRestResult<T> result = await this.PostAsync<NewTwitchAPIDataRestResult<T>>(requestUri);
            if (result != null && result.data.Count > 0)
            {
                return result.data.ToArray();
            }
            return null;
        }

        /// <summary>
        /// Performs a POST REST request using the provided request URI for New Twitch API-wrapped data.
        /// </summary>
        /// <param name="requestUri">The request URI to use</param>
        /// <param name="content">The post body content</param>
        /// <returns>A type-casted object of the contents of the response</returns>
        public async Task<T[]> PostDataResultAsync<T>(string requestUri, HttpContent content)
        {
            NewTwitchAPIDataRestResult<T> result = await this.PostAsync<NewTwitchAPIDataRestResult<T>>(requestUri, content);
            if (result != null && result.data.Count > 0)
            {
                return result.data.ToArray();
            }
            return null;
        }
    }
}
