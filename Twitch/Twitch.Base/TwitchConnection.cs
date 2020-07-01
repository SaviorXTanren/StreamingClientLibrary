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
using Twitch.Base.Services;
using Twitch.Base.Services.NewAPI;
using Twitch.Base.Services.V5API;

[assembly: InternalsVisibleTo("Twitch.Base.UnitTests")]

namespace Twitch.Base
{
    /// <summary>
    /// Authentication scopes for a user: https://dev.twitch.tv/docs/authentication/#scopes
    /// </summary>
    public enum OAuthClientScopeEnum
    {
        // v5 API

        /// <summary>
        /// Read whether a user is subscribed to your channel.
        /// </summary>
        channel_check_subscription = 0,
        /// <summary>
        /// Trigger commercials on channel.
        /// </summary>
        channel_commercial,
        /// <summary>
        /// Write channel metadata (game, status, etc).
        /// </summary>
        channel_editor,
        /// <summary>
        /// Add posts and reactions to a channel feed.
        /// </summary>
        channel_feed_edit,
        /// <summary>
        /// View a channel feed.
        /// </summary>
        channel_feed_read,
        /// <summary>
        /// Read nonpublic channel information, including email address and stream key.
        /// </summary>
        channel_read,
        /// <summary>
        /// Reset a channel’s stream key.
        /// </summary>
        channel_stream,
        /// <summary>
        /// Read all subscribers to your channel.
        /// </summary>
        channel_subscriptions,
        /// <summary>
        /// (Deprecated — cannot be requested by new clients.)
        /// Log into chat and send messages.
        /// </summary>
        chat_login,
        /// <summary>
        /// Manage a user’s collections (of videos).
        /// </summary>
        collections_edit,
        /// <summary>
        /// Manage a user’s communities.
        /// </summary>
        communities_edit,
        /// <summary>
        /// Manage community moderators.
        /// </summary>
        communities_moderate,
        /// <summary>
        /// Use OpenID Connect authentication.
        /// </summary>
        openid,
        /// <summary>
        /// Turn on/off ignoring a user. Ignoring users means you cannot see them type, receive messages from them, etc.
        /// </summary>
        user_blocks_edit,
        /// <summary>
        /// Read a user’s list of ignored users.
        /// </summary>
        user_blocks_read,
        /// <summary>
        /// Manage a user’s followed channels.
        /// </summary>
        user_follows_edit,
        /// <summary>
        /// Read nonpublic user information, like email address.
        /// </summary>
        user_read,
        /// <summary>
        /// Read a user’s subscriptions.
        /// </summary>
        user_subscriptions,
        /// <summary>
        /// Turn on Viewer Heartbeat Service ability to record user data.
        /// </summary>
        viewing_activity_read,

        // Chat / PubSub

        /// <summary>
        /// Perform moderation actions in a channel. The user requesting the scope must be a moderator in the channel.
        /// </summary>
        channel__moderate = 100,
        /// <summary>
        /// Send live stream chat and rooms messages.
        /// </summary>
        chat__edit,
        /// <summary>
        /// View live stream chat and rooms messages.
        /// </summary>
        chat__read,
        /// <summary>
        /// View your whisper messages.
        /// </summary>
        whispers__read,
        /// <summary>
        /// Send whisper messages.
        /// </summary>
        whispers__edit,

        // New API

        /// <summary>
        /// View analytics data for your extensions.
        /// </summary>
        analytics__read__extensions = 200,
        /// <summary>
        /// View analytics data for your games.
        /// </summary>
        analytics__read__games,
        /// <summary>
        /// View Bits information for your channel.
        /// </summary>
        bits__read,
        /// <summary>
        /// Run commercials on a channel.
        /// </summary>
        channel__edit__commercial,
        /// <summary>
        /// Get hype train information for your channel.
        /// </summary>
        channel__read__hype_train,
        /// <summary>
        /// Get channel point redemption events for your channel.
        /// </summary>
        channel__read__redemptions,
        /// <summary>
        /// Get a list of all subscribers to your channel and check if a user is subscribed to your channel
        /// </summary>
        channel__read__subscriptions,
        /// <summary>
        /// Manage a clip object.
        /// </summary>
        clips__edit,
        /// <summary>
        /// Read moderation events.
        /// </summary>
        moderation__read,
        /// <summary>
        /// Manage a user object.
        /// </summary>
        user__edit,
        /// <summary>
        /// Edit your channel’s broadcast configuration, including extension configuration. (This scope implies user:read:broadcast capability.)
        /// </summary>
        user__edit__broadcast,
        /// <summary>
        /// Edit your follows.
        /// </summary>
        user__edit__follows,
        /// <summary>
        /// View your broadcasting configuration, including extension configurations.
        /// </summary>
        user__read__broadcast,
        /// <summary>
        /// Read authorized user’s email address.
        /// </summary>
        user__read__email,
        /// <summary>
        /// Read authorized user’s stream key.
        /// </summary>
        user__read__stream_key
    }

    /// <summary>
    /// The main connection handler for Twitch.
    /// </summary>
    public class TwitchConnection
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
        /// APIs for the New Twitch API.
        /// </summary>
        public NewTwitchAPIServices NewAPI { get; private set; }

        /// <summary>
        /// APIs for the Twitch API V5.
        /// </summary>
        public V5APIServices V5API { get; private set; }

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
        /// <param name="forceApprovalPrompt">Whether to force an approval from the user</param>
        /// <returns>The authorization URL</returns>
        public static async Task<string> GetAuthorizationCodeURLForOAuthBrowser(string clientID, IEnumerable<OAuthClientScopeEnum> scopes, string redirectUri, bool forceApprovalPrompt = false)
        {
            Validator.ValidateString(clientID, "clientID");
            Validator.ValidateList(scopes, "scopes");
            Validator.ValidateString(redirectUri, "redirectUri");

            string url = "https://id.twitch.tv/oauth2/authorize";

            Dictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "client_id", clientID },
                { "scope", TwitchConnection.ConvertClientScopesToString(scopes) },
                { "response_type", "code" },
                { "redirect_uri", redirectUri },
            };

            if (forceApprovalPrompt)
            {
                parameters.Add("force_verify", "force");
            }

            FormUrlEncodedContent content = new FormUrlEncodedContent(parameters.AsEnumerable());

            return url + "?" + await content.ReadAsStringAsync();
        }

        /// <summary>
        /// Creates a TwitchConnection object from an OAuth authentication locally.
        /// </summary>
        /// <param name="clientID">The ID of the client application</param>
        /// <param name="clientSecret">The secret of the client application</param>
        /// <param name="scopes">The authorization scopes to request</param>
        /// <param name="forceApprovalPrompt">Whether to force an approval from the user</param>
        /// <param name="oauthListenerURL">The URL to listen for the OAuth successful authentication</param>
        /// <param name="successResponse">The response to send back upon successful authentication</param>
        /// <returns>The TwitchConnection object</returns>
        public static async Task<TwitchConnection> ConnectViaLocalhostOAuthBrowser(string clientID, string clientSecret, IEnumerable<OAuthClientScopeEnum> scopes, bool forceApprovalPrompt = false, string oauthListenerURL = DEFAULT_OAUTH_LOCALHOST_URL, string successResponse = null)
        {
            Validator.ValidateString(clientID, "clientID");
            Validator.ValidateList(scopes, "scopes");

            LocalOAuthHttpListenerServer oauthServer = new LocalOAuthHttpListenerServer(oauthListenerURL, DEFAULT_AUTHORIZATION_CODE_URL_PARAMETER, successResponse);
            oauthServer.Start();

            string url = await TwitchConnection.GetAuthorizationCodeURLForOAuthBrowser(clientID, scopes, oauthListenerURL, forceApprovalPrompt);
            ProcessStartInfo startInfo = new ProcessStartInfo() { FileName = url, UseShellExecute = true };
            Process.Start(startInfo);

            string authorizationCode = await oauthServer.WaitForAuthorizationCode();
            oauthServer.Stop();

            if (authorizationCode != null)
            {
                return await TwitchConnection.ConnectViaAuthorizationCode(clientID, clientSecret, authorizationCode, redirectUrl: oauthListenerURL);
            }
            return null;
        }

        /// <summary>
        /// Creates a TwitchConnection object from an authorization code.
        /// </summary>
        /// <param name="clientID">The ID of the client application</param>
        /// <param name="clientSecret">The secret of the client application</param>
        /// <param name="authorizationCode">The authorization code for the authenticated user</param>
        /// <param name="redirectUrl">The redirect URL of the client application</param>
        /// <returns>The TwitchConnection object</returns>
        public static async Task<TwitchConnection> ConnectViaAuthorizationCode(string clientID, string clientSecret, string authorizationCode, string redirectUrl = null)
        {
            Validator.ValidateString(clientID, "clientID");
            Validator.ValidateString(authorizationCode, "authorizationCode");

            OAuthService oauthService = new OAuthService();
            OAuthTokenModel token = await oauthService.GetOAuthTokenModel(clientID, clientSecret, authorizationCode, redirectUrl);
            if (token == null)
            {
                throw new InvalidOperationException("OAuth token was not acquired");
            }
            return new TwitchConnection(token);
        }

        /// <summary>
        /// Creates a TwitchConnection object from an OAuth token.
        /// </summary>
        /// <param name="token">The OAuth token to use</param>
        /// <param name="refreshToken">Whether to refresh the token</param>
        /// <returns>The TwitchConnection object</returns>
        public static async Task<TwitchConnection> ConnectViaOAuthToken(OAuthTokenModel token, bool refreshToken = true)
        {
            Validator.ValidateVariable(token, "token");

            TwitchConnection connection = new TwitchConnection(token);
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

        private TwitchConnection(OAuthTokenModel token)
        {
            Validator.ValidateVariable(token, "token");

            this.token = token;

            this.OAuth = new OAuthService(this);
            this.NewAPI = new NewTwitchAPIServices(this);
            this.V5API = new V5APIServices(this);
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
