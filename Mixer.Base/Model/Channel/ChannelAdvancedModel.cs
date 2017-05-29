using Mixer.Base.Model.Game;
using Mixer.Base.Model.User;

namespace Mixer.Base.Model.Channel
{
    public class ChannelAdvancedModel : ChannelModel
    {
        public GameTypeModel type { get; set; }
        public UserModel user { get; set; }
    }
}
