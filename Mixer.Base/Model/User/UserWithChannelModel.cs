using Mixer.Base.Model.Channel;

namespace Mixer.Base.Model.User
{
    /// <summary>
    /// A user and their channel information.
    /// </summary>
    public class UserWithChannelModel : UserModel
    {
        /// <summary>
        /// The channel of the user.
        /// </summary>
        public ChannelModel channel { get; set; }
        /// <summary>
        /// The groups of the user.
        /// </summary>
        public UserGroupModel[] groups { get; set; }
    }
}
