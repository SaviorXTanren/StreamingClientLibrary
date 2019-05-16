using StreamingClient.Base.ViewModel.Abstract;
using Twitch.Base.Models.V5.Users;

namespace AbstractStreamingClient.Base.ViewModel.Abstract.Twitch
{
    public class AbstractTwitchUserViewModel : AbstractUserViewModel
    {
        /// <summary>
        /// The underlying Twitch user.
        /// </summary>
        public UserModel User { get; set; }

        /// <summary>
        /// Creates a new instance of the AbstractTwitchUserViewModel class.
        /// </summary>
        /// <param name="user">The Twitch user</param>
        public AbstractTwitchUserViewModel(UserModel user)
        {
            this.User = user;
        }

        /// <summary>
        /// The platform of the user.
        /// </summary>
        public override StreamingPlatformType Platform { get { return StreamingPlatformType.Twitch; } }
        /// <summary>
        /// The ID of the user.
        /// </summary>
        public override string ID { get { return this.User.id.ToString(); } }
        /// <summary>
        /// The login name of the user.
        /// </summary>
        public override string UserName { get { return this.User.name.ToString(); } }
        /// <summary>
        /// The display name of the user.
        /// </summary>
        public override string DisplayName { get { return this.User.display_name.ToString(); } }
        /// <summary>
        /// The display image URL of the user.
        /// </summary>
        public override string DisplayImage { get { return this.User.logo; } }
    }
}
