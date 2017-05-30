using Mixer.Base.Model.Channel;
using Mixer.Base.Model.User;
using Mixer.Base.Util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mixer.Base.Services
{
    public class ChatsService : ServiceBase
    {
        public ChatsService(MixerClient client) : base(client) { }

        public async Task<ChannelChatModel> GetChat(ChannelModel channel)
        {
            Validator.ValidateVariable(channel, "channel");
            return await this.GetAsync<ChannelChatModel>("chats/" + channel.id);
        }

        public async Task<IEnumerable<ChatUserModel>> GetUsers(ChannelModel channel)
        {
            Validator.ValidateVariable(channel, "channel");
            return await this.GetPagedAsync<ChatUserModel>("chats/" + channel.id + "/users");
        }
    }
}
