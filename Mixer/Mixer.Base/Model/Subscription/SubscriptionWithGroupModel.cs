using Mixer.Base.Model.User;

namespace Mixer.Base.Model.Subscription
{
    /// <summary>
    /// A subscription with added group information as a nested object.
    /// </summary>
    public class SubscriptionWithGroupModel : SubscriptionModel
    {
        /// <summary>
        /// The group ID of the group the subscription grants the user access to.
        /// </summary>
        public uint group { get; set; }
        /// <summary>
        /// The embedded group assigned to the user.
        /// </summary>
        public UserGroupModel Group { get; set; }
    }
}
