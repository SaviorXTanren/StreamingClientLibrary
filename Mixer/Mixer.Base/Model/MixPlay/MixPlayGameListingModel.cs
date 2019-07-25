using Mixer.Base.Model.User;

namespace Mixer.Base.Model.MixPlay
{
    /// <summary>
    /// The listing of a MixPlay game.
    /// </summary>
    public class MixPlayGameListingModel : MixPlayGameModel
    {
        /// <summary>
        /// The versions of the MixPlay game.
        /// </summary>
        public MixPlayGameVersionModel[] versions { get; set; }
        /// <summary>
        /// The user that owns the MixPlay game.
        /// </summary>
        public UserModel owner { get; set; }
    }
}
