using Newtonsoft.Json.Linq;

namespace Glimesh.Base.Models.Clients.Chat
{
    /// <summary>
    /// Packet for deleting a chat message.
    /// </summary>
    public class ChatDeleteMessagePacketModel : ClientPacketModelBase
    {
        private const string DeleteMessageMutationPayload = "mutation {{ deleteMessage(channelId: {0}, messageId: {1}) {{ action, moderator {{ displayname }}, user {{ displayname }} }} }}";

        /// <summary>
        /// Creates a new instance of the ChatDeleteMessagePacketModel class.
        /// </summary>
        /// <param name="channelID">The ID of the channel to delete the message from</param>
        /// <param name="messageID">The ID of the message to delete</param>
        public ChatDeleteMessagePacketModel(string channelID, string messageID)
            : base()
        {
            this.Topic = AbsintheControlTopicName;
            this.Event = DocEventName;
            this.Payload["query"] = string.Format(DeleteMessageMutationPayload, channelID, messageID);
            this.Payload["variables"] = new JObject();
        }
    }
}