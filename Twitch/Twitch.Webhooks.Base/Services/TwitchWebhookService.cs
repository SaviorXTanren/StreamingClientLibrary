using Microsoft.AspNetCore.Http;
using StreamingClient.Base.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Twitch.Webhooks.Base.Models;

namespace Twitch.Webhooks.Base.Services
{
    /// <summary>
    /// Service that handles interacting with Twitch webhook services.
    /// </summary>
    public static class TwitchWebhookService
    {
        private class WebhookDataWrapperModel<T>
        {
            public List<T> data { get; set; } = new List<T>();
        }

        /// <summary>
        /// Processes the verification of a subscribe or unsubscribe request.
        /// </summary>
        /// <param name="request">The Http request made</param>
        /// <returns>The subscription verification</returns>
        public static WebhookSubscriptionVerificationModel VerifySubscriptionRequest(HttpRequest request)
        {
            return new WebhookSubscriptionVerificationModel(request.Query);
        }

        /// <summary>
        /// Processes the webhook request sent from Twitch.
        /// </summary>
        /// <typeparam name="T">The type of data contained in the webhook request</typeparam>
        /// <param name="request">The Http request made</param>
        /// <param name="secret">The optional secret used for request verification</param>
        /// <returns>The webhook result</returns>
        public static async Task<WebhookResultModel<T>> GetWebhookResult<T>(HttpRequest request, string secret = null)
        {
            string headerSignature = null;
            if (request.Headers.ContainsKey("X-Hub-Signature"))
            {
                headerSignature = request.Headers["X-Hub-Signature"];
                headerSignature = headerSignature.Split('=').LastOrDefault();
            }
            long headerContentLength = request.ContentLength.GetValueOrDefault();

            string body = string.Empty;
            long bodyContentLength = 0;
            string bodySignature = null;
            using (StreamReader reader = new StreamReader(request.Body))
            {
                body = await reader.ReadToEndAsync();
            }

            Dictionary<string, string> queryParameters = new Dictionary<string, string>();
            foreach (var kvp in request.Query)
            {
                queryParameters[kvp.Key] = kvp.Value;
            }

            WebhookDataWrapperModel<T> dataWrapper = null;
            if (!string.IsNullOrEmpty(body))
            {
                bodyContentLength = body.Length;
                if (!string.IsNullOrEmpty(secret))
                {
                    byte[] secretBytes = Encoding.UTF8.GetBytes(secret);
                    using (HMACSHA256 hmacSha256 = new HMACSHA256(secretBytes))
                    {
                        byte[] hashBytes = hmacSha256.ComputeHash(Encoding.UTF8.GetBytes(body));
                        bodySignature = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                    }
                }
                dataWrapper = JSONSerializerHelper.DeserializeFromString<WebhookDataWrapperModel<T>>(body);
            }

            return new WebhookResultModel<T>(headerSignature, headerContentLength, bodySignature, bodyContentLength, queryParameters, (dataWrapper != null) ? dataWrapper.data : new List<T>());
        }
    }
}
