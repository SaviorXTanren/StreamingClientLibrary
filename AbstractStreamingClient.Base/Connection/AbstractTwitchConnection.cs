using AbstractStreamingClient.Base.ViewModel.Abstract.Twitch;
using StreamingClient.Base.ViewModel.Abstract;
using System.Threading.Tasks;
using Twitch.Base;
using Twitch.Base.Models.NewAPI.Games;
using Twitch.Base.Models.V5.Channel;
using Twitch.Base.Models.V5.Users;

namespace AbstractStreamingClient.Base.Connection
{
    /// <summary>
    /// Abstract implementation of Twitch connection.
    /// </summary>
    public class AbstractTwitchConnection : AbstractStreamingServiceConnectionBase
    {
        /// <summary>
        /// The underlying Twitch connection.
        /// </summary>
        public TwitchConnection Connection { get; private set; }

        /// <summary>
        /// Creates a new instance of the AbstractTwitchConnection class.
        /// </summary>
        /// <param name="connection">The underlying Twitch connection</param>
        public AbstractTwitchConnection(TwitchConnection connection)
        {
            this.Connection = connection;
        }

        /// <summary>
        /// The type of streaming service.
        /// </summary>
        public override StreamingPlatformType Type { get { return StreamingPlatformType.Twitch; } }

        /// <summary>
        /// Gets the currently authenticated user.
        /// </summary>
        /// <returns>The currently authenticated user</returns>
        public override async Task<AbstractUserViewModel> GetCurrentUser()
        {
            UserModel user = await this.Connection.V5API.Users.GetCurrentUser();
            return (user != null) ? new AbstractTwitchUserViewModel(user) : null;
        }

        /// <summary>
        /// Gets the user by their ID.
        /// </summary>
        /// <param name="id">The ID of the user</param>
        /// <returns>The matching user</returns>
        public override async Task<AbstractUserViewModel> GetUserByID(string id)
        {
            UserModel user = await this.Connection.V5API.Users.GetUserByID(id);
            return (user != null) ? new AbstractTwitchUserViewModel(user) : null;
        }

        /// <summary>
        /// Gets the user by their name.
        /// </summary>
        /// <param name="name">The name of the user</param>
        /// <returns>The matching user</returns>
        public override async Task<AbstractUserViewModel> GetUserByName(string name)
        {
            UserModel user = await this.Connection.V5API.Users.GetUserByLogin(name);
            return (user != null) ? new AbstractTwitchUserViewModel(user) : null;
        }

        /// <summary>
        /// Gets the currently authenticated channel.
        /// </summary>
        /// <returns>The currently authenticated channel</returns>
        public override async Task<AbstractChannelViewModel> GetCurrentChannel()
        {
            ChannelModel channel = await this.Connection.V5API.Channels.GetCurrentChannel();
            return (channel != null) ? new AbstractTwitchChannelViewModel(channel) : null;
        }

        /// <summary>
        /// Updates the channel.
        /// </summary>
        /// <param name="channel">The channel to update</param>
        /// <returns>The updated channel</returns>
        public override async Task<AbstractChannelViewModel> UpdateCurrentChannel(AbstractChannelViewModel channel)
        {
            if (channel is AbstractTwitchChannelViewModel)
            {
                AbstractTwitchChannelViewModel twitchChannel = (AbstractTwitchChannelViewModel)channel;
                ChannelModel updatedChannel = await this.Connection.V5API.Channels.UpdateChannel(twitchChannel.Channel, new ChannelUpdateModel()
                {
                    status = channel.Title,
                });
                return (updatedChannel != null) ? new AbstractTwitchChannelViewModel(updatedChannel) : null;
            }
            return null;
        }

        /// <summary>
        /// Gets the channel by their ID.
        /// </summary>
        /// <param name="id">The ID of the channel</param>
        /// <returns>The matching channel</returns>
        public override async Task<AbstractChannelViewModel> GetChannelByID(string id)
        {
            ChannelModel channel = await this.Connection.V5API.Channels.GetChannelByID(id);
            return (channel != null) ? new AbstractTwitchChannelViewModel(channel) : null;
        }

        /// <summary>
        /// Gets the game by their ID.
        /// </summary>
        /// <param name="id">The ID of the game</param>
        /// <returns>The matching game</returns>
        public override async Task<AbstractGameViewModel> GetGameByID(string id)
        {
            GameModel game = await this.Connection.NewAPI.Games.GetGameByID(id);
            return (game != null) ? new AbstractTwitchGameViewModel(game) : null;
        }
    }
}
