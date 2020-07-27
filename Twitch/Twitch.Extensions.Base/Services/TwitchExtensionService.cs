using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using StreamingClient.Base.Util;
using StreamingClient.Base.Web;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Twitch.Extensions.Base.Models;

namespace Twitch.Extensions.Base.Services
{
    /// <summary>
    /// Service that handles interacting with Twitch extension services.
    /// </summary>
    public static class TwitchExtensionService
    {
        /// <summary>
        /// Validates whether the specific JWT bearer token is valid and returns the payload of the token if successful.
        /// </summary>
        /// <param name="bearerToken">The authentication bearer token</param>
        /// <param name="clientSecret">The client secret of the extension</param>
        /// <param name="payload">The payload contained in the JWT bearer token</param>
        /// <returns>An error message indicating why the validation failed, null if successful</returns>
        public static string ValidateAuthenticationToken(string bearerToken, string clientSecret, out TwitchJWTTokenPayloadModel payload)
        {
            Validator.ValidateString(bearerToken, "bearerToken");
            Validator.ValidateString(clientSecret, "clientSecret");

            payload = null;
            if (!string.IsNullOrEmpty(bearerToken))
            {
                bearerToken = bearerToken.Replace("Bearer ", "");
            }

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            if (!tokenHandler.CanReadToken(bearerToken))
            {
                return "Invalid JWT token";
            }

            var validationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(clientSecret))
            };

            JwtSecurityToken token = null;
            try
            {
                tokenHandler.ValidateToken(bearerToken, validationParameters, out SecurityToken validatedToken);
                if (validatedToken != null)
                {
                    token = (JwtSecurityToken)validatedToken;
                }
            }
            catch (Exception ex)
            {
                return "Token validation failed - " + ex.ToString();
            }

            if (token != null && token.Payload != null)
            {
                payload = JSONSerializerHelper.DeserializeFromString<TwitchJWTTokenPayloadModel>(token.Payload.SerializeToJson());
                if (payload != null && !string.IsNullOrEmpty(payload.channel_id) && !string.IsNullOrEmpty(payload.opaque_user_id))
                {
                    return null;
                }
            }
            return "Mising or invalid payload";
        }

        /// <summary>
        /// Creates a JWT authentication token for use with API calls.
        /// </summary>
        /// <param name="clientSecret">The client secret of the extension</param>
        /// <param name="ownerID">The owner user ID of the extension</param>
        /// <param name="channelID">The channel ID to broadcast to</param>
        /// <returns>The serialized JWT authentication token</returns>
        public static string CreateAuthenticationToken(string clientSecret, string ownerID, string channelID)
        {
            Validator.ValidateString(clientSecret, "clientSecret");
            Validator.ValidateString(ownerID, "ownerID");

            ClaimsIdentity claimsIdentity = new ClaimsIdentity();
            if (!string.IsNullOrEmpty(channelID))
            {
                claimsIdentity.AddClaim(new Claim("channel_id", channelID));
            }
            claimsIdentity.AddClaim(new Claim("user_id", ownerID));
            claimsIdentity.AddClaim(new Claim("role", "external"));
            claimsIdentity.AddClaim(new Claim("pubsub_perms", "{ \"send\": [ \"*\" ] }", "JSON"));

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken token = tokenHandler.CreateJwtSecurityToken(subject: claimsIdentity, expires: DateTime.UtcNow.AddSeconds(30),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Convert.FromBase64String(clientSecret)), "HS256"));

            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Sets the configuration data
        /// </summary>
        /// <param name="clientID">The client ID of the extension</param>
        /// <param name="clientSecret">The client secret of the extension</param>
        /// <param name="ownerID">The owner user ID of the extension</param>
        /// <param name="channelID">The channel ID to broadcast to</param>
        /// <param name="segment">The segment type of the configuration</param>
        /// <param name="version">The version for the configuration</param>
        /// <param name="data">The data to set for the configuration</param>
        /// <returns>An awaitable task</returns>
        /// <exception cref="HttpRestRequestException">Throw in the event of a failed request</exception>
        public static async Task SetConfiguration(string clientID, string clientSecret, string ownerID, string channelID, string segment, string version, object data)
        {
            Validator.ValidateString(clientID, "clientID");
            Validator.ValidateString(clientSecret, "clientSecret");
            Validator.ValidateString(ownerID, "ownerID");
            Validator.ValidateString(segment, "segment");
            Validator.ValidateString(version, "version");
            Validator.ValidateVariable(data, "data");

            using (AdvancedHttpClient client = TwitchExtensionService.GetHttpClient(clientID, clientSecret, ownerID, channelID))
            {
                ConfigurationModel configuration = (ConfigurationModel.GlobalConfigurationSegmentValue.Equals(segment)) ?
                    new ConfigurationModel(segment, version, JSONSerializerHelper.SerializeToString(data)) :
                    new ChannelConfigurationModel(channelID, segment, version, JSONSerializerHelper.SerializeToString(data));

                HttpResponseMessage response = await client.PutAsync($"https://api.twitch.tv/extensions/{clientID}/configurations/",
                    AdvancedHttpClient.CreateContentFromString(JSONSerializerHelper.SerializeToString(configuration)));
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRestRequestException(response);
                }
            }
        }

        /// <summary>
        /// Gets the configuration data for the specified channel.
        /// </summary>
        /// <param name="clientID">The client ID of the extension</param>
        /// <param name="clientSecret">The client secret of the extension</param>
        /// <param name="ownerID">The owner user ID of the extension</param>
        /// <param name="channelID">The channel ID to broadcast to</param>
        /// <returns>The channel-specific configuration data for the extension</returns>
        public static async Task<IEnumerable<ChannelConfigurationModel>> GetChannelConfiguration(string clientID, string clientSecret, string ownerID, string channelID)
        {
            Validator.ValidateString(clientID, "clientID");
            Validator.ValidateString(clientSecret, "clientSecret");
            Validator.ValidateString(ownerID, "ownerID");
            Validator.ValidateString(channelID, "channelID");

            using (AdvancedHttpClient client = TwitchExtensionService.GetHttpClient(clientID, clientSecret, ownerID, channelID))
            {
                HttpResponseMessage response = await client.GetAsync($"https://api.twitch.tv/extensions/{clientID}/configurations/segments/broadcaster?channel_id={channelID}");
                if (response.IsSuccessStatusCode)
                {
                    List<ChannelConfigurationModel> results = new List<ChannelConfigurationModel>();
                    JObject jobj = await response.ProcessJObjectResponse();
                    if (jobj != null)
                    {
                        foreach (var kvp in jobj)
                        {
                            ConfigurationResultModel result = kvp.Value.ToObject<ConfigurationResultModel>();
                            if (result != null)
                            {
                                results.Add(new ChannelConfigurationModel(result));
                            }
                        }
                    }
                    return results;
                }
                else
                {
                    return new List<ChannelConfigurationModel>();
                }
            }
        }


        /// <summary>
        /// Gets the global configuration data.
        /// </summary>
        /// <param name="clientID">The client ID of the extension</param>
        /// <param name="clientSecret">The client secret of the extension</param>
        /// <param name="ownerID">The owner user ID of the extension</param>
        /// <returns>The global configuration data for the extension</returns>
        public static async Task<ConfigurationModel> GetGlobalConfiguration(string clientID, string clientSecret, string ownerID)
        {
            Validator.ValidateString(clientID, "clientID");
            Validator.ValidateString(clientSecret, "clientSecret");
            Validator.ValidateString(ownerID, "ownerID");

            using (AdvancedHttpClient client = TwitchExtensionService.GetHttpClient(clientID, clientSecret, ownerID, null))
            {
                HttpResponseMessage response = await client.GetAsync($"https://api.twitch.tv/extensions/{clientID}/configurations/segments/global");
                if (response.IsSuccessStatusCode)
                {
                    JObject jobj = await response.ProcessJObjectResponse();
                    if (jobj != null && jobj.Count > 0)
                    {
                        ConfigurationResultModel result = jobj.Values().First().ToObject<ConfigurationResultModel>();
                        if (result != null)
                        {
                            return new ConfigurationModel(result);
                        }
                    }
                    return null;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Sends a broadcast to PubSub for the extension.
        /// </summary>
        /// <param name="clientID">The client ID of the extension</param>
        /// <param name="clientSecret">The client secret of the extension</param>
        /// <param name="ownerID">The owner user ID of the extension</param>
        /// <param name="channelID">The channel ID to broadcast to</param>
        /// <param name="data">The data to broadcast</param>
        /// <returns>An awaitable task</returns>
        /// <exception cref="HttpRestRequestException">Throw in the event of a failed request</exception>
        public static async Task SendBroadcast(string clientID, string clientSecret, string ownerID, string channelID, object data)
        {
            Validator.ValidateString(clientID, "clientID");
            Validator.ValidateString(clientSecret, "clientSecret");
            Validator.ValidateString(ownerID, "ownerID");
            Validator.ValidateString(channelID, "channelID");
            Validator.ValidateVariable(data, "data");

            using (AdvancedHttpClient client = TwitchExtensionService.GetHttpClient(clientID, clientSecret, ownerID, channelID))
            {
                HttpResponseMessage response = await client.PostAsync("https://api.twitch.tv/extensions/message/" + channelID,
                    AdvancedHttpClient.CreateContentFromString(JSONSerializerHelper.SerializeToString(new BroadcastBodyModel(data))));
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRestRequestException(response);
                }
            }
        }

        /// <summary>
        /// Sends a chat message to the specified channel from the extension.
        /// </summary>
        /// <param name="clientID">The client ID of the extension</param>
        /// <param name="clientSecret">The client secret of the extension</param>
        /// <param name="ownerID">The owner user ID of the extension</param>
        /// <param name="version">The version of the extension</param>
        /// <param name="channelID">The channel ID to broadcast to</param>
        /// <param name="message">The message to send</param>
        /// <returns>An awaitable task</returns>
        /// <exception cref="HttpRestRequestException">Throw in the event of a failed request</exception>
        public static async Task SendChatMessage(string clientID, string clientSecret, string ownerID, string version, string channelID, string message)
        {
            Validator.ValidateString(clientID, "clientID");
            Validator.ValidateString(clientSecret, "clientSecret");
            Validator.ValidateString(ownerID, "ownerID");
            Validator.ValidateString(version, "version");
            Validator.ValidateString(channelID, "channelID");
            Validator.ValidateString(message, "message");

            using (AdvancedHttpClient client = TwitchExtensionService.GetHttpClient(clientID, clientSecret, ownerID, channelID))
            {
                HttpResponseMessage response = await client.PostAsync($"https://api.twitch.tv/extensions/{clientID}/{version}/channels/{channelID}/chat",
                    AdvancedHttpClient.CreateContentFromString(JSONSerializerHelper.SerializeToString(new ExtensionChatMessageModel(message))));
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRestRequestException(response);
                }
            }
        }

        private static AdvancedHttpClient GetHttpClient(string clientID, string clientSecret, string ownerID, string channelID)
        {
            AdvancedHttpClient client = new AdvancedHttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Client-ID", clientID);
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + TwitchExtensionService.CreateAuthenticationToken(clientSecret, ownerID, channelID));
            return client;
        }
    }
}
