namespace Glimesh.Base.Models.Clients.Chat
{
    /// <summary>
    /// Packet sent when joining to chat.
    /// </summary>
    public class ChatConnectPacketModel : ChatPacketModelBase
    {
        /// <summary>
        /// Event sent when joining to chat.
        /// </summary>
        public const string JoinEventName = "phx_join";

        /// <summary>
        /// Creates a new instance of the ChatJoinPacketModel class.
        /// </summary>
        public ChatConnectPacketModel()
        {
            this.Topic = AbsintheControlTopicName;
            this.Event = JoinEventName;
        }
    }
}
