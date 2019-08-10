using Mixer.Base.Services;
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

[assembly: InternalsVisibleTo("Mixer.Base.UnitTests")]

namespace Mixer.Base
{
    /// <summary>
    /// https://dev.mixer.com/reference/oauth/scopes
    /// </summary>
    public enum OAuthClientScopeEnum
    {
        /// <summary>
        /// View your earned achievements.
        /// </summary>
        achievement__view__self,
        /// <summary>
        /// View your channel analytics.
        /// </summary>
        channel__analytics__self,
        /// <summary>
        /// Manage your costreaming requests.
        /// </summary>
        channel__costream__self,
        /// <summary>
        /// Delete your channel banner
        /// </summary>
        channel__deleteBanner__self,
        /// <summary>
        /// View your channel details.
        /// </summary>
        channel__details__self,
        /// <summary>
        /// Follow and unfollow other channels.
        /// </summary>
        channel__follow__self,
        /// <summary>
        /// Create and view partnership applications.
        /// </summary>
        channel__partnership,
        /// <summary>
        /// Manage your partnership status.
        /// </summary>
        channel__partnership__self,
        /// <summary>
        /// View your channel's stream key.
        /// </summary>
        channel__streamKey__self,
        /// <summary>
        /// Update your channel settings
        /// </summary>
        channel__update__self,
        /// <summary>
        /// Create new clips from videos on your channel.
        /// </summary>
        channel__clip__create__self,
        /// <summary>
        /// Allows deleting existing clips on your channel.
        /// </summary>
        channel__clip__delete__self,
        /// <summary>
        /// Bypasses the catbot chat filter.
        /// </summary>
        chat__bypass_catbot,
        /// <summary>
        /// Bypass the chat content filter.
        /// </summary>
        chat__bypass_filter,
        /// <summary>
        /// Bypass links being disallowed in chat.
        /// </summary>
        chat__bypass_links,
        /// <summary>
        /// Bypass slowchat settings on channels.
        /// </summary>
        chat__bypass_slowchat,
        /// <summary>
        /// Cancel a skill.
        /// </summary>
        chat__cancel_skill,
        /// <summary>
        /// Manage bans in chats.
        /// </summary>
        chat__change_ban,
        /// <summary>
        /// Manage roles in chats.
        /// </summary>
        chat__change_role,
        /// <summary>
        /// Interact with chats on your behalf.
        /// </summary>
        chat__chat,
        /// <summary>
        /// Clear messages in chats where authorized.
        /// </summary>
        chat__clear_messages,
        /// <summary>
        /// Connect to chat.
        /// </summary>
        chat__connect,
        /// <summary>
        /// Edit chat options, including links settings and slowchat.
        /// </summary>
        chat__edit_options,
        /// <summary>
        /// Start a giveaway in chats where authorized.
        /// </summary>
        chat__giveaway_start,
        /// <summary>
        /// Start a poll in chats where authorized.
        /// </summary>
        chat__poll_start,
        /// <summary>
        /// Vote in chat polls.
        /// </summary>
        chat__poll_vote,
        /// <summary>
        /// Clear all messages from a specific user in chat.
        /// </summary>
        chat__purge,
        /// <summary>
        /// Remove own and other's messages in chat.
        /// </summary>
        chat__remove_message,
        /// <summary>
        /// Change timeout settings in chats.
        /// </summary>
        chat__timeout,
        /// <summary>
        /// View deleted messages in chat.
        /// </summary>
        chat__view_deleted,
        /// <summary>
        /// Gives the ability to whisper in a channel
        /// </summary>
        chat__whisper,
        /// <summary>
        /// View your Mixer homepage experience and recommendations.
        /// </summary>
        delve__view__self,
        /// <summary>
        /// Create, update and delete the interactive games in your account.
        /// </summary>
        interactive__manage__self,
        /// <summary>
        /// Run as an interactive game in your channel.
        /// </summary>
        interactive__robot__self,
        /// <summary>
        /// View the users invoices.
        /// </summary>
        invoice__view__self,
        /// <summary>
        /// View and manage your security log.
        /// </summary>
        log__view__self,
        /// <summary>
        /// View and manage your OAuth clients.
        /// </summary>
        oauth__manage__self,
        /// <summary>
        /// Manage the users VODs.
        /// </summary>
        recording__manage__self,
        /// <summary>
        /// Create redeemables after performing a purchase.
        /// </summary>
        redeemable__create__self,
        /// <summary>
        /// Use users redeemable.
        /// </summary>
        redeemable__redeem__self,
        /// <summary>
        /// View users redeemables.
        /// </summary>
        redeemable__view__self,
        /// <summary>
        /// View emoticons and other graphical resources you have access to.
        /// </summary>
        resource__find__self,
        /// <summary>
        /// Cancel your subscriptions.
        /// </summary>
        subscription__cancel__self,
        /// <summary>
        /// Create new subscriptions.
        /// </summary>
        subscription__create__self,
        /// <summary>
        /// Renew your existing subscriptions.
        /// </summary>
        subscription__renew__self,
        /// <summary>
        /// View who you're subscribed to.
        /// </summary>
        subscription__view__self,
        /// <summary>
        /// Administrate teams the user has rights in.
        /// </summary>
        team__administer,
        /// <summary>
        /// Create, join, leave teams and set the users primary team.
        /// </summary>
        team__manage__self,
        /// <summary>
        /// Cancel pending transactions.
        /// </summary>
        transaction__cancel__self,
        /// <summary>
        /// View your pending transactions.
        /// </summary>
        transaction__view__self,
        /// <summary>
        /// Let's you act as this user on other resources.
        /// </summary>
        user__act_as,
        /// <summary>
        /// View your user analytics
        /// </summary>
        user__analytics__self,
        /// <summary>
        /// View your email address and other private details.
        /// </summary>
        user__details__self,
        /// <summary>
        /// View users discord invites.
        /// </summary>
        user__getDiscordInvite__self,
        /// <summary>
        /// View your user security log.
        /// </summary>
        user__log__self,
        /// <summary>
        /// View and manage your notifications.
        /// </summary>
        user__notification__self,
        /// <summary>
        /// Mark a VOD as seen for the user.
        /// </summary>
        user__seen__self,
        /// <summary>
        /// Update your account, including your email but not your password.
        /// </summary>
        user__update__self,
        /// <summary>
        /// Update your password.
        /// </summary>
        user__updatePassword__self,
    }

    /// <summary>
    /// The main connection handler for Mixer.
    /// </summary>
    public class MixerConnection
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
        /// APIs for the broadcasts of a channel.
        /// </summary>
        public BroadcastsService Broadcasts { get; private set; }

        /// <summary>
        /// APIs for channel information.
        /// </summary>
        public ChannelsService Channels { get; private set; }

        /// <summary>
        /// APIs for chat services.
        /// </summary>
        public ChatsService Chats { get; private set; }

        /// <summary>
        /// APIs for clip data &amp; creation.
        /// </summary>
        public ClipsService Clips { get; private set; }

        /// <summary>
        /// APIs for costream broadcasts.
        /// </summary>
        public CostreamService Costream { get; private set; }

        /// <summary>
        /// APIs for game type look up.
        /// </summary>
        public GameTypesService GameTypes { get; private set; }

        /// <summary>
        /// APIs for Interactive/MixPlay.
        /// </summary>
        public MixPlayService MixPlay { get; private set; }

        /// <summary>
        /// APIs for the leaderboards of a channel.
        /// </summary>
        public LeaderboardsService Leaderboards { get; private set; }

        /// <summary>
        /// APIs for OAuth interaction.
        /// </summary>
        public OAuthService OAuth { get; private set; }

        /// <summary>
        /// APIs for channel patronage.
        /// </summary>
        public PatronageService Patronage { get; private set; }

        /// <summary>
        /// APIs for skills.
        /// </summary>
        public SkillsService Skills { get; private set; }

        /// <summary>
        /// APIs for teams.
        /// </summary>
        public TeamsService Teams { get; private set; }

        /// <summary>
        /// APIs for test stream setup.
        /// </summary>
        public TestStreamsService TestStreams { get; private set; }

        /// <summary>
        /// APIs for user data.
        /// </summary>
        public UsersService Users { get; private set; }

        /// <summary>
        /// NOTE: There is a known issue with the Mixer APIs where authenticating with a short code as opposed to the regular OAuth process, where certain
        /// Chat Client commands will not work (EX: Timeout, Clear Messages, Delete Message, etc). The current work around to this is to use the traditional
        /// OAuth authentication methods.
        /// </summary>
        /// <param name="clientID">The ID of the client application</param>
        /// <param name="scopes">the scopes to request</param>
        /// <param name="codeCallback">The callback for the short code</param>
        /// <returns>The connection to Mixer</returns>
        public static async Task<MixerConnection> ConnectViaShortCode(string clientID, IEnumerable<OAuthClientScopeEnum> scopes, Action<OAuthShortCodeModel> codeCallback)
        {
            return await MixerConnection.ConnectViaShortCode(clientID, null, scopes, codeCallback);
        }

        /// <summary>
        /// NOTE: There is a known issue with the Mixer APIs where authenticating with a short code as opposed to the regular OAuth process, where certain
        /// Chat Client commands will not work (EX: Timeout, Clear Messages, Delete Message, etc). The current work around to this is to use the traditional
        /// OAuth authentication methods.
        /// </summary>
        /// <param name="clientID">The ID of the client application</param>
        /// <param name="clientSecret">The secret of the client application</param>
        /// <param name="scopes">the scopes to request</param>
        /// <param name="codeCallback">The callback for the short code</param>
        /// <returns>The connection to Mixer</returns>
        public static async Task<MixerConnection> ConnectViaShortCode(string clientID, string clientSecret, IEnumerable<OAuthClientScopeEnum> scopes, Action<OAuthShortCodeModel> codeCallback)
        {
            Validator.ValidateString(clientID, "clientID");
            Validator.ValidateList(scopes, "scopes");
            Validator.ValidateVariable(codeCallback, "codeCallback");

            OAuthService oauthService = new OAuthService();
            OAuthShortCodeModel shortCode = await oauthService.GetShortCode(clientID, clientSecret, scopes);

            codeCallback(shortCode);

            string authorizationCode = null;
            for (int i = 0; i < shortCode.expiresIn && string.IsNullOrEmpty(authorizationCode); i++)
            {
                await Task.Delay(500);
                authorizationCode = await oauthService.ValidateShortCode(shortCode);
            }

            if (!string.IsNullOrEmpty(authorizationCode))
            {
                return await MixerConnection.ConnectViaAuthorizationCode(clientID, clientSecret, authorizationCode, redirectUrl: null);
            }
            return null;
        }

        /// <summary>
        /// Gets the authorization code URL for the specified parameters.
        /// </summary>
        /// <param name="clientID">The ID of the client application</param>
        /// <param name="scopes">the scopes to request</param>
        /// <param name="redirectUri">The URI to redirect to after authorization</param>
        /// <param name="forceApprovalPrompt">Whether to force the user to approve</param>
        /// <returns>The authorization code URL</returns>
        public static async Task<string> GetAuthorizationCodeURLForOAuthBrowser(string clientID, IEnumerable<OAuthClientScopeEnum> scopes, string redirectUri, bool forceApprovalPrompt = false)
        {
            return await MixerConnection.GetAuthorizationCodeURLForOAuthBrowser(clientID, null, scopes, redirectUri, forceApprovalPrompt);
        }

        /// <summary>
        /// Gets the authorization code URL for the specified parameters.
        /// </summary>
        /// <param name="clientID">The ID of the client application</param>
        /// <param name="clientSecret">The secret of the client application</param>
        /// <param name="scopes">the scopes to request</param>
        /// <param name="redirectUri">The URI to redirect to after authorization</param>
        /// <param name="forceApprovalPrompt">Whether to force the user to approve</param>
        /// <returns>The authorization code URL</returns>
        public static async Task<string> GetAuthorizationCodeURLForOAuthBrowser(string clientID, string clientSecret, IEnumerable<OAuthClientScopeEnum> scopes, string redirectUri, bool forceApprovalPrompt = false)
        {
            Validator.ValidateString(clientID, "clientID");
            Validator.ValidateList(scopes, "scopes");
            Validator.ValidateString(redirectUri, "redirectUri");

            string url = "https://mixer.com/oauth/authorize";

            Dictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "client_id", clientID },
                { "scope", MixerConnection.ConvertClientScopesToString(scopes) },
                { "response_type", "code" },
                { "redirect_uri", redirectUri },
            };

            if (!string.IsNullOrEmpty(clientSecret))
            {
                parameters.Add("client_secret", clientSecret);
            }

            if (forceApprovalPrompt)
            {
                parameters.Add("approval_prompt", "force");
            }

            FormUrlEncodedContent content = new FormUrlEncodedContent(parameters.AsEnumerable());

            return url + "?" + await content.ReadAsStringAsync();
        }

        /// <summary>
        /// Establishes a connection to Mixer via a localhost OAuth browser.
        /// </summary>
        /// <param name="clientID">The ID of the client application</param>
        /// <param name="scopes">the scopes to request</param>
        /// <param name="oauthListenerURL">The URI to redirect to after authorization</param>
        /// <param name="loginSuccessHtmlPageFilePath">The HTML to show upon successful login</param>
        /// <param name="forceApprovalPrompt">Whether to force the user to approve</param>
        /// <returns>The connection to Mixer</returns>
        public static async Task<MixerConnection> ConnectViaLocalhostOAuthBrowser(string clientID, IEnumerable<OAuthClientScopeEnum> scopes, bool forceApprovalPrompt = false, string oauthListenerURL = DEFAULT_OAUTH_LOCALHOST_URL, string loginSuccessHtmlPageFilePath = null)
        {
            return await ConnectViaLocalhostOAuthBrowser(clientID, null, scopes, forceApprovalPrompt, oauthListenerURL: oauthListenerURL, loginSuccessHtmlPageFilePath: loginSuccessHtmlPageFilePath);
        }

        /// <summary>
        /// Establishes a connection to Mixer via a localhost OAuth browser.
        /// </summary>
        /// <param name="clientID">The ID of the client application</param>
        /// <param name="clientSecret">The secret of the client application</param>
        /// <param name="scopes">the scopes to request</param>
        /// <param name="oauthListenerURL">The URI to redirect to after authorization</param>
        /// <param name="loginSuccessHtmlPageFilePath">The HTML to show upon successful login</param>
        /// <param name="forceApprovalPrompt">Whether to force the user to approve</param>
        /// <returns>The connection to Mixer</returns>
        public static async Task<MixerConnection> ConnectViaLocalhostOAuthBrowser(string clientID, string clientSecret, IEnumerable<OAuthClientScopeEnum> scopes, bool forceApprovalPrompt = false, string oauthListenerURL = DEFAULT_OAUTH_LOCALHOST_URL, string loginSuccessHtmlPageFilePath = null)
        {
            Validator.ValidateString(clientID, "clientID");
            Validator.ValidateList(scopes, "scopes");

            LocalOAuthHttpListenerServer oauthServer = new LocalOAuthHttpListenerServer(oauthListenerURL, DEFAULT_AUTHORIZATION_CODE_URL_PARAMETER, loginSuccessHtmlPageFilePath);
            oauthServer.Start();

            string url = await MixerConnection.GetAuthorizationCodeURLForOAuthBrowser(clientID, scopes, oauthListenerURL, forceApprovalPrompt);
            ProcessStartInfo startInfo = new ProcessStartInfo() { FileName = url, UseShellExecute = true };
            Process.Start(startInfo);

            string authorizationCode = await oauthServer.WaitForAuthorizationCode();
            oauthServer.Stop();

            if (authorizationCode != null)
            {
                return await MixerConnection.ConnectViaAuthorizationCode(clientID, clientSecret, authorizationCode, redirectUrl: oauthListenerURL);
            }
            return null;
        }

        /// <summary>
        /// Establishes a connection to Mixer via an authorization code
        /// </summary>
        /// <param name="clientID">The ID of the client application</param>
        /// <param name="authorizationCode">The secret of the client application</param>
        /// <param name="redirectUrl">The URI to redirect to after authorization</param>
        /// <returns>The connection to Mixer</returns>
        public static async Task<MixerConnection> ConnectViaAuthorizationCode(string clientID, string authorizationCode, string redirectUrl = null)
        {
            return await MixerConnection.ConnectViaAuthorizationCode(clientID, null, authorizationCode, redirectUrl);
        }

        /// <summary>
        /// Establishes a connection to Mixer via an authorization code
        /// </summary>
        /// <param name="clientID">The ID of the client application</param>
        /// <param name="clientSecret">The secret of the client application</param>
        /// <param name="authorizationCode">The secret of the client application</param>
        /// <param name="redirectUrl">The URI to redirect to after authorization</param>
        /// <returns>The connection to Mixer</returns>
        public static async Task<MixerConnection> ConnectViaAuthorizationCode(string clientID, string clientSecret, string authorizationCode, string redirectUrl = null)
        {
            Validator.ValidateString(clientID, "clientID");
            Validator.ValidateString(authorizationCode, "authorizationCode");

            OAuthService oauthService = new OAuthService();
            OAuthTokenModel token = await oauthService.GetOAuthTokenModel(clientID, clientSecret, authorizationCode, redirectUrl);
            if (token == null)
            {
                throw new InvalidOperationException("OAuth token was not acquired");
            }
            return new MixerConnection(token);
        }

        /// <summary>
        /// Establishes a connection to Mixer via an OAuth token.
        /// </summary>
        /// <param name="token">An OAuth token</param>
        /// <param name="refreshToken">Whether to refresh the token</param>
        /// <returns>The connection to Mixer</returns>
        public static async Task<MixerConnection> ConnectViaOAuthToken(OAuthTokenModel token, bool refreshToken = true)
        {
            Validator.ValidateVariable(token, "token");

            MixerConnection connection = new MixerConnection(token);
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

        private MixerConnection(OAuthTokenModel token)
        {
            Validator.ValidateVariable(token, "token");

            this.token = token;

            this.Broadcasts = new BroadcastsService(this);
            this.Channels = new ChannelsService(this);
            this.Chats = new ChatsService(this);
            this.Clips = new ClipsService(this);
            this.Costream = new CostreamService(this);
            this.GameTypes = new GameTypesService(this);
            this.Leaderboards = new LeaderboardsService(this);
            this.MixPlay = new MixPlayService(this);
            this.OAuth = new OAuthService(this);
            this.Patronage = new PatronageService(this);
            this.Skills = new SkillsService(this);
            this.Teams = new TeamsService(this);
            this.TestStreams = new TestStreamsService(this);
            this.Users = new UsersService(this);
        }

        /// <summary>
        /// Refreshes the OAuth token associated with the connection.
        /// </summary>
        /// <returns>An awaitable Task</returns>
        public async Task RefreshOAuthToken() { this.token = await this.OAuth.RefreshToken(this.token); }

        /// <summary>
        /// Gets a copy of the current OAuth token.
        /// </summary>
        /// <returns>A copy of the current OAuth token</returns>
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
