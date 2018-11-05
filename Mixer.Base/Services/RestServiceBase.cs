using Mixer.Base.Model.OAuth;
using Mixer.Base.Util;
using Mixer.Base.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Mixer.Base.Services
{
    /// <summary>
    /// The abstract class in charge of handling RESTful requests.
    /// </summary>
    public abstract class RestServiceBase
    {
        private const string RequestLastPageRegexString = "page=[\\d]+>; rel=\"last\"";

        /// <summary>
        /// This event occurs when a RESTful request is sent.
        /// </summary>
        public event EventHandler<Tuple<string, HttpContent>> OnRequestSent;
        /// <summary>
        /// This event occurs when a successful response is received.
        /// </summary>
        public event EventHandler<string> OnSuccessResponseReceived;
        /// <summary>
        /// This event occurs when a failed response is received.
        /// </summary>
        public event EventHandler<RestServiceRequestException> OnFailureResponseReceived;

        /// <summary>
        /// Creates an instance of the RestServiceBase.
        /// </summary>
        public RestServiceBase() { }

        /// <summary>
        /// Performs a GET REST request using the provided request URI.
        /// </summary>
        /// <param name="requestUri">The request URI to use</param>
        /// <returns>A response message of the request</returns>
        public async Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            using (HttpClientWrapper client = await this.GetHttpClient())
            {
                this.LogRequest(requestUri);
                return await client.GetAsync(requestUri);
            }
        }

        /// <summary>
        /// Performs a GET REST request using the provided request URI.
        /// </summary>
        /// <param name="requestUri">The request URI to use</param>
        /// <returns>A type-casted object of the contents of the response</returns>
        public async Task<T> GetAsync<T>(string requestUri)
        {
            return await this.ProcessResponse<T>(await this.GetAsync(requestUri));
        }

        /// <summary>
        /// Performs a GET REST request using the provided request URI.
        /// </summary>
        /// <param name="requestUri">The request URI to use</param>
        /// <returns>A JObject of the contents of the response</returns>
        public async Task<JObject> GetJObjectAsync(string requestUri)
        {
            return await this.ProcessJObjectResponse(await this.GetAsync(requestUri));
        }

        /// <summary>
        /// Performs a GET REST request using the provided request URI.
        /// </summary>
        /// <param name="requestUri">The request URI to use</param>
        /// <returns>A string of the contents of the response</returns>
        public async Task<string> GetStringAsync(string requestUri)
        {
            return await this.ProcessStringResponse(await this.GetAsync(requestUri));
        }

        /// <summary>
        /// Performs a GET REST request using the provided request URI for paged results.
        /// </summary>
        /// <param name="requestUri">The request URI to use</param>
        /// <param name="maxResults">The maximum results to return. The total results returned can exceed this value if more results are found within the pages acquired.</param>
        /// <param name="linkPagesAvailable">Whether the link pages header property exists</param>
        /// <returns>A type-casted list of objects of the contents of the response</returns>
        public async Task<IEnumerable<T>> GetPagedAsync<T>(string requestUri, uint maxResults = 1, bool linkPagesAvailable = true)
        {
            List<T> results = new List<T>();
            int currentPage = 0;
            int pageTotal = 0;

            while (currentPage <= pageTotal && results.Count < maxResults)
            {
                string currentRequestUri = requestUri;
                if (pageTotal > 0)
                {
                    if (currentRequestUri.Contains("?"))
                    {
                        currentRequestUri += "&";
                    }
                    else
                    {
                        currentRequestUri += "?";
                    }
                    currentRequestUri += "page=" + currentPage;
                }
                HttpResponseMessage response = await this.GetAsync(currentRequestUri);

                T[] pagedResults = await this.ProcessResponse<T[]>(response);
                results.AddRange(pagedResults);
                currentPage++;

                if (linkPagesAvailable)
                {
                    IEnumerable<string> linkValues;
                    if (response.Headers.TryGetValues("link", out linkValues))
                    {
                        Regex regex = new Regex(RequestLastPageRegexString);
                        Match match = regex.Match(linkValues.First());
                        if (match != null && match.Success)
                        {
                            string matchValue = match.Captures[0].Value;
                            matchValue = matchValue.Substring(5);
                            matchValue = matchValue.Substring(0, matchValue.IndexOf('>'));
                            pageTotal = int.Parse(matchValue);
                        }
                    }
                }
                else if (pagedResults.Count() > 0)
                {
                    pageTotal++;
                }
            }

            return results;
        }

        /// <summary>
        /// Performs a POST REST request using the provided request URI.
        /// </summary>
        /// <param name="requestUri">The request URI to use</param>
        /// <param name="content">The content to send</param>
        /// <param name="autoRefreshToken">Whether to auto refresh the OAuth token</param>
        /// <returns>A response message of the request</returns>
        public async Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content, bool autoRefreshToken = true)
        {
            using (HttpClientWrapper client = await this.GetHttpClient(autoRefreshToken))
            {
                this.LogRequest(requestUri, content);
                return await client.PostAsync(requestUri, content);
            }
        }

        /// <summary>
        /// Performs a POST REST request using the provided request URI.
        /// </summary>
        /// <param name="requestUri">The request URI to use</param>
        /// <param name="content">The content to send</param>
        /// <param name="autoRefreshToken">Whether to auto refresh the OAuth token</param>
        /// <returns>A type-casted object of the contents of the response</returns>
        public async Task<T> PostAsync<T>(string requestUri, HttpContent content, bool autoRefreshToken = true)
        {
            return await this.ProcessResponse<T>(await this.PostAsync(requestUri, content, autoRefreshToken));
        }

        /// <summary>
        /// Performs a PUT REST request using the provided request URI.
        /// </summary>
        /// <param name="requestUri">The request URI to use</param>
        /// <param name="content">The content to send</param>
        /// <returns>A response message of the request</returns>
        public async Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content)
        {
            using (HttpClientWrapper client = await this.GetHttpClient())
            {
                this.LogRequest(requestUri, content);
                return await client.PutAsync(requestUri, content);
            }
        }

        /// <summary>
        /// Performs a PUT REST request using the provided request URI.
        /// </summary>
        /// <param name="requestUri">The request URI to use</param>
        /// <param name="content">The content to send</param>
        /// <returns>A type-casted object of the contents of the response</returns>
        public async Task<T> PutAsync<T>(string requestUri, HttpContent content)
        {
            return await this.ProcessResponse<T>(await this.PutAsync(requestUri, content));
        }

        /// <summary>
        /// Performs a PATCH REST request using the provided request URI.
        /// </summary>
        /// <param name="requestUri">The request URI to use</param>
        /// <param name="content">The content to send</param>
        /// <returns>A response message of the request</returns>
        public async Task<HttpResponseMessage> PatchAsync(string requestUri, HttpContent content)
        {
            using (HttpClientWrapper client = await this.GetHttpClient())
            {
                HttpMethod method = new HttpMethod("PATCH");
                HttpRequestMessage request = new HttpRequestMessage(method, requestUri) { Content = content };
                this.LogRequest(requestUri, content);
                return await client.SendAsync(request);
            }
        }

        /// <summary>
        /// Performs a PATCH REST request using the provided request URI.
        /// </summary>
        /// <param name="requestUri">The request URI to use</param>
        /// <param name="content">The content to send</param>
        /// <returns>A type-casted object of the contents of the response</returns>
        public async Task<T> PatchAsync<T>(string requestUri, HttpContent content)
        {
            return await this.ProcessResponse<T>(await this.PatchAsync(requestUri, content));
        }

        /// <summary>
        /// Performs a DELETE REST request using the provided request URI.
        /// </summary>
        /// <param name="requestUri">The request URI to use</param>
        /// <returns>Whether the deletion was successful</returns>
        public async Task<bool> DeleteAsync(string requestUri)
        {
            using (HttpClientWrapper client = await this.GetHttpClient())
            {
                this.LogRequest(requestUri);
                HttpResponseMessage response = await client.DeleteAsync(requestUri);
                return (response.StatusCode == HttpStatusCode.NoContent);
            }
        }

        /// <summary>
        /// Creates an HttpContent object from the specified object.
        /// </summary>
        /// <param name="obj">The object to serialize</param>
        /// <returns>The HttpContent containing the serialized object</returns>
        public HttpContent CreateContentFromObject(object obj) { return this.CreateContentFromString(JsonConvert.SerializeObject(obj)); }

        /// <summary>
        /// Creates an HttpContent object from the specified string.
        /// </summary>
        /// <param name="str">The string to serialize</param>
        /// <returns>The HttpContent containing the serialized string</returns>
        public HttpContent CreateContentFromString(string str) { return new StringContent(str, Encoding.UTF8, "application/json"); }

        /// <summary>
        /// HTTP encodes the specified string.
        /// </summary>
        /// <param name="str">The string to encode</param>
        /// <returns>The HTTP encoded string</returns>
        public string EncodeString(string str) { return HttpUtility.UrlEncode(str); }

        /// <summary>
        /// Processes and deserializes the HttpResponse into a type-casted object.
        /// </summary>
        /// <param name="response">The HttpResponse to process</param>
        /// <returns>A type-casted object of the contents of the response</returns>
        public async Task<T> ProcessResponse<T>(HttpResponseMessage response)
        {
            return JsonConvert.DeserializeObject<T>(await this.ProcessStringResponse(response));
        }

        /// <summary>
        /// Processes and deserializes the HttpResponse into a JObject.
        /// </summary>
        /// <param name="response">The HttpResponse to process</param>
        /// <returns>A JObject of the contents of the response</returns>
        public async Task<JObject> ProcessJObjectResponse(HttpResponseMessage response)
        {
            return JObject.Parse(await this.ProcessStringResponse(response));
        }

        /// <summary>
        /// Processes and deserializes the HttpResponse into a string.
        /// </summary>
        /// <param name="response">The HttpResponse to process</param>
        /// <returns>A string of the contents of the response</returns>
        public async Task<string> ProcessStringResponse(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created)
            {
                string result = await response.Content.ReadAsStringAsync();
                if (this.OnSuccessResponseReceived != null)
                {
                    this.OnSuccessResponseReceived(this, result);
                }
                return result;
            }
            else
            {
                RestServiceRequestException ex = new RestServiceRequestException(response);
                if (this.OnFailureResponseReceived != null)
                {
                    this.OnFailureResponseReceived(this, ex);
                }
                throw ex;
            }
        }

        /// <summary>
        /// Gets the OAuth token for the connection of this service.
        /// </summary>
        /// <param name="autoRefreshToken">Whether to automatically refresh the OAuth token or not if it has to be</param>
        /// <returns>The OAuth token for the connection</returns>
        protected abstract Task<OAuthTokenModel> GetOAuthToken(bool autoRefreshToken = true);

        /// <summary>
        /// Gets the base address for all RESTful calls for this service.
        /// </summary>
        /// <returns>The base address for all RESTful calls</returns>
        protected abstract string GetBaseAddress();

        /// <summary>
        /// Gets the HttpClient using the OAuth for the connection of this service.
        /// </summary>
        /// <param name="autoRefreshToken">Whether to automatically refresh the OAuth token or not if it has to be</param>
        /// <returns>The HttpClient for the connection</returns>
        protected virtual async Task<HttpClientWrapper> GetHttpClient(bool autoRefreshToken = true)
        {
            OAuthTokenModel token = await this.GetOAuthToken(autoRefreshToken);
            if (token != null)
            {
                return new HttpClientWrapper(this.GetBaseAddress(), token);
            }
            return new HttpClientWrapper(this.GetBaseAddress());
        }

        private void LogRequest(string requestUri, HttpContent content = null)
        {
            if (this.OnRequestSent != null)
            {
                this.OnRequestSent(this, new Tuple<string, HttpContent>(requestUri, content));
            }
        }
    }
}