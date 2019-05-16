namespace Mixer.Base.Model.User
{
    /// <summary>
    /// Information about a user in a channel's chat.
    /// </summary>
    public class ChatUserModel
    {
        /// <summary>
        /// The ID of the user.
        /// </summary>
        public uint? userId { get; set; }
        /// <summary>
        /// The name of the user.
        /// </summary>
        public string userName { get; set; }
        /// <summary>
        /// The roles the user has in the channel.
        /// </summary>
        public string[] userRoles { get; set; }
    }
}
