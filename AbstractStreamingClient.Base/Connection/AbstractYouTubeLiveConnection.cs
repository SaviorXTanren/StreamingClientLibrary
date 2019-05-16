using AbstractStreamingClient.Base.ViewModel.Abstract.YouTubeLive;
using Google.Apis.YouTube.v3.Data;
using StreamingClient.Base.ViewModel.Abstract;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YouTubeLive.Base;
using static Google.Apis.YouTube.v3.LiveBroadcastsResource.ListRequest;

namespace AbstractStreamingClient.Base.Connection
{
    /// <summary>
    /// Abstract implementation of YouTube Live connection.
    /// </summary>
    public class AbstractYouTubeLiveConnection : AbstractStreamingServiceConnectionBase
    {
        /// <summary>
        /// The underlying YouTube Live connection.
        /// </summary>
        public YouTubeLiveConnection Connection { get; private set; }

        /// <summary>
        /// Creates a new instance of the AbstractYouTubeLiveConnection class.
        /// </summary>
        /// <param name="connection">The underlying YouTube Live connection</param>
        public AbstractYouTubeLiveConnection(YouTubeLiveConnection connection)
        {
            this.Connection = connection;
        }

        /// <summary>
        /// The type of streaming service.
        /// </summary>
        public override StreamingPlatformType Type { get { return StreamingPlatformType.YouTubeLive; } }

        /// <summary>
        /// Gets the currently authenticated user.
        /// </summary>
        /// <returns>The currently authenticated user</returns>
        public override async Task<AbstractUserViewModel> GetCurrentUser()
        {
            Channel user = await this.Connection.Channels.GetMyChannel();
            return (user != null) ? new AbstractYouTubeLiveUserViewModel(user) : null;
        }

        /// <summary>
        /// Gets the user by their ID.
        /// </summary>
        /// <param name="id">The ID of the user</param>
        /// <returns>The matching user</returns>
        public override async Task<AbstractUserViewModel> GetUserByID(string id)
        {
            Channel user = await this.Connection.Channels.GetChannelByID(id);
            return (user != null) ? new AbstractYouTubeLiveUserViewModel(user) : null;
        }

        /// <summary>
        /// Gets the user by their name.
        /// </summary>
        /// <param name="name">The name of the user</param>
        /// <returns>The matching user</returns>
        public override async Task<AbstractUserViewModel> GetUserByName(string name)
        {
            Channel user = await this.Connection.Channels.GetChannelByUsername(name);
            return (user != null) ? new AbstractYouTubeLiveUserViewModel(user) : null;
        }

        /// <summary>
        /// Gets the currently authenticated channel.
        /// </summary>
        /// <returns>The currently authenticated channel</returns>
        public override async Task<AbstractChannelViewModel> GetCurrentChannel()
        {
            Channel channel = await this.Connection.Channels.GetMyChannel();
            IEnumerable<LiveBroadcast> broadcasts = await this.Connection.LiveBroadcasts.GetMyBroadcasts(BroadcastStatusEnum.Active);
            return (channel != null) ? new AbstractYouTubeLiveChannelViewModel(channel, broadcasts.FirstOrDefault()) : null;
        }

        /// <summary>
        /// Updates the channel.
        /// </summary>
        /// <param name="channel">The channel to update</param>
        /// <returns>The updated channel</returns>
        public override Task<AbstractChannelViewModel> UpdateCurrentChannel(AbstractChannelViewModel channel)
        {
            return null;
        }

        /// <summary>
        /// Gets the channel by their ID.
        /// </summary>
        /// <param name="id">The ID of the channel</param>
        /// <returns>The matching channel</returns>
        public override async Task<AbstractChannelViewModel> GetChannelByID(string id)
        {
            Channel channel = await this.Connection.Channels.GetChannelByID(id);
            return (channel != null) ? new AbstractYouTubeLiveChannelViewModel(channel, null) : null;
        }

        /// <summary>
        /// Gets the game by their ID.
        /// </summary>
        /// <param name="id">The ID of the game</param>
        /// <returns>The matching game</returns>
        public override Task<AbstractGameViewModel> GetGameByID(string id)
        {
            return null;
        }
    }
}
