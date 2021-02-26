using Newtonsoft.Json.Linq;

namespace Glimesh.Base.Models.Clients.Chat
{
    /// <summary>
    /// Packet for timing out a user.
    /// </summary>
    public class ChatShortTimeoutUserPacketModel : ClientPacketModelBase
    {
        private const string ShortTimeoutUserMutationPayload = "mutation {{ shortTimeoutUser(channelId: {0}, userId: {1}) {{ action, moderator {{ displayname }} }} }}";

        /// <summary>
        /// Creates a new instance of the ChatShortTimeoutPacketModel class.
        /// </summary>
        /// <param name="channelID">The ID of the channel to send the message to</param>
        /// <param name="userID">The ID of the user to time out</param>
        public ChatShortTimeoutUserPacketModel(string channelID, string userID)
            : base()
        {
            this.Topic = AbsintheControlTopicName;
            this.Event = DocEventName;
            this.Payload["query"] = string.Format(ShortTimeoutUserMutationPayload, channelID, userID);
            this.Payload["variables"] = new JObject();
        }
    }
}
