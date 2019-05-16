using Mixer.Base.Model.Channel;

namespace Mixer.Base.Model.User
{
    /// <summary>
    /// Private information for a user.
    /// </summary>
    public class PrivatePopulatedUserModel : PrivateUserModel
    {
        /// <summary>
        /// The user's channel information.
        /// </summary>
        public ChannelModel channel { get; set; }
        /// <summary>
        /// The groups that the user is a member of.
        /// </summary>
        public UserGroupModel[] groups { get; set; }
        /// <summary>
        /// The preferences for the user.s
        /// </summary>
        public UserPreferencesModel preferences { get; set; }
    }
}
