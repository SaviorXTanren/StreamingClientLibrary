using Microsoft.IdentityModel.Tokens;
using StreamingClient.Base.Util;
using StreamingClient.Base.Web;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Twitch.Extensions.Base.Models;

namespace Twitch.Extensions.Base.Services
{
    public static class TwitchBroadcastService
    {
        /// <summary>
        /// Sends a broadcast to PubSub for the extension.
        /// </summary>
        /// <param name="clientID">The client ID of the extension</param>
        /// <param name="clientSecret">The client secret of the extension</param>
        /// <param name="ownerID">The owner user ID of the extension</param>
        /// <param name="channelID">The channel ID to broadcast to</param>
        /// <param name="data">The data to broadcast</param>
        /// <returns>Whether the broadcast was successful or not. Throws a HttpRestRequestException in the event of failed request</returns>
        public static async Task<bool> SendBroadcast(string clientID, string clientSecret, string ownerID, string channelID, object data)
        {
            ClaimsIdentity claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim("channel_id", channelID));
            claimsIdentity.AddClaim(new Claim("user_id", ownerID));
            claimsIdentity.AddClaim(new Claim("role", "external"));
            claimsIdentity.AddClaim(new Claim("pubsub_perms", "{ \"send\": [ \"*\" ] }", "JSON"));

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken token = tokenHandler.CreateJwtSecurityToken(subject: claimsIdentity, expires: DateTime.UtcNow.AddSeconds(30),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Convert.FromBase64String(clientSecret)), "HS256"));

            using (AdvancedHttpClient client = new AdvancedHttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Client-Id", clientID);
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenHandler.WriteToken(token));

                HttpResponseMessage response = await client.PostAsync("https://api.twitch.tv/extensions/message/" + channelID,
                    AdvancedHttpClient.CreateContentFromString(JSONSerializerHelper.SerializeToString(new BroadcastBodyModel(data))));
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    throw new HttpRestRequestException(response);
                }
            }
        }
    }
}
