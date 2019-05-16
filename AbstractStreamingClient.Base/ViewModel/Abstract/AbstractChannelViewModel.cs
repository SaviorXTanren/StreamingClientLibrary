using AbstractStreamingClient.Base;

namespace StreamingClient.Base.ViewModel.Abstract
{
    /// <summary>
    /// Interface of a channel on a streaming service.
    /// </summary>
    public abstract class AbstractChannelViewModel
    {
        /// <summary>
        /// The platform of the channel.
        /// </summary>
        public abstract StreamingPlatformType Platform { get; }
        /// <summary>
        /// The ID of the channel.
        /// </summary>
        public abstract string ID { get; }
        /// <summary>
        /// The name of the channel.
        /// </summary>
        public abstract string Name { get; }
        /// <summary>
        /// The title of the channel.
        /// </summary>
        public abstract string Title { get; set; }
        /// <summary>
        /// The URL of the channel.
        /// </summary>
        public abstract string URL { get; }
    }
}
