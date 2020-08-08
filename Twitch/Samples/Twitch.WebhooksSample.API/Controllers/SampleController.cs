using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Twitch.Base.Models.NewAPI.Users;
using Twitch.Webhooks.Base.Models;
using Twitch.Webhooks.Base.Services;

namespace Twitch.WebHooksSample.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SampleController : ControllerBase
    {
        private const string secret = null;

        private static UserModel user;

        [HttpGet("userchanged")]
        public ActionResult GetUserChanged()
        {
            WebhookSubscriptionVerificationModel verification = TwitchWebhookService.VerifySubscriptionRequest(this.Request);
            if (verification.IsSubscribe)
            {
                if (verification.IsSuccessful)
                {
                    return this.Ok(verification.challenge);
                }
                else
                {
                    return this.BadRequest();
                }
            }
            else
            {
                return this.Ok();
            }
        }

        [HttpPost("userchanged")]
        public async Task<ActionResult> PostUserChanged()
        {
            try
            {
                WebhookResultModel<UserModel> result = await TwitchWebhookService.GetWebhookResult<UserModel>(this.Request, secret);
                if (result.IsVerified)
                {
                    user = result.Data.FirstOrDefault();
                }
                return this.Ok();
            }
            catch (Exception)
            {
                return this.BadRequest();
            }
        }

        [HttpGet("user")]
        public ActionResult GetUser()
        {
            return this.Ok(user);
        }
    }
}
