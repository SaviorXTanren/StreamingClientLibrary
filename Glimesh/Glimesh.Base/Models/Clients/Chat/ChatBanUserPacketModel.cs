using Newtonsoft.Json.Linq;

namespace Glimesh.Base.Models.Clients.Chat
{
    /// <summary>
    /// Packet for banning a user.
    /// </summary>
    public class ChatBanUserPacketModel : ChatPacketModelBase
    {
        private const string BanUserMutationPayload = "mutation {{ banUser(channelId: {0}, userId: {1}) {{ action, moderator {{ displayname }} }} }}";

        /// <summary>
        /// Creates a new instance of the ChatBanUserPacketModel class.
        /// </summary>
        /// <param name="channelID">The ID of the channel to send the message to</param>
        /// <param name="userID">The ID of the user to ban</param>
        public ChatBanUserPacketModel(string channelID, string userID)
            : base()
        {
            this.Topic = AbsintheControlTopicName;
            this.Event = DocEventName;
            this.Payload["query"] = string.Format(BanUserMutationPayload, channelID, userID);
            this.Payload["variables"] = new JObject();
        }
    }
}
