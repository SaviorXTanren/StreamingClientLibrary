using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Twitch.Extensions.Base.Models;
using Twitch.Extensions.Base.Services;

namespace Twitch.ExtensionSample.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SampleController : ControllerBase
    {
        public string clientID = "";
        public string clientSecret = "";
        public string ownerID = "";

        [HttpGet("test")]
        public async Task<ActionResult<string>> Test()
        {
            if (!this.Request.Headers.TryGetValue("Authorization", out StringValues authHeader))
            {
                return this.Unauthorized("Missing Authorization header");
            }

            string bearerToken = authHeader.ToString();
            if (!string.IsNullOrEmpty(bearerToken))
            {
                bearerToken = bearerToken.Replace("Bearer ", "");
            }

            string result = TwitchAuthenticationService.ValidateAuthenticationToken(bearerToken, clientSecret, out TwitchJWTTokenPayloadModel payload);
            if (!string.IsNullOrEmpty(result))
            {
                return this.Unauthorized(result);
            }

            JObject data = new JObject();
            data["hello"] = "world";
            await TwitchBroadcastService.SendBroadcast(clientID, clientSecret, ownerID, payload.channel_id, data);

            return this.Ok("Broadcast Sent");
        }
    }
}
