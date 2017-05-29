using Mixer.Base.Services;
using Mixer.Base.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mixer.Base
{
    public class MixerClient
    {
        private AuthorizationToken token;

        public ChannelsService Channels { get; private set; }

        public static async Task<MixerClient> ConnectViaShortCode(string clientID, IEnumerable<ClientScopeEnum> scopes, Action<string> codeCallback)
        {
            Validator.ValidateString(clientID, "clientID");
            Validator.ValidateList(scopes, "scopes");
            Validator.ValidateVariable(codeCallback, "codeCallback");

            ShortCode shortCode = await AuthorizationToken.GenerateShortCode(clientID, scopes);

            codeCallback(shortCode.code);

            string authorizationCode = await AuthorizationToken.ValidateShortCode(shortCode);
            if (authorizationCode != null)
            {
                return await MixerClient.ConnectViaAuthorizationCode(clientID, authorizationCode);
            }
            return null;
        }

        public static async Task<MixerClient> ConnectViaAuthorizationCode(string clientID, string authorizationCode)
        {
            Validator.ValidateString(clientID, "clientID");
            Validator.ValidateList(authorizationCode, "authorizationCode");

            AuthorizationToken token = await AuthorizationToken.GetAuthorizationToken(clientID, authorizationCode);
            return new MixerClient(token);
        }

        private MixerClient(AuthorizationToken token)
        {
            Validator.ValidateVariable(token, "token");

            this.token = token;

            this.Channels = new ChannelsService(this);
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
