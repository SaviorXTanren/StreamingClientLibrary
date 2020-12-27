using Newtonsoft.Json.Linq;
using StreamingClient.Base.Web;
using System.Threading.Tasks;

namespace Trovo.Base.Services
{
    /// <summary>
    /// The APIs for chat-based services.
    /// </summary>
    public class ChatService : TrovoServiceBase
    {
        /// <summary>
        /// Creates an instance of the ChatService.
        /// </summary>
        /// <param name="connection">The Trovo connection to use</param>
        public ChatService(TrovoConnection connection) : base(connection) { }

        public async Task<string> GetToken()
        {
            JObject jobj = await this.GetJObjectAsync("chat/token");
            if (jobj != null && jobj.ContainsKey("token"))
            {
                return jobj["token"].ToString();
            }
            return null;
        }

        public async Task SendMessage(string message)
        {
            JObject jobj = new JObject();
            jobj["content"] = message;
            await this.PostAsync("chat/send", AdvancedHttpClient.CreateContentFromObject(jobj));
        }
    }
}
