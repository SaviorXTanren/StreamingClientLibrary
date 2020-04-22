using StreamingClient.Base.Model.OAuth;
using StreamingClient.Base.Util;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace YouTube.Base.Services
{
    /// <summary>
    /// The APIs for OAuth-based services.
    /// </summary>
    public class OAuthService : YouTubeServiceBase
    {
        private const string OAuthBaseAddress = "https://www.googleapis.com/oauth2/v4/token";

        /// <summary>
        /// Creates an instance of the OAuthService.
        /// </summary>
        /// <param name="connection">The YouTube connection to use</param>
        public OAuthService(YouTubeConnection connection) : base(connection, OAuthBaseAddress) { }

        internal OAuthService() : base(OAuthBaseAddress) { }

        /// <summary>
        /// Creates an OAuth token for authenticating with the Mixer services.
        /// </summary>
        /// <param name="clientID">The id of the client application</param>
        /// <param name="authorizationCode">The authorization code</param>
        /// <param name="redirectUrl">The URL to redirect to after authorization is complete</param>
        /// <returns></returns>
        public async Task<OAuthTokenModel> GetOAuthTokenModel(string clientID, string authorizationCode, string redirectUrl = null)
        {
            return await this.GetOAuthTokenModel(clientID, null, authorizationCode, redirectUrl);
        }

        /// <summary>
        /// Creates an OAuth token for authenticating with the Mixer services.
        /// </summary>
        /// <param name="clientID">The id of the client application</param>
        /// <param name="clientSecret">The secret key of the client application</param>
        /// <param name="authorizationCode">The authorization code</param>
        /// <param name="redirectUrl">The URL to redirect to after authorization is complete</param>
        /// <returns></returns>
        public async Task<OAuthTokenModel> GetOAuthTokenModel(string clientID, string clientSecret, string authorizationCode, string redirectUrl = null)
        {
            Validator.ValidateString(clientID, "clientID");
            Validator.ValidateString(authorizationCode, "authorizationCode");

            Dictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "client_id", clientID },
                { "client_secret", clientSecret },
                { "code", authorizationCode },
                { "grant_type", "authorization_code" },
                { "redirect_uri", redirectUrl },
            };
            FormUrlEncodedContent content = new FormUrlEncodedContent(parameters.AsEnumerable());

            OAuthTokenModel token = await this.PostAsync<OAuthTokenModel>(string.Empty, new StringContent(await content.ReadAsStringAsync(), Encoding.UTF8, "application/x-www-form-urlencoded"), autoRefreshToken: false);
            token.clientID = clientID;
            token.clientSecret = clientSecret;
            token.authorizationCode = authorizationCode;
            return token;
        }

        /// <summary>
        /// Refreshes the specified OAuth token.
        /// </summary>
        /// <param name="token">The token to refresh</param>
        /// <returns>The refreshed token</returns>
        public async Task<OAuthTokenModel> RefreshToken(OAuthTokenModel token)
        {
            Validator.ValidateVariable(token, "token");

            Dictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "client_id", token.clientID },
                { "client_secret", token.clientSecret },
                { "refresh_token", token.refreshToken },
                { "grant_type", "refresh_token" },
            };
            FormUrlEncodedContent content = new FormUrlEncodedContent(parameters.AsEnumerable());

            OAuthTokenModel newToken = await this.PostAsync<OAuthTokenModel>(string.Empty, new StringContent(await content.ReadAsStringAsync(), Encoding.UTF8, "application/x-www-form-urlencoded"), autoRefreshToken: false);
            newToken.clientID = token.clientID;
            newToken.clientSecret = token.clientSecret;
            newToken.authorizationCode = token.authorizationCode;
            newToken.refreshToken = token.refreshToken;
            return newToken;
        }
    }
}
