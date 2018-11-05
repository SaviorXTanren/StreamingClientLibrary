using Mixer.Base.Model.OAuth;
using Mixer.Base.Util;
using System.Threading.Tasks;

namespace Mixer.Base.Services
{
    /// <summary>
    /// The abstract class in charge of handling RESTful requests against the Mixer APIs.
    /// </summary>
    public abstract class MixerServiceBase : RestServiceBase
    {
        private const string MixerRestAPIBaseAddressFormat = "https://mixer.com/api/v{0}/";

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
