using StreamingClient.Base.ViewModel.Abstract;
using System.Threading.Tasks;

namespace AbstractStreamingClient.Base
{
    /// <summary>
    /// The type of streaming platform.
    /// </summary>
    public enum StreamingPlatformType
    {
        /// <summary>
        /// Mixer
        /// </summary>
        Mixer,
        /// <summary>
        /// Twitch
        /// </summary>
        Twitch,
        /// <summary>
        /// YouTube Live
        /// </summary>
        YouTubeLive,
    }

    /// <summary>
    /// An abstracted connection to a streaming service.
    /// </summary>
    public abstract class AbstractStreamingServiceConnectionBase
    {
        /// <summary>
        /// The type of streaming service.
        /// </summary>
        public abstract StreamingPlatformType Type { get; }

        /// <summary>
        /// Gets the currently authenticated user.
        /// </summary>
        /// <returns>The currently authenticated user</returns>
        public abstract Task<AbstractUserViewModel> GetCurrentUser();

        /// <summary>
        /// Gets the user by their ID.
        /// </summary>
        /// <param name="id">The ID of the user</param>
        /// <returns>The matching user</returns>
        public abstract Task<AbstractUserViewModel> GetUserByID(string id);

        /// <summary>
        /// Gets the user by their name.
        /// </summary>
        /// <param name="name">The name of the user</param>
        /// <returns>The matching user</returns>
        public abstract Task<AbstractUserViewModel> GetUserByName(string name);

        /// <summary>
        /// Gets the currently authenticated channel.
        /// </summary>
        /// <returns>The currently authenticated channel</returns>
        public abstract Task<AbstractChannelViewModel> GetCurrentChannel();

        /// <summary>
        /// Updates the channel.
        /// </summary>
        /// <param name="channel">The channel to update</param>
        /// <returns>The updated channel</returns>
        public abstract Task<AbstractChannelViewModel> UpdateCurrentChannel(AbstractChannelViewModel channel);

        /// <summary>
        /// Gets the channel by their ID.
        /// </summary>
        /// <param name="id">The ID of the channel</param>
        /// <returns>The matching channel</returns>
        public abstract Task<AbstractChannelViewModel> GetChannelByID(string id);

        /// <summary>
        /// Gets the game by their ID.
        /// </summary>
        /// <param name="id">The ID of the game</param>
        /// <returns>The matching game</returns>
        public abstract Task<AbstractGameViewModel> GetGameByID(string id);
    }
}
