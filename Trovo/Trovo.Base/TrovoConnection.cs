using StreamingClient.Base.Model.OAuth;
using StreamingClient.Base.Util;
using StreamingClient.Base.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Trovo.Base.Services;

[assembly: InternalsVisibleTo("Trovo.Base.UnitTests")]
namespace Trovo.Base
{
    /// <summary>
    /// The OAuth authentication scopes.
    /// </summary>
    public enum OAuthClientScopeEnum
    {
        /// <summary>
        /// View your channel details. Including Stream Key. 
        /// </summary>
        channel_details_self,
        /// <summary>
        /// Update your channel settings.
        /// </summary>
        channel_update_self,
        /// <summary>
        /// Get your subscribers list
        /// </summary>
        channel_subscriptions,

        /// <summary>
        /// Connect to chat
        /// </summary>
        chat_connect,
        /// <summary>
        /// Send chat messages as connected user.
        /// </summary>
        chat_send_self,
        /// <summary>
        /// Send chat messages to the currently authenticated channel.
        /// </summary>
        send_to_my_channel,
        /// <summary>
        /// Perform chat commands and delete chat messages.
        /// </summary>
        manage_messages,

        /// <summary>
        /// View your email address and user profiles. 
        /// </summary>
        user_details_self,
    }

    /// <summary>
    /// https://trovo.live/policy/apis-developer-doc.html
    /// </summary>
    public class TrovoConnection
    {
        /// <summary>
        /// The default OAuth redirect URL used for authentication.
        /// </summary>
        public const string DEFAULT_OAUTH_LOCALHOST_URL = "http://localhost:8919/";

        /// <summary>
        /// The default request parameter for the access token from the OAuth service.
        /// </summary>
        public const string DEFAULT_ACCESS_TOKEN_URL_PARAMETER = "access_token";

        private OAuthTokenModel token;

        /// <summary>
        /// The Client ID associated with the connection.
        /// </summary>
        public string ClientID { get { return (this.token != null) ? this.token.clientID : null; } }

        /// <summary>
        /// Generates the OAuth authorization URL to use for authentication.
        /// </summary>
        /// <param name="clientID">The ID of the client application</param>
        /// <param name="scopes">The authorization scopes to request</param>
        /// <param name="redirectUri">The redirect URL for the client application</param>
        /// <param name="state">The state for authentication check</param>
        /// <param name="forceApprovalPrompt">Whether to force an approval from the user</param>
        /// <returns>The authorization URL</returns>
        public static async Task<string> GetAuthorizationCodeURLForOAuthBrowser(string clientID, IEnumerable<OAuthClientScopeEnum> scopes, string redirectUri, string state = "abc123", bool forceApprovalPrompt = false)
        {
            Validator.ValidateString(clientID, "clientID");
            Validator.ValidateList(scopes, "scopes");
            Validator.ValidateString(redirectUri, "redirectUri");
            Dictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "client_id", clientID },
                { "response_type", "token" },
                { "scope", TrovoConnection.ConvertClientScopesToString(scopes) },
                { "redirect_uri", redirectUri },
                { "state", state },
            };

            if (forceApprovalPrompt)
            {
                parameters.Add("force_verify", "force");
            }

            FormUrlEncodedContent content = new FormUrlEncodedContent(parameters.AsEnumerable());
            return OAuthService.OAuthBaseAddress + "?" + await content.ReadAsStringAsync();
        }

        /// <summary>
        /// Creates a TrovoConnection object from an OAuth authentication locally.
        /// </summary>
        /// <param name="clientID">The ID of the client application</param>
        /// <param name="scopes">The authorization scopes to request</param>
        /// <param name="state">The state for authentication check</param>
        /// <param name="forceApprovalPrompt">Whether to force an approval from the user</param>
        /// <param name="oauthListenerURL">The URL to listen for the OAuth successful authentication</param>
        /// <param name="successResponse">The response to send back upon successful authentication</param>
        /// <returns>The TrovoConnection object</returns>
        public static async Task<TrovoConnection> ConnectViaLocalhostOAuthBrowser(string clientID, IEnumerable<OAuthClientScopeEnum> scopes, string state = "abc123", bool forceApprovalPrompt = false, string oauthListenerURL = DEFAULT_OAUTH_LOCALHOST_URL, string successResponse = null)
        {
            Validator.ValidateString(clientID, "clientID");
            Validator.ValidateList(scopes, "scopes");

            LocalOAuthHttpListenerServer oauthServer = new LocalOAuthHttpListenerServer(DEFAULT_ACCESS_TOKEN_URL_PARAMETER, successResponse);
            oauthServer.Start(oauthListenerURL);

            string url = await TrovoConnection.GetAuthorizationCodeURLForOAuthBrowser(clientID, scopes, oauthListenerURL, state, forceApprovalPrompt);
            ProcessStartInfo startInfo = new ProcessStartInfo() { FileName = url, UseShellExecute = true };
            Process.Start(startInfo);

            string accessToken = await oauthServer.WaitForAuthorizationCode();
            oauthServer.Stop();

            if (accessToken != null)
            {
                TrovoConnection connection = new TrovoConnection(new OAuthTokenModel()
                {
                    clientID = clientID,
                    accessToken = accessToken,
                    AcquiredDateTime = DateTimeOffset.Now,
                    expiresIn = int.MaxValue,
                });

                OAuthTokenValidationModel validation = await connection.OAuth.ValidateToken(connection.token);
                connection.token.expiresTimeStamp = validation.expire_ts;

                return connection;
            }
            return null;
        }

        /// <summary>
        /// Creates a TrovoConnection object from an OAuth token.
        /// </summary>
        /// <param name="token">The OAuth token to use</param>
        /// <param name="refreshToken">Whether to refresh the token</param>
        /// <returns>The TrovoConnection object</returns>
        public static async Task<TrovoConnection> ConnectViaOAuthToken(OAuthTokenModel token, bool refreshToken = true)
        {
            Validator.ValidateVariable(token, "token");

            TrovoConnection connection = new TrovoConnection(token);
            if (refreshToken)
            {
                await connection.RefreshOAuthToken();
            }

            return connection;
        }

        internal static string ConvertClientScopesToString(IEnumerable<OAuthClientScopeEnum> scopes)
        {
            return string.Join("+", scopes);
        }

        /// <summary>
        /// The OAuth service.
        /// </summary>
        public OAuthService OAuth { get; private set; }

        /// <summary>
        /// The Channels service.
        /// </summary>
        public ChannelsService Channels { get; private set; }

        /// <summary>
        /// The Chat service.
        /// </summary>
        public ChatService Chat { get; private set; }

        /// <summary>
        /// The Users service.
        /// </summary>
        public UsersService Users { get; private set; }

        /// <summary>
        /// The Category service.
        /// </summary>
        public CategoryService Categories { get; set; }

        private TrovoConnection(OAuthTokenModel token)
        {
            Validator.ValidateVariable(token, "token");

            this.token = token;
            this.OAuth = new OAuthService(this);
            this.Channels = new ChannelsService(this);
            this.Chat = new ChatService(this);
            this.Users = new UsersService(this);
            this.Categories = new CategoryService(this);
        }

        /// <summary>
        /// Refreshs the current OAuth token.
        /// </summary>
        /// <returns>An awaitable Task</returns>
        public Task RefreshOAuthToken()
        {
            // TODO - Add OAuth token refreshing logic when API supports it
            //this.token = await this.OAuth.RefreshToken(this.token);
            return Task.FromResult(0);
        }

        /// <summary>
        /// Gets a copy of the current OAuth token.
        /// </summary>
        /// <returns>The OAuth token copy</returns>
        public OAuthTokenModel GetOAuthTokenCopy() { return JSONSerializerHelper.Clone<OAuthTokenModel>(this.token); }

        internal async Task<OAuthTokenModel> GetOAuthToken(bool autoRefreshToken = true)
        {
            if (autoRefreshToken && this.token.ExpirationDateTime < DateTimeOffset.Now)
            {
                await this.RefreshOAuthToken();
            }
            return this.token;
        }
    }
}
