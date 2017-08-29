using Mixer.Base.Model.Costream;
using Mixer.Base.Model.User;
using Mixer.Base.Util;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Mixer.Base.Services
{
    /// <summary>
    /// The APIs for costream-based services.
    /// </summary>
    public class CostreamService : ServiceBase
    {
        public CostreamService(MixerConnection connection) : base(connection) { }

        public async Task<CostreamModel> GetCostream(Guid costreamID)
        {
            Validator.ValidateVariable(costreamID, "costreamID");
            return await this.GetAsync<CostreamModel>("costreams/" + costreamID.ToString());
        }

        public async Task<CostreamModel> UpdateCostream(Guid costreamID, CostreamModel costream)
        {
            Validator.ValidateVariable(costreamID, "costreamID");
            Validator.ValidateVariable(costream, "costream");
            return await this.PatchAsync<CostreamModel>("costreams/" + costreamID.ToString(), this.CreateContentFromObject(costream));
        }

        public async Task<bool> RemoveChannelFromCostream(Guid costreamID, CostreamChannelModel channel)
        {
            Validator.ValidateVariable(costreamID, "costreamID");
            Validator.ValidateVariable(channel, "channel");
            return await this.DeleteAsync("costreams/" + costreamID.ToString() + "/channels/" + channel.id);
        }

        public async Task<bool> InviteToCostream(List<UserModel> users)
        {
            Validator.ValidateList(users, "users");

            JObject payload = new JObject();
            payload["validFor"] = 300000;   // Invite valid for 5 minutes
            payload["inviteeIds"] = new JArray(users);

            HttpResponseMessage response = await this.PostAsync("costreams/invite", this.CreateContentFromObject(payload));
            return (response.StatusCode == System.Net.HttpStatusCode.NoContent);
        }

        public async Task<CostreamModel> GetCurrentCostream()
        {
            return await this.GetAsync<CostreamModel>("costreams/current");
        }

        public async Task<bool> LeaveCurrentCostream()
        {
            return await this.DeleteAsync("costreams/current");
        }
    }
}
