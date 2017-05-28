using Mixer.Base.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Mixer.Base
{
    public enum ClientScopeEnum
    {
        achievement__view__self,
        channel__analytics,
        channel__analytics__self,
        channel__costream__self,
        channel__deleteBanner,
        channel__deleteBanner__self,
        channel__details__self,
        channel__follow__self,
        channel__partnership,
        channel__partnership__self,
        channel__streamKey__self,
        channel__update__self,
        chat__bypass_links,
        chat__bypass_slowchat,
        chat__change_ban,
        chat__change_role,
        chat__chat,
        chat__clear_messages,
        chat__connect,
        chat__edit_options,
        chat__giveaway_start,
        chat__poll_start,
        chat__poll_vote,
        chat__purge,
        chat__remove_message,
        chat__timeout,
        chat__view_deleted,
        chat__whisper,
        interactive__manage__self,
        interactive__robot__self,
        invoice__view__self,
        log__view__self,
        notification__update__self,
        notification__view__self,
        recording__manage__self,
        redeemable__create__self,
        redeemable__redeem__self,
        redeemable__view__self,
        resource__find__self,
        subscription__cancel__self,
        subscription__create__self,
        subscription__renew__self,
        subscription__view__self,
        team__administer,
        team__manage__self,
        transaction__cancel__self,
        transaction__view__self,
        type__viewHidden,
        user__analytics__self,
        user__details__self,
        user__getDiscordInvite__self,
        user__log__self,
        user__notification__self,
        user__seen__self,
        user__update__self,
        user__updatePassword__self,
    }

    public class ShortCode
    {
        public string code { get; set; }
        public string handle { get; set; }
        public uint expires_in { get; set; }
    }

    public class AuthorizationToken
    {
        public static async Task<ShortCode> GenerateShortCode(string clientID, IEnumerable<ClientScopeEnum> scopes)
        {
            if (string.IsNullOrEmpty(clientID))
            {
                throw new ArgumentException("Client ID must have a valid value");
            }

            if (scopes.Count() == 0)
            {
                throw new ArgumentException("At least one scope must be specified");
            }

            using (HttpClientWrapper client = new HttpClientWrapper())
            {
                FormUrlEncodedContent content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("client_id", clientID),
                    new KeyValuePair<string, string>("client_secret", null),
                    new KeyValuePair<string, string>("scope", AuthorizationToken.ConvertClientScopesToString(scopes)),
                });

                HttpResponseMessage response = await client.PostAsync("oauth/shortcode", content);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ShortCode>(result);
                }
                else
                {
                    throw new HttpRequestException(string.Format("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase));
                }
            }
        }

        public static async Task<string> ValidateShortCode(ShortCode shortCode)
        {
            if (shortCode == null)
            {
                throw new ArgumentException("Short Code is null");
            }

            for (int i = 0; i < shortCode.expires_in; i++)
            {
                using (HttpClientWrapper client = new HttpClientWrapper())
                {
                    HttpResponseMessage response = await client.GetAsync("oauth/shortcode/check/" + shortCode.handle);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        JObject jobject = JObject.Parse(result);
                        return (string)jobject["code"];
                    }
                }
                await Task.Delay(1000);
            }
            return null;
        }

        public static async Task<AuthorizationToken> GetAuthorizationToken(string clientID, string authorizationCode)
        {
            if (string.IsNullOrEmpty(clientID))
            {
                throw new ArgumentException("Client ID must have a valid value");
            }

            if (string.IsNullOrEmpty(authorizationCode))
            {
                throw new ArgumentException("Authorization Code must have a valid value");
            }

            using (HttpClientWrapper client = new HttpClientWrapper())
            {
                FormUrlEncodedContent content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("client_id", clientID),
                    new KeyValuePair<string, string>("code", authorizationCode),
                });

                HttpResponseMessage response = await client.PostAsync("oauth/token", content);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    JObject jobject = JObject.Parse(result);
                    return new AuthorizationToken(clientID, authorizationCode, (string)jobject["access_token"], (string)jobject["refresh_token"], (int)jobject["expires_in"]);
                }
                else
                {
                    throw new HttpRequestException(string.Format("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase));
                }
            }
        }

        private static string ConvertClientScopesToString(IEnumerable<ClientScopeEnum> scopes)
        {
            string result = "";

            foreach (ClientScopeEnum scope in scopes)
            {
                string enumName = Enum.GetName(typeof(ClientScopeEnum), scope);
                if (string.IsNullOrEmpty(enumName))
                {
                    throw new ArgumentException("Invalid client scope specified: " + scope);
                }
                enumName = enumName.Replace("__", ":");
                result += enumName + " ";
            }

            if (result.Length > 0)
            {
                result = result.Substring(0, result.Length - 1);
            }

            return result;
        }

        public DateTimeOffset Expiration { get; private set; }

        internal string AccessToken { get; private set; }

        private string clientID;
        private string authorizationCode;
        private string refreshToken;

        private AuthorizationToken(string clientID, string authorizationCode, string accessToken, string refreshToken, int expiresIn)
        {
            if (string.IsNullOrEmpty(clientID))
            {
                throw new ArgumentException("Client ID must have a valid value");
            }

            if (string.IsNullOrEmpty(authorizationCode))
            {
                throw new ArgumentException("Authorization Code must have a valid value");
            }

            if (string.IsNullOrEmpty(accessToken))
            {
                throw new ArgumentException("Access Token must have a valid value");
            }

            if (string.IsNullOrEmpty(refreshToken))
            {
                throw new ArgumentException("Refresh Token must have a valid value");
            }

            if (expiresIn <= 0)
            {
                throw new ArgumentException("Expires In must be a valid value");
            }

            this.clientID = clientID;
            this.authorizationCode = authorizationCode;
            this.AccessToken = accessToken;
            this.refreshToken = refreshToken;
            this.Expiration = DateTimeOffset.Now.AddSeconds(expiresIn);
        }

        public async Task RefreshToken()
        {
            using (HttpClientWrapper client = new HttpClientWrapper())
            {
                FormUrlEncodedContent content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "refresh_token"),
                    new KeyValuePair<string, string>("client_id", this.clientID),
                    new KeyValuePair<string, string>("refresh_token", this.refreshToken),
                });

                HttpResponseMessage response = await client.PostAsync("oauth/token", content);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    JObject jobject = JObject.Parse(result);

                    this.AccessToken = (string)jobject["access_token"];
                    this.refreshToken = (string)jobject["refresh_token"];
                    this.Expiration = DateTimeOffset.Now.AddSeconds((int)jobject["expires_in"]);
                }
                else
                {
                    throw new HttpRequestException(string.Format("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase));
                }
            }
        }
    }
}
