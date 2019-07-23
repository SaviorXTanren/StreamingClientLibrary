namespace Mixer.Base.Model.Channel
{
    /// <summary>
    /// Represents a follow relationship between a user and a channel.
    /// </summary>
    public class FollowModel : TimeStampedModel
    {
        /// <summary>
        /// The follower user id.
        /// </summary>
        public uint user { get; set; }
        /// <summary>
        /// The followee channel id.
        /// </summary>
        public uint channel { get; set; }
    }
}
