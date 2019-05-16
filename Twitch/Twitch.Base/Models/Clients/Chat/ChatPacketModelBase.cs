namespace Twitch.Base.Models.Clients.Chat
{
    /// <summary>
    /// Information about a base chat packet.
    /// </summary>
    public class ChatPacketModelBase
    {
        /// <summary>
        /// The raw packet information.
        /// </summary>
        public ChatRawPacketModel RawPacket { get; set; }

        /// <summary>
        /// Creates a new instance of the ChatPacketModelBase class.
        /// </summary>
        public ChatPacketModelBase() { }

        /// <summary>
        /// Creates a new instance of the ChatPacketModelBase class.
        /// </summary>
        /// <param name="rawPacket">The raw Chat packet</param>
        public ChatPacketModelBase(ChatRawPacketModel rawPacket)
        {
            this.RawPacket = rawPacket;
        }
    }
}
