using Google.Apis.YouTube.v3.Data;
using StreamingClient.Base.ViewModel.Abstract;

namespace AbstractStreamingClient.Base.ViewModel.Abstract.YouTubeLive
{
    public class AbstractYouTubeLiveUserViewModel : AbstractUserViewModel
    {
        /// <summary>
        /// The underlying YouTube Live user.
        /// </summary>
        public Channel User { get; set; }

        /// <summary>
        /// Creates a new instance of the AbstractTwitchUserViewModel class.
        /// </summary>
        /// <param name="user">The YouTube Live user</param>
        public AbstractYouTubeLiveUserViewModel(Channel user)
        {
            this.User = user;
        }

        /// <summary>
        /// The platform of the user.
        /// </summary>
        public override StreamingPlatformType Platform { get { return StreamingPlatformType.YouTubeLive; } }
        /// <summary>
        /// The ID of the user.
        /// </summary>
        public override string ID { get { return this.User.Id; } }
        /// <summary>
        /// The login name of the user.
        /// </summary>
        public override string UserName { get { return this.User.Snippet.Title.ToString(); } }
        /// <summary>
        /// The display name of the user.
        /// </summary>
        public override string DisplayName { get { return this.UserName; } }
        /// <summary>
        /// The display image URL of the user.
        /// </summary>
        public override string DisplayImage { get { return this.User.Snippet.Thumbnails.Default__.Url; } }
    }
}
