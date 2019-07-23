namespace Mixer.Base.Model.User
{
    /// <summary>
    /// Represents a follow for a user.
    /// </summary>
    public class FollowerUserModel : UserWithChannelModel
    {
        /// <summary>
        /// The time when the follow took place.
        /// </summary>
        public TimeStampedModel followed { get; set; }
    }
}
