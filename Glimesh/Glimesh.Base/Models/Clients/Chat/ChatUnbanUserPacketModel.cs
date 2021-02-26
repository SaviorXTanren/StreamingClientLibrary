using Newtonsoft.Json.Linq;

namespace Glimesh.Base.Models.Clients.Chat
{
    /// <summary>
    /// Packet for unbanning a user.
    /// </summary>
    public class ChatUnbanUserPacketModel : ClientPacketModelBase
    {
        private const string UnbanUserMutationPayload = "mutation {{ unbanUser(channelId: {0}, userId: {1}) {{ action, moderator {{ displayname }} }} }}";

        /// <summary>
        /// Creates a new instance of the ChatUnbanUserPacketModel class.
        /// </summary>
        /// <param name="channelID">The ID of the channel to send the message to</param>
        /// <param name="userID">The ID of the user to unban</param>
        public ChatUnbanUserPacketModel(string channelID, string userID)
            : base()
        {
            this.Topic = AbsintheControlTopicName;
            this.Event = DocEventName;
            this.Payload["query"] = string.Format(UnbanUserMutationPayload, channelID, userID);
            this.Payload["variables"] = new JObject();
        }
    }
}
