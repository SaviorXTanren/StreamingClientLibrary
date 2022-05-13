using Glimesh.Base.Models.Users;
using Newtonsoft.Json.Linq;

namespace Glimesh.Base.Models.Clients.Chat
{
    /// <summary>
    /// Packet sent when connecting to a specific channel's chat.
    /// </summary>
    public class ChatJoinPacketModel : ClientPacketModelBase
    {
        /// <summary>
        /// Creates a new instance of the ChatConnectPacketModel class.
        /// </summary>
        /// <param name="channelID">The ID of the channel to get chat for</param>
        public ChatJoinPacketModel(string channelID)
        {
            this.Topic = AbsintheControlTopicName;
            this.Event = DocEventName;
            this.Payload["query"] = $"subscription {{ chatMessage(channelId: {channelID}) {{ id channel {{ id }} user {{ {UserModel.BasicFields} }} message tokens {{ ...on EmoteToken {{ src, text, type }}, ...on TextToken {{ text, type }}, ...on UrlToken {{ text, type, url }} }} insertedAt is_followed_message is_subscription_message }} }}";
            this.Payload["variables"] = new JObject();
        }
    }
}
