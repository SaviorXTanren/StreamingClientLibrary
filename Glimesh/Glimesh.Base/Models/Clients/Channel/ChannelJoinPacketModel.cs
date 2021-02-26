using Glimesh.Base.Models.Channels;
using Newtonsoft.Json.Linq;

namespace Glimesh.Base.Models.Clients.Channel
{
    /// <summary>
    /// Packet sent when connecting to a specific channel's live updates.
    /// </summary>
    public class ChannelJoinPacketModel : ClientPacketModelBase
    {
        /// <summary>
        /// Creates a new instance of the ChannelJoinPacketModel class.
        /// </summary>
        /// <param name="channelID">The ID of the channel to get updates for</param>
        public ChannelJoinPacketModel(string channelID)
        {
            this.Topic = AbsintheControlTopicName;
            this.Event = DocEventName;
            this.Payload["query"] = $"subscription {{ channel(ID: {channelID}) {{ {ChannelModel.AllFields} }} }}";
            this.Payload["variables"] = new JObject();
        }
    }
}
