using Newtonsoft.Json.Linq;
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
                RestServiceRequestException ex = new RestServiceRequestException(response);
                Logger.Log(ex);
                throw ex;
            }
        }
    }
}
