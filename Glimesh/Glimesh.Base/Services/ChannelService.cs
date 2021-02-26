using Glimesh.Base.Models.Channels;
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
        /// Gets all of the available channels
        /// </summary>
        /// <returns>All available channels</returns>
        public async Task<IEnumerable<ChannelModel>> GetAllChannels() { return await this.QueryAsync<IEnumerable<ChannelModel>>($"{{ channels {{ {ChannelModel.BasicFields} }} }}", "channels"); }

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
        public async Task<ChannelModel> GetChannelByName(string name) { return await this.QueryAsync<ChannelModel>($"{{ channel(username: \"{name}\") {{ {ChannelModel.AllFields} }} }}", "channel"); }
    }
}