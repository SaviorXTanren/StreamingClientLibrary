using StreamingClient.Base.Util;
using StreamingClient.Base.Web;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Twitch.Base.Models.NewAPI.Tags;
using Twitch.Base.Models.NewAPI.Users;

namespace Twitch.Base.Services.NewAPI
{
    /// <summary>
    /// The APIs for Tags-based services.
    /// </summary>
    public class TagsService : NewTwitchAPIServiceBase
    {
        /// <summary>
        /// Creates an instance of the TagsService.
        /// </summary>
        /// <param name="connection">The Twitch connection to use</param>
        public TagsService(TwitchConnection connection) : base(connection) { }

        /// <summary>
        /// Gets all stream tags available.
        /// </summary>
        /// <param name="maxResults">The maximum number of results. Will be either that amount or slightly more</param>
        /// <returns>A list of tags</returns>
        public async Task<IEnumerable<TagModel>> GetStreamTags(int maxResults = 20)
        {
            return await this.GetPagedDataResultAsync<TagModel>("tags/streams", maxResults);
        }

        /// <summary>
        /// Gets all stream tags associated with a broadcaster.
        /// </summary>
        /// <param name="broadcaster">The broadcaster to get stream tags for.</param>
        /// <returns>A list of tags</returns>
        public async Task<IEnumerable<TagModel>> GetStreamTagsForBroadcaster(UserModel broadcaster)
        {
            Validator.ValidateVariable(broadcaster, "broadcaster");
            return await this.GetDataResultAsync<TagModel>("tags/streams?broadcaster_id=" + broadcaster.id);
        }

        /// <summary>
        /// Gets all stream tags matching the specified tag IDs.
        /// </summary>
        /// <param name="tagIDs">A list of tag IDs</param>
        /// <returns>A list of tags</returns>
        public async Task<IEnumerable<TagModel>> GetStreamTagsByIDs(IEnumerable<string> tagIDs)
        {
            Validator.ValidateList(tagIDs, "tagIDs");
            return await this.GetPagedDataResultAsync<TagModel>("tags/streams?tag_id=" + string.Join("&tag_id=", tagIDs), tagIDs.Count());
        }

        /// <summary>
        /// Updates the specified broadcaster's channel with the specified tags.
        /// </summary>
        /// <param name="broadcaster">The broadcaster to get stream tags for.</param>
        /// <param name="tags">The set of tags to update with</param>
        /// <returns>An awaitable Task</returns>
        public async Task UpdateStreamTags(UserModel broadcaster, IEnumerable<TagModel> tags = null)
        {
            Validator.ValidateVariable(broadcaster, "broadcaster");
            List<string> tagIDs = (tags != null) ? tags.Select(t => t.tag_id).ToList() : new List<string>();
            await this.PutAsync("streams/tags?broadcaster_id=" + broadcaster.id, AdvancedHttpClient.CreateContentFromObject(new { tag_ids = tagIDs }));
        }
    }
}
