using Mixer.Base.Services;
using Mixer.Base.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mixer.Base
{
    public class MixerConnection
    {
        private AuthorizationToken token;

        public ChannelsService Channels { get; private set; }
        public ChatsService Chats { get; private set; }
        public InteractiveService Interactive { get; private set; }
        public UsersService Users { get; private set; }

        public static async Task<MixerConnection> ConnectViaShortCode(string clientID, IEnumerable<ClientScopeEnum> scopes, Action<ShortCode> codeCallback)
        {
            return await MixerConnection.ConnectViaShortCode(clientID, null, scopes, codeCallback);
        }

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

        public static async Task<string> GetAuthorizationCodeURLForOAuth(string clientID, IEnumerable<ClientScopeEnum> scopes, string redirectUri)
        {
            return await MixerConnection.GetAuthorizationCodeURLForOAuth(clientID, null, scopes, redirectUri);
        }

        public static async Task<string> GetAuthorizationCodeURLForOAuth(string clientID, string clientSecret, IEnumerable<ClientScopeEnum> scopes, string redirectUri)
        {
            Validator.ValidateString(clientID, "clientID");
            Validator.ValidateList(scopes, "scopes");
            Validator.ValidateString(redirectUri, "redirectUri");

            return await AuthorizationToken.GetAuthorizationCodeURLForOAuth(clientID, clientSecret, scopes, redirectUri);
        }

        public static async Task<MixerConnection> ConnectViaAuthorizationCode(string clientID, string authorizationCode)
        {
            Validator.ValidateString(clientID, "clientID");
            Validator.ValidateList(authorizationCode, "authorizationCode");

            AuthorizationToken token = await AuthorizationToken.GetAuthorizationToken(clientID, authorizationCode);
            return new MixerConnection(token);
        }

        private MixerConnection(AuthorizationToken token)
        {
            Validator.ValidateVariable(token, "token");

            this.token = token;

            this.Channels = new ChannelsService(this);
            this.Chats = new ChatsService(this);
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
