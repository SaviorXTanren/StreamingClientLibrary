namespace Glimesh.Base.Models.Clients
{
    /// <summary>
    /// A response packet received from the server.
    /// </summary>
    public class ClientResponsePacketModel : ClientPacketModelBase
    {
        /// <summary>
        /// Creates a new instance of the ClientResponsePacketModel class.
        /// </summary>
        /// <param name="packet">The text packet data</param>
        public ClientResponsePacketModel(string packet) : base(packet) { }
    }
}
