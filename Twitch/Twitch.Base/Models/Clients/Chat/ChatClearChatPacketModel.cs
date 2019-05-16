using StreamingClient.Base.Util;
using System.Linq;

namespace Twitch.Base.Models.Clients.Chat
{
    /// <summary>
    /// Information about a chat clear packet.
    /// </summary>
    public class ChatClearChatPacketModel : ChatPacketModelBase
    {
        /// <summary>
        /// The ID of the command for a chat clear.
        /// </summary>
        public const string CommandID = "ClEARCHAT";

        /// <summary>
        /// The user's login name who was purged, if any.
        /// </summary>
        public string UserLogin { get; set; }

        /// <summary>
        /// The time of the user in seconds. A value of 0 is a permanent ban.
        /// </summary>
        public long BanDuration { get; set; } = 0;

        /// <summary>
        /// Creates a new instance of the ChatClearChatPacketModel class.
        /// </summary>
        /// <param name="packet">The Chat packet</param>
        public ChatClearChatPacketModel(ChatRawPacketModel packet)
            : base(packet)
        {
            if (packet.Parameters.Count > 1)
            {
                this.UserLogin = packet.Parameters.Last();
            }
            this.BanDuration = packet.GetTagLong("ban-duration");
        }
    }
}
