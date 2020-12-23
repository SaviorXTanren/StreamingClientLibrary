using StreamingClient.Base.Model.OAuth;
using StreamingClient.Base.Services;
using StreamingClient.Base.Util;
using System.Threading.Tasks;

namespace Glimesh.Base.Services
{
    /// <summary>
    /// Base class for all Glimesh services.
    /// </summary>
    public abstract class GlimeshServiceBase : OAuthRestServiceBase
    {
        private GlimeshConnection connection;
        private string baseAddress;

        /// <summary>
        /// Creates an instance of the GlimeshServiceBase.
        /// </summary>
        /// <param name="connection">The Glimesh connection to use</param>
        /// <param name="baseAddress">The base address to use</param>
        public GlimeshServiceBase(GlimeshConnection connection, string baseAddress)
        {
            Validator.ValidateVariable(connection, "connection");
            this.connection = connection;
            this.baseAddress = baseAddress;
        }

        internal GlimeshServiceBase(string baseAddress)
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
    }
}
