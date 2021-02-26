using Glimesh.Base.Models.Channels;
using Newtonsoft.Json.Linq;

namespace Glimesh.Base.Models.Clients.Channel
{
    /// <summary>
    /// Packet received when an update has occurred in a channel.
    /// </summary>
    public class ChannelUpdatePacketModel : ClientResponsePacketModel
    {
        /// <summary>
        /// The updated channel data.
        /// </summary>
        public ChannelModel Channel { get; set; }

        /// <summary>
        /// Creates a new instance of the ChannelUpdatePacketModel class.
        /// </summary>
        /// <param name="serializedChatPacketArray">The serialized packet array</param>
        public ChannelUpdatePacketModel(string serializedChatPacketArray)
            : base(serializedChatPacketArray)
        {
            JObject channel = (JObject)this.Payload.SelectToken("result.data.channel");
            if (channel != null)
            {
                this.Channel = channel.ToObject<ChannelModel>();
            }
        }
    }
}
