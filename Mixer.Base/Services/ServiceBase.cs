using Mixer.Base.Model.OAuth;
using Mixer.Base.Util;
using Mixer.Base.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Mixer.Base.Services
{
    public abstract class ServiceBase
    {
        private const string RequestLastPageRegexString = "page=[\\d]+>; rel=\"last\"";

        private MixerConnection connection;

        public ServiceBase(MixerConnection connection)
        {
            Validator.ValidateVariable(connection, "connection");
            this.connection = connection;
        }

        internal ServiceBase() { }

        protected async Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            using (HttpClientWrapper client = await this.GetHttpClient())
            {
                return await client.GetAsync(requestUri);
            }
        }

        protected async Task<T> GetAsync<T>(string requestUri)
        {
            return await this.ProcessResponse<T>(await this.GetAsync(requestUri));
        }

        protected async Task<JObject> GetJObjectAsync(string requestUri)
        {
            return await this.ProcessJObjectResponse(await this.GetAsync(requestUri));
        }

        protected async Task<string> GetStringAsync(string requestUri)
        {
            return await this.ProcessStringResponse(await this.GetAsync(requestUri));
        }

        protected async Task<IEnumerable<T>> GetPagedAsync<T>(string requestUri)
        {
            List<T> results = new List<T>();
            int currentPage = 0;
            int pageTotal = 0;

            while (currentPage <= pageTotal)
            {
                string currentRequestUri = requestUri + "?page=" + currentPage;
                HttpResponseMessage response = await this.GetAsync(currentRequestUri);

                if (pageTotal == 0)
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

                T[] pagedResults = await this.ProcessResponse<T[]>(response);
                results.AddRange(pagedResults);
                currentPage++;
            }

            return results;
        }

        protected async Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
        {
            using (HttpClientWrapper client = await this.GetHttpClient())
            {
                return await client.PostAsync(requestUri, content);
            }
        }

        protected async Task<T> PostAsync<T>(string requestUri, HttpContent content)
        {
            return await this.ProcessResponse<T>(await this.PostAsync(requestUri, content));
        }

        protected async Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content)
        {
            using (HttpClientWrapper client = await this.GetHttpClient())
            {
                return await client.PutAsync(requestUri, content);
            }
        }

        protected async Task<T> PutAsync<T>(string requestUri, HttpContent content)
        {
            return await this.ProcessResponse<T>(await this.PutAsync(requestUri, content));
        }

        protected async Task<HttpResponseMessage> PatchAsync(string requestUri, HttpContent content)
        {
            using (HttpClientWrapper client = await this.GetHttpClient())
            {
                HttpMethod method = new HttpMethod("PATCH");
                HttpRequestMessage request = new HttpRequestMessage(method, requestUri) { Content = content };
                return await client.SendAsync(request);
            }
        }

        protected async Task<T> PatchAsync<T>(string requestUri, HttpContent content)
        {
            return await this.ProcessResponse<T>(await this.PatchAsync(requestUri, content));
        }

        protected async Task<bool> DeleteAsync(string requestUri)
        {
            using (HttpClientWrapper client = await this.GetHttpClient())
            {
                HttpResponseMessage response = await client.DeleteAsync(requestUri);
                return (response.StatusCode == HttpStatusCode.NoContent);
            }
        }

        protected HttpContent CreateContentFromObject(object obj) { return this.CreateContentFromString(JsonConvert.SerializeObject(obj)); }

        protected HttpContent CreateContentFromString(string str) { return new StringContent(str, Encoding.UTF8, "application/json"); }

        private async Task<T> ProcessResponse<T>(HttpResponseMessage response)
        {
            return JsonConvert.DeserializeObject<T>(await this.ProcessStringResponse(response));
        }

        private async Task<JObject> ProcessJObjectResponse(HttpResponseMessage response)
        {
            return JObject.Parse(await this.ProcessStringResponse(response));
        }

        private async Task<string> ProcessStringResponse(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.OK)
            {
                string result = await response.Content.ReadAsStringAsync();
                return result;
            }
            else
            {
                throw new RestServiceRequestException(response);
            }
        }

        private async Task<HttpClientWrapper> GetHttpClient()
        {
            if (this.connection != null)
            {
                return new HttpClientWrapper(await this.connection.GetOAuthTokenModel());
            }
            return new HttpClientWrapper();
        }
    }
}
