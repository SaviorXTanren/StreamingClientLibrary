using Mixer.Base.Model.Channel;
using Mixer.Base.Model.User;
using Mixer.Base.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mixer.Base.Services
{
    public class ChatsService : ServiceBase
    {
        public ChatsService(MixerClient client) : base(client) { }

        public async Task<IEnumerable<string>> GetChat(ChannelModel channel)
        {
            Validator.ValidateVariable(channel, "channel");
            JObject obj = await this.GetJObjectAsync("chats/" + channel.id);
            return JsonConvert.DeserializeObject<string[]>(obj["endpoints"].ToString());
        }

        public async Task<IEnumerable<ChatUserModel>> GetUsers(ChannelModel channel)
        {
            Validator.ValidateVariable(channel, "channel");
            return await this.GetPagedAsync<ChatUserModel>("chats/" + channel.id + "/users");
        }
    }
}
