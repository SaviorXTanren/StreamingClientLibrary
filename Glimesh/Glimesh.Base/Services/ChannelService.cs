using Glimesh.Base.Models.Channels;
using Glimesh.Base.Models.GraphQL;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Glimesh.Base.Services
{
    /// <summary>
    /// The APIs for Channel-based services.
    /// </summary>
    public class ChannelService : GlimeshServiceBase
    {
        /// <summary>
        /// Creates an instance of the ChannelService.
        /// </summary>
        /// <param name="connection">The Glimesh connection to use</param>
        public ChannelService(GlimeshConnection connection) : base(connection) { }

        /// <summary>
        /// Gets the channels currently live on the homepage
        /// </summary>
        /// <returns>All available channels</returns>
        public async Task<IEnumerable<ChannelModel>> GetHomepageChannels(int count = 1)
        {
            GraphQLEdgeArrayModel<ChannelModel> channels = await this.QueryAsync<GraphQLEdgeArrayModel<ChannelModel>>($"{{ homepageChannels(first: {count}) {{ {ChannelModel.BasicFieldsWithStreamerEdges} }} }}", "homepageChannels");
            return channels?.Items;
        }

        /// <summary>
        /// Gets all of the available live channels
        /// </summary>
        /// <param name="categorySlug">The optional category slug to search for</param>
        /// <param name="count">The number of channels to return</param>
        /// <returns>All available channels</returns>
        public async Task<IEnumerable<ChannelModel>> GetLiveChannels(string categorySlug = null, int count = 1)
        {
            string query = $"{{ channels(status: LIVE, first: {count}) {{ {ChannelModel.BasicFieldsWithStreamerEdges} }} }}";
            if (!string.IsNullOrEmpty(categorySlug))
            {
                query = $"{{ channels(categorySlug: \"{categorySlug}\", status: LIVE, first: {count}) {{ {ChannelModel.BasicFieldsWithStreamerEdges} }} }}";
            }

            GraphQLEdgeArrayModel<ChannelModel> channels = await this.QueryAsync<GraphQLEdgeArrayModel<ChannelModel>>(query, "channels");
            return channels?.Items;
        }

        /// <summary>
        /// Gets the channel with the specified id.
        /// </summary>
        /// <param name="id">The id of the channel</param>
        /// <returns>The category</returns>
        public async Task<ChannelModel> GetChannelByID(string id) { return await this.QueryAsync<ChannelModel>($"{{ channel(id: \"{id}\") {{ {ChannelModel.AllFields} }} }}", "channel"); }

        /// <summary>
        /// Gets the channel with the specified id.
        /// </summary>
        /// <param name="name">The name of the channel</param>
        /// <returns>The channel</returns>
        public async Task<ChannelModel> GetChannelByName(string name) { return await this.QueryAsync<ChannelModel>($"{{ channel(streamerUsername: \"{name}\") {{ {ChannelModel.AllFields} }} }}", "channel"); }
    }
}