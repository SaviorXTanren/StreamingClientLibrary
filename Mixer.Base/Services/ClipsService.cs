using Mixer.Base.Model.Broadcast;
using Mixer.Base.Model.Channel;
using Mixer.Base.Model.Clips;
using Mixer.Base.Util;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Mixer.Base.Services
{
    /// <summary>
    /// The APIs for clips-based services.
    /// </summary>
    public class ClipsService : MixerServiceBase
    {
        /// <summary>
        /// Creates an instance of the ClipsService.
        /// </summary>
        /// <param name="connection">The Mixer connection to use</param>
        public ClipsService(MixerConnection connection) : base(connection) { }

        /// <summary>
        /// Gets whether a clip can be created for the specified broadcast.
        /// </summary>
        /// <param name="broadcast">The broadcast to validate</param>
        /// <returns>Whether a clip can be created for the specified broadcast</returns>
        public async Task<bool> CanClipBeMade(BroadcastModel broadcast)
        {
            Validator.ValidateVariable(broadcast, "broadcast");

            HttpResponseMessage response = await this.GetAsync("clips/broadcasts/" + broadcast.id.ToString() + "/canClip");
            return (response.StatusCode == HttpStatusCode.OK);
        }

        /// <summary>
        /// Creates a clip with the specified request data.
        /// </summary>
        /// <param name="clipRequest">The request data for the clip</param>
        /// <returns>The clip information if the request was processed successfully</returns>
        public async Task<ClipModel> CreateClip(ClipRequestModel clipRequest)
        {
            Validator.ValidateVariable(clipRequest, "clipRequest");

            return await this.PostAsync<ClipModel>("clips/create", this.CreateContentFromObject(clipRequest));
        }

        /// <summary>
        /// Gets the clip with the specified shareable ID.
        /// </summary>
        /// <param name="shareableClipID">The shareable ID of the clip</param>
        /// <returns>The clip with the specified shareable ID</returns>
        public async Task<ClipModel> GetClip(string shareableClipID)
        {
            Validator.ValidateString(shareableClipID, "shareableClipID");

            return await this.GetAsync<ClipModel>("clips/" + shareableClipID);
        }

        /// <summary>
        /// Gets all clips for the specified Channel.
        /// </summary>
        /// <param name="channel">The channel to get clips for</param>
        /// <returns>All clips for the specified Channel</returns>
        public async Task<IEnumerable<ClipModel>> GetChannelClips(ChannelModel channel)
        {
            Validator.ValidateVariable(channel, "channel");

            return await this.GetAsync<List<ClipModel>>("clips/channels/" + channel.id.ToString());
        }

        /// <summary>
        /// Changes the title of the clip with the specified shareable ID.
        /// </summary>
        /// <param name="shareableClipID">The shareable ID of the clip</param>
        /// <param name="title">The new title for the clip</param>
        /// <returns>Whether the clip's title was changed</returns>
        public async Task<bool> ChangeClipTitle(string shareableClipID, string title)
        {
            Validator.ValidateString(shareableClipID, "shareableClipID");
            Validator.ValidateString(title, "title");

            HttpResponseMessage response = await this.PostAsync("clips/" + shareableClipID + "/metadata", this.CreateContentFromString(title));
            return (response.StatusCode == HttpStatusCode.OK);
        }

        /// <summary>
        /// Deletes the clip with the specified shareable ID.
        /// </summary>
        /// <param name="shareableClipID">The shareable ID of the clip</param>
        /// <returns>Whether the clip was deleted</returns>
        public async Task<bool> DeleteClip(string shareableClipID)
        {
            Validator.ValidateString(shareableClipID, "shareableClipID");

            return await this.DeleteAsync("clips/" + shareableClipID);
        }
    }
}
