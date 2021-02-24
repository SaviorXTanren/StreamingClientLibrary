using Newtonsoft.Json.Linq;

namespace Glimesh.Base.Models.Clients.Chat
{
    /// <summary>
    /// Packet sent when connecting to a specific channel's chat.
    /// </summary>
    public class ChatJoinPacketModel : ChatPacketModelBase
    {
        /// <summary>
        /// Event sent when connecting to specific channel's chat.
        /// </summary>
        public const string ConnectEventName = "doc";

        private const string SubscriptionQueryPayload = "subscription {{ chatMessage(channelId: {0}) {{ channel {{ id }} user {{ id username displayname avatar }} message }} }}";

        /// <summary>
        /// Creates a new instance of the ChatConnectPacketModel class.
        /// </summary>
        /// <param name="channelID">The ID of the channel to connect to</param>
        public ChatJoinPacketModel(string channelID)
        {
            this.Topic = AbsintheControlTopicName;
            this.Event = ConnectEventName;
            this.Payload["query"] = string.Format(SubscriptionQueryPayload, channelID);
            this.Payload["variables"] = new JObject();
        }
    }
}
