using StreamingClient.Base.Model.OAuth;
using StreamingClient.Base.Services;
using StreamingClient.Base.Util;
using StreamingClient.Base.Web;
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
