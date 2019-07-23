namespace Mixer.Base.Model.Channel
{
    /// <summary>
    /// Chat connection information for a channel.
    /// </summary>
    public class ChannelChatModel
    {
        /// <summary>
        /// List of chat server websocket urls.
        /// </summary>
        public string[] endpoints { get; set; }
        /// <summary>
        /// The auth key required to join a channel's chatroom.
        /// </summary>
        public string authkey { get; set; }
        /// <summary>
        /// List of roles he user will have once joined.
        /// </summary>
        public string[] roles { get; set; }
        /// <summary>
        /// List of permissions the user will have once joined.
        /// </summary>
        public string[] permissions { get; set; }
        /// <summary>
        /// Whether the channel is in load shedding mode.
        /// </summary>
        public bool? isLoadShed { get; set; }
    }
}
