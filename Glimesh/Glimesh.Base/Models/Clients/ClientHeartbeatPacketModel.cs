namespace Glimesh.Base.Models.Clients
{
    /// <summary>
    /// Heartbeat packet sent to keep the web socket connection open.
    /// </summary>
    public class ClientHeartbeatPacketModel : ClientPacketModelBase
    {
        /// <summary>
        /// Topic sent for heartbeat chat connection.
        /// </summary>
        public const string HeartbeatTopicName = "phoenix";

        /// <summary>
        /// Creates a new instance of the ClientHeartbeatPacketModel class.
        /// </summary>
        public ClientHeartbeatPacketModel()
        {
            this.Topic = HeartbeatTopicName;
            this.Event = "heartbeat";
        }
    }
}
