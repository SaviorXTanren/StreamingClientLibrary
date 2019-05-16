using StreamingClient.Base.Util;
using System.Collections.Generic;
using System.Threading.Tasks;
using Twitch.Base.Models.V5.Channel;
using Twitch.Base.Models.V5.Chat;

namespace Twitch.Base.Services.V5API
{
    /// <summary>
    /// The APIs for Chat-based services.
    /// </summary>
    public class ChatService : V5APIServiceBase
    {
        /// <summary>
        /// Creates an instance of the ChatService.
        /// </summary>
        /// <param name="connection">The Twitch connection to use</param>
        public ChatService(TwitchConnection connection) : base(connection) { }

        /// <summary>
        /// Gets the chat badges for a channel.
        /// </summary>
        /// <param name="channel">The channel to get chat badges for</param>
        /// <returns>The chat badges</returns>
        public async Task<ChannelChatBadgesModel> GetChannelChatBadges(ChannelModel channel)
        {
            Validator.ValidateVariable(channel, "channel");
            return await this.GetAsync<ChannelChatBadgesModel>("chat/" + channel.id + "/badges");
        }

        /// <summary>
        /// Gets all chat emoticons and their details.
        /// </summary>
        /// <returns>The chat emoticons</returns>
        public async Task<IEnumerable<ChatRoomModel>> GetChatRoomsForChannel(ChannelModel channel)
        {
            Validator.ValidateVariable(channel, "channel");
            return await this.GetNamedArrayAsync<ChatRoomModel>("chat/" + channel.id + "/rooms", "rooms");
        }
    }
}
