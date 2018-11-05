using Mixer.Base.Model.OAuth;
using Mixer.Base.Util;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Mixer.Base.Services
{
    /// <summary>
    /// The APIs for OAuth-based services.
    /// </summary>
    public class OAuthService : MixerServiceBase
    {
        /// <summary>
        /// Creates an instance of the OAuthService.
        /// </summary>
        /// <param name="connection">The Mixer connection to use</param>
        public OAuthService(MixerConnection connection) : base(connection) { }

        internal OAuthService() : base() { }

        /// <summary>
        /// Gets the short code authorization for the specified client and scopes.
        /// </summary>
        /// <param name="clientID">The id of the client application</param>
        /// <param name="clientSecret">The secret key of the client application</param>
        /// <param name="scopes">The scopes to authorize</param>
        /// <returns>The short code authorization</returns>
        public async Task<OAuthShortCodeModel> GetShortCode(string clientID, string clientSecret, IEnumerable<OAuthClientScopeEnum> scopes)
        {
            Validator.ValidateString(clientID, "clientID");
            Validator.ValidateList(scopes, "scopes");

            JObject payload = new JObject();
            payload["client_id"] = clientID;
            payload["client_secret"] = clientSecret;
            payload["scope"] = MixerConnection.ConvertClientScopesToString(scopes);

            return await this.PostAsync<OAuthShortCodeModel>("oauth/shortcode", this.CreateContentFromObject(payload));
        }

        /// <summary>
        /// Validates the specified short code authorization and returns the authorization code.
        /// </summary>
        /// <param name="shortCode">The short code authorization to validate</param>
        /// <returns>The authorization code</returns>
        public async Task<string> ValidateShortCode(OAuthShortCodeModel shortCode)
        {
            Validator.ValidateVariable(shortCode, "shortCode");

            HttpResponseMessage response = await this.GetAsync("oauth/shortcode/check/" + shortCode.handle);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                JObject jobject = JObject.Parse(await response.Content.ReadAsStringAsync());
                return (string)jobject["code"];
            }
            return null;
        }

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

            JObject payload = new JObject();
            payload["grant_type"] = "authorization_code";
            payload["client_id"] = clientID;
            payload["code"] = authorizationCode;
            if (!string.IsNullOrEmpty(clientSecret))
            {
                payload["client_secret"] = clientSecret;
            }
            if (!string.IsNullOrEmpty(redirectUrl))
            {
                payload["redirect_uri"] = redirectUrl;
            }

            OAuthTokenModel token = await this.PostAsync<OAuthTokenModel>("oauth/token", this.CreateContentFromObject(payload), autoRefreshToken: false);
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

            JObject payload = new JObject();
            payload["grant_type"] = "refresh_token";
            payload["client_id"] = token.clientID;
            if (!string.IsNullOrEmpty(token.clientSecret))
            {
                payload["client_secret"] = token.clientSecret;
            }
            payload["refresh_token"] = token.refreshToken;

            OAuthTokenModel newToken = await this.PostAsync<OAuthTokenModel>("oauth/token", this.CreateContentFromObject(payload), autoRefreshToken: false);
            newToken.clientID = token.clientID;
            newToken.authorizationCode = token.authorizationCode;
            return newToken;
        }
    }
}
