namespace Glimesh.Base.Models.Clients
{
    /// <summary>
    /// Packet sent when connecting to the web socket server.
    /// </summary>
    public class ClientConnectPacketModel : ClientPacketModelBase
    {
        /// <summary>
        /// Event sent when joining to chat.
        /// </summary>
        public const string JoinEventName = "phx_join";

        /// <summary>
        /// Creates a new instance of the ClientConnectPacketModel class.
        /// </summary>
        public ClientConnectPacketModel()
        {
            this.Topic = AbsintheControlTopicName;
            this.Event = JoinEventName;
        }
    }
}
