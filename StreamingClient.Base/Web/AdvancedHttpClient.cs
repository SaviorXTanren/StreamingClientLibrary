using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StreamingClient.Base.Model.OAuth;
using StreamingClient.Base.Util;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace StreamingClient.Base.Web
{
    /// <summary>
    /// An advanced Http client.
    /// </summary>
    public class AdvancedHttpClient : HttpClient
    {
        /// <summary>
        /// Creates an HttpContent object from the specified object.
        /// </summary>
        /// <param name="obj">The object to serialize</param>
        /// <returns>The HttpContent containing the serialized object</returns>
        public static HttpContent CreateContentFromObject(object obj) { return AdvancedHttpClient.CreateContentFromString(JsonConvert.SerializeObject(obj)); }

        /// <summary>
        /// Creates an HttpContent object from the specified string.
        /// </summary>
        /// <param name="str">The string to serialize</param>
        /// <returns>The HttpContent containing the serialized string</returns>
        public static HttpContent CreateContentFromString(string str) { return new StringContent(str, Encoding.UTF8, "application/json"); }

        /// <summary>
        /// HTTP encodes the specified string.
        /// </summary>
        /// <param name="str">The string to encode</param>
        /// <returns>The HTTP encoded string</returns>
        public static string EncodeString(string str) { return HttpUtility.UrlEncode(str); }

        /// <summary>
        /// Creates a new instance of the JSONHttpClient.
        /// </summary>
        public AdvancedHttpClient()
            : base()
        {
            this.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// Creates a new instance of the JSONHttpClient with a specified base address.
        /// </summary>
        /// <param name="baseAddress">The base address to use for communication</param>
        public AdvancedHttpClient(string baseAddress)
            : this()
        {
            this.BaseAddress = new Uri(baseAddress);
        }

        /// <summary>
        /// Creates a new instance of the JSONHttpClient with a specified base address &amp; Basic authorization value.
        /// </summary>
        /// <param name="baseAddress">The base address to use for communication</param>
        /// <param name="basicAuthorizationValue">The basic value to include in the authorization header</param>
        public AdvancedHttpClient(string baseAddress, string basicAuthorizationValue) : this(baseAddress, "Basic", basicAuthorizationValue) { }

        /// <summary>
        /// Creates a new instance of the JSONHttpClient with a specified base address &amp; custom authorization type.
        /// </summary>
        /// <param name="baseAddress">The base address to use for communication</param>
        /// <param name="authorizationType">The type of authorization to include in the header</param>
        /// <param name="authorizationValue">The value to include in the authorization header</param>
        public AdvancedHttpClient(string baseAddress, string authorizationType, string authorizationValue)
            : this(baseAddress)
        {
            this.DefaultRequestHeaders.Add("Authorization", authorizationType + " " + authorizationValue);
        }

        /// <summary>
        /// Creates a new instance of the JSONHttpClient with a specified base address &amp; OAuth token.
        /// </summary>
        /// <param name="baseAddress">The base address to use for communication</param>
        /// <param name="token">The OAuth token to include in the authentication header</param>
        public AdvancedHttpClient(string baseAddress, OAuthTokenModel token)
            : this(baseAddress)
        {
            this.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.accessToken);
        }

        /// <summary>
        /// Performs a GET REST request using the provided request URI.
        /// </summary>
        /// <param name="requestUri">The request URI to use</param>
        /// <returns>A response message of the request</returns>
        public new async Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            this.LogRequest(requestUri);
            return await base.GetAsync(requestUri);
        }

        /// <summary>
        /// Performs a GET REST request using the provided request URI.
        /// </summary>
        /// <param name="requestUri">The request URI to use</param>
        /// <returns>A type-casted object of the contents of the response</returns>
        public async Task<T> GetAsync<T>(string requestUri)
        {
            return await (await this.GetAsync(requestUri)).ProcessResponse<T>();
        }

        /// <summary>
        /// Performs a GET REST request using the provided request URI.
        /// </summary>
        /// <param name="requestUri">The request URI to use</param>
        /// <returns>A JObject of the contents of the response</returns>
        public async Task<JObject> GetJObjectAsync(string requestUri)
        {
            return await (await this.GetAsync(requestUri)).ProcessJObjectResponse();
        }

        /// <summary>
        /// Performs a GET REST request using the provided request URI.
        /// </summary>
        /// <param name="requestUri">The request URI to use</param>
        /// <returns>A string of the contents of the response</returns>
        public new async Task<string> GetStringAsync(string requestUri)
        {
            return await (await this.GetAsync(requestUri)).ProcessStringResponse();
        }

        /// <summary>
        /// Performs a POST REST request using the provided request URI.
        /// </summary>
        /// <param name="requestUri">The request URI to use</param>
        /// <returns>A type-casted object of the contents of the response</returns>
        public async Task<T> PostAsync<T>(string requestUri)
        {
            this.LogRequest(requestUri);
            return await this.PostAsync<T>(requestUri, AdvancedHttpClient.CreateContentFromString(string.Empty));
        }

        /// <summary>
        /// Performs a POST REST request using the provided request URI.
        /// </summary>
        /// <param name="requestUri">The request URI to use</param>
        /// <param name="content">The content to send</param>
        /// <returns>A response message of the request</returns>
        public new async Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
        {
            this.LogRequest(requestUri, content);
            return await base.PostAsync(requestUri, content);
        }

        /// <summary>
        /// Performs a POST REST request using the provided request URI.
        /// </summary>
        /// <param name="requestUri">The request URI to use</param>
        /// <param name="content">The content to send</param>
        /// <returns>A type-casted object of the contents of the response</returns>
        public async Task<T> PostAsync<T>(string requestUri, HttpContent content)
        {
            return await (await this.PostAsync(requestUri, content)).ProcessResponse<T>();
        }

        /// <summary>
        /// Performs a POST REST request encoded as a Form URL using the provided request URI.
        /// </summary>
        /// <param name="requestUri">The request URI to use</param>
        /// <param name="contentList">The list of key-value pairs to send</param>
        /// <returns>A type-casted object of the contents of the response</returns>
        public async Task<T> PostFormUrlEncodedAsync<T>(string requestUri, List<KeyValuePair<string, string>> contentList)
        {
            using (var content = new FormUrlEncodedContent(contentList))
            {
                content.Headers.Clear();
                content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                return await (await this.PostAsync(requestUri, content)).ProcessResponse<T>();
            }
        }

        /// <summary>
        /// Performs a PUT REST request using the provided request URI.
        /// </summary>
        /// <param name="requestUri">The request URI to use</param>
        /// <returns>A type-casted object of the contents of the response</returns>
        public async Task<T> PutAsync<T>(string requestUri)
        {
            this.LogRequest(requestUri);
            return await this.PutAsync<T>(requestUri, AdvancedHttpClient.CreateContentFromString(string.Empty));
        }

        /// <summary>
        /// Performs a PUT REST request using the provided request URI.
        /// </summary>
        /// <param name="requestUri">The request URI to use</param>
        /// <param name="content">The content to send</param>
        /// <returns>A response message of the request</returns>
        public new async Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content)
        {
            this.LogRequest(requestUri, content);
            return await base.PutAsync(requestUri, content);
        }

        /// <summary>
        /// Performs a PUT REST request using the provided request URI.
        /// </summary>
        /// <param name="requestUri">The request URI to use</param>
        /// <param name="content">The content to send</param>
        /// <returns>A type-casted object of the contents of the response</returns>
        public async Task<T> PutAsync<T>(string requestUri, HttpContent content)
        {
            return await (await this.PutAsync(requestUri, content)).ProcessResponse<T>();
        }

        /// <summary>
        /// Performs a PATCH REST request using the provided request URI.
        /// </summary>
        /// <param name="requestUri">The request URI to use</param>
        /// <param name="content">The content to send</param>
        /// <returns>A response message of the request</returns>
        public async Task<HttpResponseMessage> PatchAsync(string requestUri, HttpContent content)
        {
            HttpMethod method = new HttpMethod("PATCH");
            HttpRequestMessage request = new HttpRequestMessage(method, requestUri) { Content = content };
            this.LogRequest(requestUri, content);
            return await base.SendAsync(request);
        }

        /// <summary>
        /// Performs a PATCH REST request using the provided request URI.
        /// </summary>
        /// <param name="requestUri">The request URI to use</param>
        /// <param name="content">The content to send</param>
        /// <returns>A type-casted object of the contents of the response</returns>
        public async Task<T> PatchAsync<T>(string requestUri, HttpContent content)
        {
            return await (await this.PatchAsync(requestUri, content)).ProcessResponse<T>();
        }

        /// <summary>
        /// Performs a DELETE REST request using the provided request URI.
        /// </summary>
        /// <param name="requestUri">The request URI to use</param>
        /// <param name="content">The content to send</param>
        /// <returns>Whether the deletion was successful</returns>
        public async Task<bool> DeleteAsync(string requestUri, HttpContent content = null)
        {
            HttpResponseMessage response = await this.DeleteAsyncWithResponse(requestUri, content);
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Performs a DELETE REST request using the provided request URI.
        /// </summary>
        /// <param name="requestUri">The request URI to use</param>
        /// <param name="content">The content to send</param>
        /// <returns>A type-casted object of the contents of the response</returns>
        public async Task<T> DeleteAsync<T>(string requestUri, HttpContent content = null)
        {
            HttpResponseMessage response = await this.DeleteAsyncWithResponse(requestUri, content);
            return await response.ProcessResponse<T>();
        }

        /// <summary>
        /// Performs a DELETE REST request using the provided request URI.
        /// </summary>
        /// <param name="requestUri">The request URI to use</param>
        /// <param name="content">The content to send</param>
        /// <returns>A response message of the request</returns>
        public async Task<HttpResponseMessage> DeleteAsyncWithResponse(string requestUri, HttpContent content = null)
        {
            this.LogRequest(requestUri);
            if (content != null)
            {
                HttpMethod method = new HttpMethod("DELETE");
                HttpRequestMessage request = new HttpRequestMessage(method, requestUri) { Content = content };
                return await base.SendAsync(request);
            }
            else
            {
                return await base.DeleteAsync(requestUri);
            }
        }

        private void LogRequest(string requestUri, HttpContent content = null)
        {
            if (content != null)
            {
                try
                {
                    Logger.Log(LogLevel.Debug, string.Format("Rest API Request: {0} - {1}", requestUri, content.ReadAsStringAsync().Result));
                }
                catch (Exception) { }
            }
            else
            {
                Logger.Log(LogLevel.Debug, string.Format("Rest API Request: {0}", requestUri));
            }
        }
    }
}
