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
    public class CostreamService : MixerServiceBase
    {
        /// <summary>
        /// Creates an instance of the CostreamService.
        /// </summary>
        /// <param name="connection">The Mixer connection to use</param>
        public CostreamService(MixerConnection connection) : base(connection) { }

        /// <summary>
        /// Get the costream with the specified id.
        /// </summary>
        /// <param name="costreamID">The costream id</param>
        /// <returns>The costream</returns>
        public async Task<CostreamModel> GetCostream(Guid costreamID)
        {
            Validator.ValidateVariable(costreamID, "costreamID");
            return await this.GetAsync<CostreamModel>("costreams/" + costreamID.ToString());
        }

        /// <summary>
        /// Updates the specified costream.
        /// </summary>
        /// <param name="costreamID">The id of the costream</param>
        /// <param name="costream">The costream to update</param>
        /// <returns>The updated costream</returns>
        public async Task<CostreamModel> UpdateCostream(Guid costreamID, CostreamModel costream)
        {
            Validator.ValidateVariable(costreamID, "costreamID");
            Validator.ValidateVariable(costream, "costream");
            return await this.PatchAsync<CostreamModel>("costreams/" + costreamID.ToString(), this.CreateContentFromObject(costream));
        }

        /// <summary>
        /// Removes the specified channel from the specified costream.
        /// </summary>
        /// <param name="costreamID">The id of the costream</param>
        /// <param name="channel">The channel to be removed</param>
        /// <returns>Whether the operation succeeded</returns>
        public async Task<bool> RemoveChannelFromCostream(Guid costreamID, CostreamChannelModel channel)
        {
            Validator.ValidateVariable(costreamID, "costreamID");
            Validator.ValidateVariable(channel, "channel");
            return await this.DeleteAsync("costreams/" + costreamID.ToString() + "/channels/" + channel.id);
        }

        /// <summary>
        /// Invites the specified users to the authenticated user's costream.
        /// </summary>
        /// <param name="users">The users to invite</param>
        /// <returns>Wheter the operation succeeded</returns>
        public async Task<bool> InviteToCostream(List<UserModel> users)
        {
            Validator.ValidateList(users, "users");

            JObject payload = new JObject();
            payload["validFor"] = 300000;   // Invite valid for 5 minutes
            payload["inviteeIds"] = new JArray(users);

            HttpResponseMessage response = await this.PostAsync("costreams/invite", this.CreateContentFromObject(payload));
            return (response.StatusCode == System.Net.HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Gets the current costream for the authenticated user.
        /// </summary>
        /// <returns>The current costreamm</returns>
        public async Task<CostreamModel> GetCurrentCostream()
        {
            return await this.GetAsync<CostreamModel>("costreams/current");
        }

        /// <summary>
        /// Leaves the current costream that the authenicated user is in
        /// </summary>
        /// <returns>Whether the operation succeeded</returns>
        public async Task<bool> LeaveCurrentCostream()
        {
            return await this.DeleteAsync("costreams/current");
        }
    }
}
