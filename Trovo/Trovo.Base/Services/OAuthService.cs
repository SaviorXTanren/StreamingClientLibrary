using StreamingClient.Base.Model.OAuth;
using StreamingClient.Base.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Trovo.Base.Services
{
    /// <summary>
    /// The validation object for an OAuth token.
    /// </summary>
    public class OAuthTokenValidationModel
    {
        /// <summary>
        /// The ID of the user.
        /// </summary>
        public string uid { get; set; }
        /// <summary>
        /// The ID of the client application.
        /// </summary>
        public string client_id { get; set; }
        /// <summary>
        /// The display name of the user.
        /// </summary>
        public string nick_name { get; set; }
        /// <summary>
        /// The scopes requested.
        /// </summary>
        public List<string> scopes { get; set; } = new List<string>();
        /// <summary>
        /// The expiration time in Unix seconds.
        /// </summary>
        public long expire_ts { get; set; }

        /// <summary>
        /// The expiration date time.
        /// </summary>
        public DateTimeOffset Expiration { get { return DateTimeOffset.FromUnixTimeSeconds(this.expire_ts); } }
    }

    /// <summary>
    /// The OAuth service.
    /// </summary>
    public class OAuthService : TrovoServiceBase
    {
        /// <summary>
        /// The base OAuth address.
        /// </summary>
        public const string OAuthBaseAddress = "http://cdn.trovo.live/page/login.html";

        /// <summary>
        /// Creates an instance of the OAuthService.
        /// </summary>
        /// <param name="connection">The YouTube connection to use</param>
        public OAuthService(TrovoConnection connection) : base(connection) { }

        internal OAuthService() : base() { }

        /// <summary>
        /// Refreshes the specified OAuth token.
        /// </summary>
        /// <param name="token">The token to refresh</param>
        /// <returns>The refreshed token</returns>
        public async Task<OAuthTokenValidationModel> ValidateToken(OAuthTokenModel token)
        {
            Validator.ValidateVariable(token, "token");
            return await this.GetAsync<OAuthTokenValidationModel>("validate");
        }
    }
}
