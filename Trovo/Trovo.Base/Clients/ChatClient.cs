using Newtonsoft.Json.Linq;
using StreamingClient.Base.Util;
using StreamingClient.Base.Web;
using System;
using System.Threading.Tasks;
using Trovo.Base.Models.Chat;

namespace Trovo.Base.Clients
{
    public class ChatClient : ClientWebSocketBase
    {
        private TrovoConnection connection;

        public ChatClient(TrovoConnection connection)
        {
            this.connection = connection;
        }

        public async Task<bool> Connect()
        {
            string token = await this.connection.Chat.GetToken();
            if (!string.IsNullOrEmpty(token))
            {
                if (await base.Connect("wss://chat.trovo.live/chat"))
                {
                    ChatMessageModel authMessage = new ChatMessageModel();
                    authMessage.type = "AUTH";
                    authMessage.nonce = Guid.NewGuid().ToString();
                    JObject jobj = new JObject();
                    jobj["token"] = token;
                    authMessage.data = JSONSerializerHelper.SerializeToString(jobj);
                    await this.Send(JSONSerializerHelper.SerializeToString(authMessage));

                    return true;
                }
            }
            return false;
        }

        public async Task Ping()
        {
            ChatMessageModel pingMessage = new ChatMessageModel();
            pingMessage.type = "PING";
            pingMessage.nonce = Guid.NewGuid().ToString();
            await this.Send(JSONSerializerHelper.SerializeToString(pingMessage));
        }

        protected override Task ProcessReceivedPacket(string packet)
        {
            Logger.Log(LogLevel.Debug, "Trovo Chat Packet: " + packet);
            return Task.FromResult(0);
        }
    }
}
