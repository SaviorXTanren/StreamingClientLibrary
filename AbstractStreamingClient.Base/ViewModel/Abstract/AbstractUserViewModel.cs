using AbstractStreamingClient.Base;

namespace StreamingClient.Base.ViewModel.Abstract
{
    /// <summary>
    /// Interface of a user on a streaming service.
    /// </summary>
    public abstract class AbstractUserViewModel
    {
        /// <summary>
        /// The platform of the user.
        /// </summary>
        public abstract StreamingPlatformType Platform { get; }
        /// <summary>
        /// The ID of the user.
        /// </summary>
        public abstract string ID { get; }
        /// <summary>
        /// The login name of the user.
        /// </summary>
        public abstract string UserName { get; }
        /// <summary>
        /// The display name of the user.
        /// </summary>
        public abstract string DisplayName { get; }
        /// <summary>
        /// The display image URL of the user.
        /// </summary>
        public abstract string DisplayImage { get; }
    }
}
