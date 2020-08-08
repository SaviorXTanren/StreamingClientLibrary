using StreamingClient.Base.Model.OAuth;
using StreamingClient.Base.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Trovo.Base.Services
{
    public class OAuthTokenValidationModel
    {
        public string uid { get; set; }
        public string client_id { get; set; }
        public string nick_name { get; set; }
        public List<string> scopes { get; set; } = new List<string>();
        public long expire_ts { get; set; }

        public DateTimeOffset Expiration { get { return DateTimeOffset.FromUnixTimeSeconds(this.expire_ts); } }
    }

    public class OAuthService : TrovoServiceBase
    {
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
