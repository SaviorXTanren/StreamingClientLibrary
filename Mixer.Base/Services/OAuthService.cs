using Mixer.Base.Model.OAuth;
using Mixer.Base.Util;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Mixer.Base.Services
{
    public class OAuthService : ServiceBase
    {
        public OAuthService(MixerConnection connection) : base(connection) { }

        internal OAuthService() : base() { }

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

        public async Task<OAuthTokenModel> GetOAuthTokenModel(string clientID, string authorizationCode, string redirectUrl = null)
        {
            Validator.ValidateString(clientID, "clientID");
            Validator.ValidateString(authorizationCode, "authorizationCode");

            JObject payload = new JObject();
            payload["grant_type"] = "authorization_code";
            payload["client_id"] = clientID;
            payload["code"] = authorizationCode;
            if (!string.IsNullOrEmpty(redirectUrl))
            {
                payload["redirect_uri"] = redirectUrl;
            }

            OAuthTokenModel token = await this.PostAsync<OAuthTokenModel>("oauth/token", this.CreateContentFromObject(payload));
            token.clientID = clientID;
            token.authorizationCode = authorizationCode;
            return token;
        }

        public async Task<OAuthTokenModel> RefreshToken(OAuthTokenModel token)
        {
            Validator.ValidateVariable(token, "token");

            JObject payload = new JObject();
            payload["grant_type"] = "refresh_token";
            payload["client_id"] = token.clientID;
            payload["refresh_token"] = token.refreshToken;

            OAuthTokenModel newToken = await this.PostAsync<OAuthTokenModel>("oauth/token", this.CreateContentFromObject(payload));
            newToken.clientID = token.clientID;
            newToken.authorizationCode = token.authorizationCode;
            return newToken;
        }
    }
}
