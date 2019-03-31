using StreamingClient.Base.Model.OAuth;
using StreamingClient.Base.Services;
using StreamingClient.Base.Util;
using StreamingClient.Base.Web;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Mixer.Base.Services
{
    /// <summary>
    /// The abstract class in charge of handling RESTful requests against the Mixer APIs.
    /// </summary>
    public abstract class MixerServiceBase : OAuthRestServiceBase
    {
        private const string MixerRestAPIBaseAddressFormat = "https://mixer.com/api/v{0}/";

        private const string RequestLastPageRegexString = "page=[\\d]+>; rel=\"last\"";
        private const string RequestContinuationTokenPrefixString = "continuationToken=";

        private MixerConnection connection;
        private string baseAddress;

        /// <summary>
        /// Creates an instance of the MixerServiceBase.
        /// </summary>
        /// <param name="connection">The Mixer connection to use</param>
        public MixerServiceBase(MixerConnection connection) : this(connection, 1) { }

        /// <summary>
        /// Creates an instance of the MixerServiceBase.
        /// </summary>
        /// <param name="connection">The Mixer connection to use</param>
        /// <param name="version">The version number of the Mixer API endpoint</param>
        public MixerServiceBase(MixerConnection connection, int version)
        {
            Validator.ValidateVariable(connection, "connection");
            this.connection = connection;
            this.baseAddress = string.Format(MixerRestAPIBaseAddressFormat, version);
        }

        internal MixerServiceBase()
        {
            this.baseAddress = string.Format(MixerRestAPIBaseAddressFormat, 1);
        }

        /// <summary>
        /// Performs a GET REST request using the provided request URI for paged results.
        /// </summary>
        /// <param name="requestUri">The request URI to use</param>
        /// <param name="maxResults">The maximum results to return. The total results returned can exceed this value if more results are found within the pages acquired.</param>
        /// <param name="linkPagesAvailable">Whether the link pages header property exists</param>
        /// <returns>A type-casted list of objects of the contents of the response</returns>
        public async Task<IEnumerable<T>> GetPagedNumberAsync<T>(string requestUri, uint maxResults = 1, bool linkPagesAvailable = true)
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

                T[] pagedResults = await response.ProcessResponse<T[]>();
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
        /// Performs a GET REST request using the provided request URI for paged results.
        /// </summary>
        /// <param name="requestUri">The request URI to use</param>
        /// <param name="maxResults">The maximum results to return. The total results returned can exceed this value if more results are found within the pages acquired.</param>
        /// <returns>A type-casted list of objects of the contents of the response</returns>
        public async Task<IEnumerable<T>> GetPagedContinuationAsync<T>(string requestUri, uint maxResults = 1)
        {
            List<T> results = new List<T>();
            string continuationToken = null;

            do
            {
                string currentRequestUri = requestUri;
                if (!string.IsNullOrEmpty(continuationToken))
                {
                    if (currentRequestUri.Contains("?"))
                    {
                        currentRequestUri += "&";
                    }
                    else
                    {
                        currentRequestUri += "?";
                    }
                    currentRequestUri += "continuationToken=" + continuationToken;
                }
                HttpResponseMessage response = await this.GetAsync(currentRequestUri);

                T[] pagedResults = await response.ProcessResponse<T[]>();
                results.AddRange(pagedResults);

                continuationToken = null;

                IEnumerable<string> linkValues;
                if (pagedResults.Length > 0 && response.Headers.TryGetValues("link", out linkValues))
                {
                    if (linkValues.Count() > 0)
                    {
                        string token = linkValues.First();
                        int tokenStart = token.IndexOf(RequestContinuationTokenPrefixString);
                        if (tokenStart >= 0)
                        {
                            token = token.Substring(tokenStart + RequestContinuationTokenPrefixString.Length);
                            int tokenEnd = token.IndexOf(">");
                            if (tokenEnd >= 0)
                            {
                                token = token.Substring(0, tokenEnd);
                            }
                        }

                        continuationToken = token;
                    }
                }
            } while (results.Count < maxResults && !string.IsNullOrEmpty(continuationToken));

            return results;
        }

        /// <summary>
        /// Creates an HttpContent object from the specified object.
        /// </summary>
        /// <param name="obj">The object to serialize</param>
        /// <returns>The HttpContent containing the serialized object</returns>
        protected HttpContent CreateContentFromObject(object obj) { return AdvancedHttpClient.CreateContentFromObject(obj); }

        /// <summary>
        /// Creates an HttpContent object from the specified string.
        /// </summary>
        /// <param name="str">The string to serialize</param>
        /// <returns>The HttpContent containing the serialized string</returns>
        protected HttpContent CreateContentFromString(string str) { return AdvancedHttpClient.CreateContentFromString(str); }

        /// <summary>
        /// Gets the OAuth token for the connection of this service.
        /// </summary>
        /// <param name="autoRefreshToken">Whether to automatically refresh the OAuth token or not if it has to be</param>
        /// <returns>The OAuth token for the connection</returns>
        protected override async Task<OAuthTokenModel> GetOAuthToken(bool autoRefreshToken = true)
        {
            if (this.connection != null)
            {
                return await this.connection.GetOAuthToken(autoRefreshToken);
            }
            return null;
        }

        /// <summary>
        /// Gets the base address for all RESTful calls for this service.
        /// </summary>
        /// <returns>The base address for all RESTful calls</returns>
        protected override string GetBaseAddress() { return this.baseAddress; }
    }
}
