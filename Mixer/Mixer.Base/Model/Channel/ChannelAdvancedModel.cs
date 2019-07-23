using Mixer.Base.Model.Game;
using Mixer.Base.Model.User;

namespace Mixer.Base.Model.Channel
{
    /// <summary>
    /// Augmented regular channel with additional data.
    /// </summary>
    public class ChannelAdvancedModel : ChannelModel
    {
        /// <summary>
        /// A nested type showing information about this channel's currently selected type.
        /// </summary>
        public GameTypeModel type { get; set; }
        /// <summary>
        /// This channel's owner
        /// </summary>
        public UserWithGroupsModel user { get; set; }
    }
}
