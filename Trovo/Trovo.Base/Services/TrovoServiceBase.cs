using StreamingClient.Base.Model.OAuth;
using StreamingClient.Base.Services;
using StreamingClient.Base.Util;
using StreamingClient.Base.Web;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Trovo.Base.Services
{
    /// <summary>
    /// Base class for all Trovo services.
    /// </summary>
    public abstract class TrovoServiceBase : OAuthRestServiceBase
    {
        private const string TrovoRestAPIBaseAddressFormat = "https://open-api.trovo.live/openplatform/";

        /// <summary>
        /// The Trovo connection.
        /// </summary>
        protected TrovoConnection connection;

        private string baseAddress;
        private string clientID;

        /// <summary>
        /// Creates an instance of the TrovoServiceBase.
        /// </summary>
        /// <param name="connection">The YouTube connection to use</param>
        public TrovoServiceBase(TrovoConnection connection) : this(connection, TrovoRestAPIBaseAddressFormat) { }

        /// <summary>
        /// Creates an instance of the TrovoServiceBase.
        /// </summary>
        /// <param name="connection">The Trovo connection to use</param>
        /// <param name="baseAddress">The base address to use</param>
        public TrovoServiceBase(TrovoConnection connection, string baseAddress)
        {
            Validator.ValidateVariable(connection, "connection");
            this.connection = connection;
            this.baseAddress = baseAddress;
            this.clientID = connection.ClientID;
        }

        internal TrovoServiceBase() : this(TrovoRestAPIBaseAddressFormat) { }

        internal TrovoServiceBase(string baseAddress)
        {
            this.baseAddress = baseAddress;
        }

        /// <summary>
        /// Performs a GET REST request using the provided request URI for paged cursor data.
        /// </summary>
        /// <param name="requestUri">The request URI to use</param>
        /// <param name="maxResults">The maximum number of results. Will be either that amount or slightly more</param>
        /// <param name="maxLimit">The maximum limit of results that can be returned in a single request</param>
        /// <returns>A type-casted object of the contents of the response</returns>
        public async Task<IEnumerable<T>> PostPagedCursorAsync<T>(string requestUri, int maxResults = 1, int maxLimit = 100)
        {
            Dictionary<string, string> queryParameters = new Dictionary<string, string>();
            queryParameters.Add("limit", ((maxResults > maxLimit) ? maxLimit : maxResults).ToString());

            List<T> results = new List<T>();
            string cursor = null;
            do
            {
                if (!string.IsNullOrEmpty(cursor))
                {
                    queryParameters["cursor"] = cursor;
                }

                T data = await this.PostAsync<T>(requestUri, AdvancedHttpClient.CreateContentFromObject(queryParameters));

                cursor = null;
                if (data != null)
                {
                    results.Add(data);
                    //cursor = data.Cursor;
                }
            }
            while (results.Count < maxResults && !string.IsNullOrEmpty(cursor));

            return results;
        }

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

        /// <summary>
        /// Gets the HttpClient using the OAuth for the connection of this service.
        /// </summary>
        /// <param name="autoRefreshToken">Whether to automatically refresh the OAuth token or not if it has to be</param>
        /// <returns>The HttpClient for the connection</returns>
        protected override async Task<AdvancedHttpClient> GetHttpClient(bool autoRefreshToken = true)
        {
            AdvancedHttpClient client = new AdvancedHttpClient(this.GetBaseAddress());
            OAuthTokenModel token = await this.GetOAuthToken(autoRefreshToken);
            if (token != null)
            {
                client = new AdvancedHttpClient(this.GetBaseAddress(), "OAuth", token.accessToken);
            }
            client.DefaultRequestHeaders.Add("Client-ID", this.clientID);
            return client;
        }
    }
}
