using Mixer.Base.Model.Channel;

namespace Mixer.Base.Model.User
{
    public class PrivatePopulatedUserModel : PrivateUserModel
    {
        public ChannelModel channel { get; set; }
        public UserGroupModel[] groups { get; set; }
        public UserPreferencesModel preferences { get; set; }
    }
}
