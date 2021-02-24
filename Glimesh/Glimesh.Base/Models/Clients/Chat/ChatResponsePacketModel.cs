namespace Glimesh.Base.Models.Clients.Chat
{
    /// <summary>
    /// A response chat packet received from the server.
    /// </summary>
    public class ChatResponsePacketModel : ChatPacketModelBase
    {
        /// <summary>
        /// Creates a new instance of the ChatResponsePacketModel class.
        /// </summary>
        /// <param name="packet">The text packet data</param>
        public ChatResponsePacketModel(string packet) : base(packet) { }
    }
}
