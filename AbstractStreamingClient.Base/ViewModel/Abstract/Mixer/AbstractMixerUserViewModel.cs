using Mixer.Base.Model.User;
using StreamingClient.Base.ViewModel.Abstract;

namespace AbstractStreamingClient.Base.ViewModel.Abstract.Mixer
{
    public class AbstractMixerUserViewModel : AbstractUserViewModel
    {
        /// <summary>
        /// The underlying Mixer user.
        /// </summary>
        public UserModel User { get; set; }

        /// <summary>
        /// Creates a new instance of the AbstractMixerUserViewModel class.
        /// </summary>
        /// <param name="user">The Mixer user</param>
        public AbstractMixerUserViewModel(UserModel user)
        {
            this.User = user;
        }

        /// <summary>
        /// The platform of the user.
        /// </summary>
        public override StreamingPlatformType Platform { get { return StreamingPlatformType.Mixer; } }
        /// <summary>
        /// The ID of the user.
        /// </summary>
        public override string ID { get { return this.User.id.ToString(); } }
        /// <summary>
        /// The login name of the user.
        /// </summary>
        public override string UserName { get { return this.User.username.ToString(); } }
        /// <summary>
        /// The display name of the user.
        /// </summary>
        public override string DisplayName { get { return this.UserName; } }
        /// <summary>
        /// The display image URL of the user.
        /// </summary>
        public override string DisplayImage { get { return this.User.avatarUrl; } }
    }
}
