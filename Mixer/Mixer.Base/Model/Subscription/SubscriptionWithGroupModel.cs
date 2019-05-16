using Mixer.Base.Model.User;

namespace Mixer.Base.Model.Subscription
{
    public class SubscriptionWithGroupModel : SubscriptionModel
    {
        public uint group { get; set; }
        public UserGroupModel Group { get; set; }
    }
}
