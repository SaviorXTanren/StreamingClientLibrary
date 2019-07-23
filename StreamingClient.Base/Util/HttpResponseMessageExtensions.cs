using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace StreamingClient.Base.Util
{
    /// <summary>
    /// Extension methods for the HttpResponseMessage class.
    /// </summary>
    public static class HttpResponseMessageExtensions
    {
        /// <summary>
        /// Gets the first value of the specified header if it exists.
        /// </summary>
        /// <param name="response">The HttpResponse to process</param>
        /// <param name="name">The name of the header</param>
        /// <returns>The first value of the specified header if it exists</returns>
        public static string GetHeaderValue(this HttpResponseMessage response, string name)
        {
            if (response.Headers.Contains(name) && response.Headers.TryGetValues(name, out IEnumerable<string> values))
            {
                return values.FirstOrDefault();
            }
            return null;
        }

        /// <summary>
        /// Processes and deserializes the HttpResponse into a type-casted object.
        /// </summary>
        /// <param name="response">The HttpResponse to process</param>
        /// <returns>A type-casted object of the contents of the response</returns>
        public static async Task<T> ProcessResponse<T>(this HttpResponseMessage response)
        {
            return JSONSerializerHelper.DeserializeFromString<T>(await response.ProcessStringResponse());
        }

        /// <summary>
        /// Processes and deserializes the HttpResponse into a JObject.
        /// </summary>
        /// <param name="response">The HttpResponse to process</param>
        /// <returns>A JObject of the contents of the response</returns>
        public static async Task<JObject> ProcessJObjectResponse(this HttpResponseMessage response)
        {
            return JObject.Parse(await response.ProcessStringResponse());
        }

        /// <summary>
        /// Processes and deserializes the HttpResponse into a string.
        /// </summary>
        /// <param name="response">The HttpResponse to process</param>
        /// <returns>A string of the contents of the response</returns>
        public static async Task<string> ProcessStringResponse(this HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                Logger.Log(LogLevel.Debug, result);
                return result;
            }
            else
            {
                HttpRestRequestException ex = new HttpRestRequestException(response);
                Logger.Log(ex);
                throw ex;
            }
        }
    }
}
