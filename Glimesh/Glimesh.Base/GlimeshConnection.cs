using Glimesh.Base.Services;
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

[assembly: InternalsVisibleTo("Glimesh.Base.UnitTests")]

namespace Glimesh.Base
{
    /// <summary>
    /// https://glimesh.github.io/api-docs/docs/reference/scopes/
    /// </summary>
    public enum OAuthClientScopeEnum
    {
        /// <summary>
        /// Get public information about the user and other users on Glimesh.
        /// </summary>
        [Name("public")]
        publicinfo,
        /// <summary>
        /// View the email of the user.
        /// </summary>
        email,
        /// <summary>
        /// View and speak in a chatroom.
        /// </summary>
        chat,
        /// <summary>
        /// View the streamkey of the user.
        /// </summary>
        streamkey,
    }

    /// <summary>
    /// The main connection handler for Glimesh.
    /// </summary>
    public class GlimeshConnection
    {
        /// <summary>
        /// The default OAuth redirect URL used for authentication.
        /// </summary>
        public const string DEFAULT_OAUTH_LOCALHOST_URL = "http://localhost:8919/";

        /// <summary>
        /// The default request parameter for the authorization code from the OAuth service.
        /// </summary>
        public const string DEFAULT_AUTHORIZATION_CODE_URL_PARAMETER = "code";

        private OAuthTokenModel token;

        /// <summary>
        /// APIs for OAuth interaction.
        /// </summary>
        public OAuthService OAuth { get; private set; }

        /// <summary>
        /// APIs for Category interaction.
        /// </summary>
        public CategoryService Category { get; private set; }

        /// <summary>
        /// APIs for Channel interaction.
        /// </summary>
        public ChannelService Channel { get; private set; }

        /// <summary>
        /// APIs for User interaction.
        /// </summary>
        public UsersService Users { get; private set; }

        /// <summary>
        /// Generates the OAuth authorization URL to use for authentication.
        /// </summary>
        /// <param name="clientID">The ID of the client application</param>
        /// <param name="scopes">The authorization scopes to request</param>
        /// <param name="redirectUri">The redirect URL for the client application</param>
        /// <param name="forceApprovalPrompt">Whether to force an approval from the user</param>
        /// <returns>The authorization URL</returns>
        public static async Task<string> GetAuthorizationCodeURLForOAuthBrowser(string clientID, IEnumerable<OAuthClientScopeEnum> scopes, string redirectUri, bool forceApprovalPrompt = false)
        {
            Validator.ValidateString(clientID, "clientID");
            Validator.ValidateList(scopes, "scopes");
            Validator.ValidateString(redirectUri, "redirectUri");
            Dictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "client_id", clientID },
                { "scope", GlimeshConnection.ConvertClientScopesToString(scopes) },
                { "response_type", "code" },
                { "redirect_uri", redirectUri },
            };

            if (forceApprovalPrompt)
            {
                parameters.Add("force_verify", "force");
            }

            FormUrlEncodedContent content = new FormUrlEncodedContent(parameters.AsEnumerable());

            return "https://glimesh.tv/oauth/authorize?" + await content.ReadAsStringAsync();
        }

        /// <summary>
        /// Creates a GlimeshConnection object from an OAuth authentication locally.
        /// </summary>
        /// <param name="clientID">The ID of the client application</param>
        /// <param name="clientSecret">The secret of the client application</param>
        /// <param name="scopes">The authorization scopes to request</param>
        /// <param name="forceApprovalPrompt">Whether to force an approval from the user</param>
        /// <param name="oauthListenerURL">The URL to listen for the OAuth successful authentication</param>
        /// <param name="successResponse">The response to send back upon successful authentication</param>
        /// <returns>The GlimeshConnection object</returns>
        public static async Task<GlimeshConnection> ConnectViaLocalhostOAuthBrowser(string clientID, string clientSecret, IEnumerable<OAuthClientScopeEnum> scopes, bool forceApprovalPrompt = false, string oauthListenerURL = DEFAULT_OAUTH_LOCALHOST_URL, string successResponse = null)
        {
            Validator.ValidateString(clientID, "clientID");
            Validator.ValidateList(scopes, "scopes");

            LocalOAuthHttpListenerServer oauthServer = new LocalOAuthHttpListenerServer(DEFAULT_AUTHORIZATION_CODE_URL_PARAMETER, successResponse);
            oauthServer.Start(oauthListenerURL);

            string url = await GlimeshConnection.GetAuthorizationCodeURLForOAuthBrowser(clientID, scopes, oauthListenerURL, forceApprovalPrompt);
            ProcessStartInfo startInfo = new ProcessStartInfo() { FileName = url, UseShellExecute = true };
            Process.Start(startInfo);

            string authorizationCode = await oauthServer.WaitForAuthorizationCode();
            oauthServer.Stop();

            if (authorizationCode != null)
            {
                return await GlimeshConnection.ConnectViaAuthorizationCode(clientID, clientSecret, authorizationCode, redirectUrl: oauthListenerURL);
            }
            return null;
        }

        /// <summary>
        /// Creates a GlimeshConnection object from an authorization code.
        /// </summary>
        /// <param name="clientID">The ID of the client application</param>
        /// <param name="clientSecret">The secret of the client application</param>
        /// <param name="authorizationCode">The authorization code for the authenticated user</param>
        /// <param name="redirectUrl">The redirect URL of the client application</param>
        /// <returns>The GlimeshConnection object</returns>
        public static async Task<GlimeshConnection> ConnectViaAuthorizationCode(string clientID, string clientSecret, string authorizationCode, string redirectUrl = null)
        {
            Validator.ValidateString(clientID, "clientID");
            Validator.ValidateString(authorizationCode, "authorizationCode");

            OAuthService oauthService = new OAuthService();
            OAuthTokenModel token = await oauthService.GetOAuthTokenModel(clientID, clientSecret, authorizationCode, redirectUrl);
            if (token == null)
            {
                throw new InvalidOperationException("OAuth token was not acquired");
            }
            return new GlimeshConnection(token);
        }

        /// <summary>
        /// Creates a GlimeshConnection object from an OAuth token.
        /// </summary>
        /// <param name="token">The OAuth token to use</param>
        /// <param name="refreshToken">Whether to refresh the token</param>
        /// <returns>The GlimeshConnection object</returns>
        public static async Task<GlimeshConnection> ConnectViaOAuthToken(OAuthTokenModel token, bool refreshToken = true)
        {
            Validator.ValidateVariable(token, "token");

            GlimeshConnection connection = new GlimeshConnection(token);
            if (refreshToken)
            {
                await connection.RefreshOAuthToken();
            }

            return connection;
        }

        internal static string ConvertClientScopesToString(IEnumerable<OAuthClientScopeEnum> scopes)
        {
            string result = "";

            foreach (string scopeName in EnumHelper.GetEnumNames(scopes))
            {
                result += scopeName.Replace("__", ":") + " ";
            }

            if (result.Length > 0)
            {
                result = result.Substring(0, result.Length - 1);
            }

            return result;
        }

        private GlimeshConnection(OAuthTokenModel token)
        {
            Validator.ValidateVariable(token, "token");

            this.token = token;

            this.OAuth = new OAuthService(this);
            this.Category = new CategoryService(this);
            this.Channel = new ChannelService(this);
            this.Users = new UsersService(this);
        }

        /// <summary>
        /// Refreshs the current OAuth token.
        /// </summary>
        /// <returns>An awaitable Task</returns>
        public async Task RefreshOAuthToken() { this.token = await this.OAuth.RefreshToken(this.token); }

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
