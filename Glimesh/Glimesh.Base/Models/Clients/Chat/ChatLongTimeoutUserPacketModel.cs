using Newtonsoft.Json.Linq;

namespace Glimesh.Base.Models.Clients.Chat
{
    /// <summary>
    /// Packet for timing out a user.
    /// </summary>
    public class ChatLongTimeoutUserPacketModel : ClientPacketModelBase
    {
        private const string LongTimeoutUserMutationPayload = "mutation {{ longTimeoutUser(channelId: {0}, userId: {1}) {{ action, moderator {{ displayname }} }} }}";

        /// <summary>
        /// Creates a new instance of the ChatLongTimeoutPacketModel class.
        /// </summary>
        /// <param name="channelID">The ID of the channel to send the message to</param>
        /// <param name="userID">The ID of the user to time out</param>
        public ChatLongTimeoutUserPacketModel(string channelID, string userID)
            : base()
        {
            this.Topic = AbsintheControlTopicName;
            this.Event = DocEventName;
            this.Payload["query"] = string.Format(LongTimeoutUserMutationPayload, channelID, userID);
            this.Payload["variables"] = new JObject();
        }
    }
}
