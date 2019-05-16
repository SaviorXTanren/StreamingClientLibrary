using AbstractStreamingClient.Base.ViewModel.Abstract.Mixer;
using Mixer.Base;
using Mixer.Base.Model.Channel;
using Mixer.Base.Model.Game;
using Mixer.Base.Model.User;
using StreamingClient.Base.ViewModel.Abstract;
using System.Threading.Tasks;

namespace AbstractStreamingClient.Base.Connection
{
    /// <summary>
    /// Abstract implementation of Mixer connection.
    /// </summary>
    public class AbstractMixerConnection : AbstractStreamingServiceConnectionBase
    {
        /// <summary>
        /// The underlying Mixer connection.
        /// </summary>
        public MixerConnection Connection { get; private set; }

        /// <summary>
        /// Creates a new instance of the AbstractMixerConnection class.
        /// </summary>
        /// <param name="connection">The underlying Mixer connection</param>
        public AbstractMixerConnection(MixerConnection connection)
        {
            this.Connection = connection;
        }

        /// <summary>
        /// The type of streaming service.
        /// </summary>
        public override StreamingPlatformType Type { get { return StreamingPlatformType.Mixer; } }

        /// <summary>
        /// Gets the currently authenticated user.
        /// </summary>
        /// <returns>The currently authenticated user</returns>
        public override async Task<AbstractUserViewModel> GetCurrentUser()
        {
            UserModel user = await this.Connection.Users.GetCurrentUser();
            return (user != null) ? new AbstractMixerUserViewModel(user) : null;
        }

        /// <summary>
        /// Gets the user by their ID.
        /// </summary>
        /// <param name="id">The ID of the user</param>
        /// <returns>The matching user</returns>
        public override async Task<AbstractUserViewModel> GetUserByID(string id)
        {
            if (uint.TryParse(id, out uint uintID))
            {
                UserModel user = await this.Connection.Users.GetUser(uintID);
                return (user != null) ? new AbstractMixerUserViewModel(user) : null;
            }
            return null;
        }

        /// <summary>
        /// Gets the user by their name.
        /// </summary>
        /// <param name="name">The name of the user</param>
        /// <returns>The matching user</returns>
        public override async Task<AbstractUserViewModel> GetUserByName(string name)
        {
            UserModel user = await this.Connection.Users.GetUser(name);
            return (user != null) ? new AbstractMixerUserViewModel(user) : null;
        }

        /// <summary>
        /// Gets the currently authenticated channel.
        /// </summary>
        /// <returns>The currently authenticated channel</returns>
        public override async Task<AbstractChannelViewModel> GetCurrentChannel()
        {
            PrivatePopulatedUserModel user = await this.Connection.Users.GetCurrentUser();
            return (user != null) ? new AbstractMixerChannelViewModel(user.channel) : null;
        }

        /// <summary>
        /// Updates the channel.
        /// </summary>
        /// <param name="channel">The channel to update</param>
        /// <returns>The updated channel</returns>
        public override async Task<AbstractChannelViewModel> UpdateCurrentChannel(AbstractChannelViewModel channel)
        {
            if (channel is AbstractMixerChannelViewModel)
            {
                AbstractMixerChannelViewModel mixerChannel = (AbstractMixerChannelViewModel)channel;
                ChannelModel updatedChannel = await this.Connection.Channels.UpdateChannel(mixerChannel.Channel);
                return (updatedChannel != null) ? new AbstractMixerChannelViewModel(updatedChannel) : null;
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
            if (uint.TryParse(id, out uint uintID))
            {
                ChannelModel channel = await this.Connection.Channels.GetChannel(uintID);
                return (channel != null) ? new AbstractMixerChannelViewModel(channel) : null;
            }
            return null;
        }

        /// <summary>
        /// Gets the game by their ID.
        /// </summary>
        /// <param name="id">The ID of the game</param>
        /// <returns>The matching game</returns>
        public override async Task<AbstractGameViewModel> GetGameByID(string id)
        {
            if (uint.TryParse(id, out uint uintID))
            {
                GameTypeModel game = await this.Connection.GameTypes.GetGameType(uintID);
                return (game != null) ? new AbstractMixerGameViewModel(game) : null;
            }
            return null;
        }
    }
}
