namespace Glimesh.Base.Models.Clients.Chat
{
    /// <summary>
    /// Heartbeat packet sent to keep chat connection open.
    /// </summary>
    public class ChatHeartbeatPacketModel : ChatPacketModelBase
    {
        /// <summary>
        /// Topic sent for heartbeat chat connection.
        /// </summary>
        public const string HeartbeatTopicName = "phoenix";

        /// <summary>
        /// Creates a new instance of the ChatHeartbeatPacketModel class.
        /// </summary>
        public ChatHeartbeatPacketModel()
        {
            this.Topic = HeartbeatTopicName;
            this.Event = "heartbeat";
        }
    }
}
