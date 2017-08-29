using Mixer.Base.Model.User;

namespace Mixer.Base.Model.Interactive
{
    public class InteractiveGameListingModel : InteractiveGameModel
    {
        public InteractiveGameVersionModel[] versions { get; set; }
        public UserModel owner { get; set; }
    }
}
