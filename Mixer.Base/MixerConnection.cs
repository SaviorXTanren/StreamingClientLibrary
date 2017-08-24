using Mixer.Base.Services;
using Mixer.Base.Util;
using Mixer.Base.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Mixer.Base
{
    public class MixerConnection
    {
        public const string OAUTH_LOCALHOST_URL = "http://localhost:8080/";

        private AuthorizationToken token;

        public ChannelsService Channels { get; private set; }
        public ChatsService Chats { get; private set; }
        public CostreamService Costream { get; private set; }
        public InteractiveService Interactive { get; private set; }
        public UsersService Users { get; private set; }

        /// <summary>
        /// NOTE: There is a known issue with the Mixer APIs where authenticating with a short code as opposed to the regular OAuth process, where certain
        /// Chat Client commands will not work (EX: Timeout, Clear Messages, Delete Message, etc). The current work around to this is to use the traditional
        /// OAuth authentication methods.
        /// </summary>
        /// <param name="clientID"></param>
        /// <param name="scopes"></param>
        /// <param name="codeCallback"></param>
        /// <returns></returns>
        public static async Task<MixerConnection> ConnectViaShortCode(string clientID, IEnumerable<ClientScopeEnum> scopes, Action<ShortCode> codeCallback)
        {
            return await MixerConnection.ConnectViaShortCode(clientID, null, scopes, codeCallback);
        }

        /// <summary>
        /// NOTE: There is a known issue with the Mixer APIs where authenticating with a short code as opposed to the regular OAuth process, where certain
        /// Chat Client commands will not work (EX: Timeout, Clear Messages, Delete Message, etc). The current work around to this is to use the traditional
        /// OAuth authentication methods.
        /// </summary>
        /// <param name="clientID"></param>
        /// <param name="scopes"></param>
        /// <param name="codeCallback"></param>
        /// <returns></returns>
        public static async Task<MixerConnection> ConnectViaShortCode(string clientID, string clientSecret, IEnumerable<ClientScopeEnum> scopes, Action<ShortCode> codeCallback)
        {
            Validator.ValidateString(clientID, "clientID");
            Validator.ValidateList(scopes, "scopes");
            Validator.ValidateVariable(codeCallback, "codeCallback");

            string authorizationCode = await AuthorizationToken.GetAuthorizationCodeViaShortCode(clientID, clientSecret, scopes, codeCallback);
            if (authorizationCode != null)
            {
                return await MixerConnection.ConnectViaAuthorizationCode(clientID, authorizationCode);
            }
            return null;
        }

        public static async Task<string> GetAuthorizationCodeURLForOAuthBrowser(string clientID, IEnumerable<ClientScopeEnum> scopes, string redirectUri)
        {
            return await MixerConnection.GetAuthorizationCodeURLForOAuthBrowser(clientID, null, scopes, redirectUri);
        }

        public static async Task<string> GetAuthorizationCodeURLForOAuthBrowser(string clientID, string clientSecret, IEnumerable<ClientScopeEnum> scopes, string redirectUri)
        {
            Validator.ValidateString(clientID, "clientID");
            Validator.ValidateList(scopes, "scopes");
            Validator.ValidateString(redirectUri, "redirectUri");

            return await AuthorizationToken.GetAuthorizationCodeURLForOAuthBrowser(clientID, clientSecret, scopes, redirectUri);
        }

        public static async Task<MixerConnection> ConnectViaLocalhostOAuthBrowser(string clientID, IEnumerable<ClientScopeEnum> scopes)
        {
            return await ConnectViaLocalhostOAuthBrowser(clientID, null, scopes);
        }

        public static async Task<MixerConnection> ConnectViaLocalhostOAuthBrowser(string clientID, string clientSecret, IEnumerable<ClientScopeEnum> scopes)
        {
            Validator.ValidateString(clientID, "clientID");
            Validator.ValidateList(scopes, "scopes");

            OAuthHttpListenerServer oauthServer = new OAuthHttpListenerServer(OAUTH_LOCALHOST_URL);
            oauthServer.Start();

            string url = await MixerConnection.GetAuthorizationCodeURLForOAuthBrowser(clientID, scopes, OAUTH_LOCALHOST_URL);

            Process.Start(url);

            string authorizationCode = await oauthServer.WaitForAuthorizationCode();
            oauthServer.End();

            if (authorizationCode != null)
            {
                return await MixerConnection.ConnectViaAuthorizationCode(clientID, authorizationCode, redirectUrl: OAUTH_LOCALHOST_URL);
            }
            return null;
        }

        public static async Task<MixerConnection> ConnectViaAuthorizationCode(string clientID, string authorizationCode, string redirectUrl = null)
        {
            Validator.ValidateString(clientID, "clientID");
            Validator.ValidateString(authorizationCode, "authorizationCode");

            AuthorizationToken token = await AuthorizationToken.GetAuthorizationToken(clientID, authorizationCode, redirectUrl);
            return new MixerConnection(token);
        }

        private MixerConnection(AuthorizationToken token)
        {
            Validator.ValidateVariable(token, "token");

            this.token = token;

            this.Channels = new ChannelsService(this);
            this.Chats = new ChatsService(this);
            this.Costream = new CostreamService(this);
            this.Interactive = new InteractiveService(this);
            this.Users = new UsersService(this);
        }

        internal async Task<AuthorizationToken> GetAuthorizationToken()
        {
            if (this.token.Expiration < DateTimeOffset.Now)
            {
                await this.token.RefreshToken();
            }
            return this.token;
        }
    }
}
