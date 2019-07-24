namespace Mixer.Base.Model.Chat
{
    /// <summary>
    /// A chat user.
    /// </summary>
    public class ChatUserEventModel
    {
        /// <summary>
        /// The ID of the user.
        /// </summary>
        public uint id { get; set; }
        /// <summary>
        /// The account name of the user.
        /// </summary>
        public string username { get; set; }
        /// <summary>
        /// The roles of the user.
        /// </summary>
        public string[] roles { get; set; }
        /// <summary>
        /// The permissions of the user.
        /// </summary>
        public string[] permissions { get; set; }
        /// <summary>
        /// The ID of the user.
        /// </summary>
        public uint user { get { return id; } set { this.id = value; } }
    }
}
