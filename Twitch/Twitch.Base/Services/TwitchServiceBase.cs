using StreamingClient.Base.Model.OAuth;
using StreamingClient.Base.Services;
using StreamingClient.Base.Util;
using StreamingClient.Base.Web;
using System.Threading.Tasks;

namespace Twitch.Base.Services
{
    /// <summary>
    /// The abstract class in charge of handling RESTful requests against the Twitch APIs.
    /// </summary>
    public class TwitchServiceBase : OAuthRestServiceBase
    {
        private TwitchConnection connection;
        private string baseAddress;

        private string clientID;

        /// <summary>
        /// Creates an instance of the TwitchServiceBase.
        /// </summary>
        /// <param name="connection">The Twitch connection to use</param>
        /// <param name="baseAddress">The base address to use</param>
        public TwitchServiceBase(TwitchConnection connection, string baseAddress)
        {
            Validator.ValidateVariable(connection, "connection");
            this.connection = connection;
            this.baseAddress = baseAddress;
            this.clientID = connection.ClientID;
        }

        internal TwitchServiceBase(string baseAddress)
        {
            this.baseAddress = baseAddress;
        }

        /// <summary>
        /// Gets the HttpClient using the OAuth for the connection of this service.
        /// </summary>
        /// <param name="autoRefreshToken">Whether to automatically refresh the OAuth token or not if it has to be</param>
        /// <returns>The HttpClient for the connection</returns>
        protected override async Task<AdvancedHttpClient> GetHttpClient(bool autoRefreshToken = true)
        {
            AdvancedHttpClient client = await base.GetHttpClient(autoRefreshToken);
            if (!string.IsNullOrEmpty(this.clientID))
            {
                client.DefaultRequestHeaders.Add("Client-ID", this.clientID);
            }
            return client;
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
    }
}
