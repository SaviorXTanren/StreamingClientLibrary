using Mixer.Base.Model.Channel;

namespace Mixer.Base.Model.User
{
    public class UserWithChannelModel : UserModel
    {
        public ChannelModel channel { get; set; }
    }
}
