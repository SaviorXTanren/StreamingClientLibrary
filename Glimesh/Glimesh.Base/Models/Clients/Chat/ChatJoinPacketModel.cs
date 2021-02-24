using Newtonsoft.Json.Linq;

namespace Glimesh.Base.Models.Clients.Chat
{
    /// <summary>
    /// Packet sent when connecting to a specific channel's chat.
    /// </summary>
    public class ChatJoinPacketModel : ChatPacketModelBase
    {
        private const string SubscriptionQueryPayload = "subscription {{ chatMessage(channelId: {0}) {{ id channel {{ id }} user {{ id username displayname avatar }} message tokens {{ ...on EmoteToken {{ src, text, type, url }}, ...on TextToken {{ text, type }}, ...on UrlToken {{ text, type, url }} }} insertedAt }} }}";

        /// <summary>
        /// Creates a new instance of the ChatConnectPacketModel class.
        /// </summary>
        /// <param name="channelID">The ID of the channel to connect to</param>
        public ChatJoinPacketModel(string channelID)
        {
            this.Topic = AbsintheControlTopicName;
            this.Event = DocEventName;
            this.Payload["query"] = string.Format(SubscriptionQueryPayload, channelID);
            this.Payload["variables"] = new JObject();
        }
    }
}
