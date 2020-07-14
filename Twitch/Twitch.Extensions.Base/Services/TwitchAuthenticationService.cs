using Microsoft.IdentityModel.Tokens;
using StreamingClient.Base.Util;
using System;
using System.IdentityModel.Tokens.Jwt;
using Twitch.Extensions.Base.Models;

namespace Twitch.Extensions.Base.Services
{
    /// <summary>
    /// Service that handles authentication validation for Twitch JWT tokens via Extensions.
    /// </summary>
    public static class TwitchAuthenticationService
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
    }
}
