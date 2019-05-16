namespace Mixer.Base.Model.Client
{
    /// <summary>
    /// Basic properties on a Mixer web socket packet.
    /// </summary>
    public class WebSocketPacket
    {
        /// <summary>
        /// The type of packet.
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// The ID of the packet.
        /// </summary>
        public uint id { get; set; }
    }
}
