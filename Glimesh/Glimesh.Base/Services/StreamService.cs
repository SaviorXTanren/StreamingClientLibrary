using System.Threading.Tasks;

namespace Glimesh.Base.Services
{
    /// <summary>
    /// The APIs for Stream-based services.
    /// </summary>
    public class StreamService : GlimeshServiceBase
    {
        /// <summary>
        /// Creates an instance of the StreamService.
        /// </summary>
        /// <param name="connection">The Glimesh connection to use</param>
        public StreamService(GlimeshConnection connection) : base(connection) { }

        /// <summary>
        /// Updates the current stream info for the specified channel ID.
        /// </summary>
        /// <param name="channelID">The ID of the channel to update</param>
        /// <param name="title">The new title of the stream</param>
        /// <returns>An awaitable Task</returns>
        public async Task UpdateStreamInfo(string channelID, string title) { await this.MutationAsync<object>($"mutation {{ updateStreamInfo(channelId: \"{channelID}\", title: \"{title}\") {{ title, id }} }}", "updateStreamInfo"); }
    }
}
