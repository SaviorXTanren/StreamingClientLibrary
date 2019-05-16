using AbstractStreamingClient.Base;

namespace StreamingClient.Base.ViewModel.Abstract
{
    /// <summary>
    /// Interface of a game on a streaming service.
    /// </summary>
    public abstract class AbstractGameViewModel
    {
        /// <summary>
        /// The platform of the game.
        /// </summary>
        public abstract StreamingPlatformType Platform { get; }
        /// <summary>
        /// The ID of the game.
        /// </summary>
        public abstract string ID { get; }
        /// <summary>
        /// The name of the game.
        /// </summary>
        public abstract string Name { get; }
    }
}
