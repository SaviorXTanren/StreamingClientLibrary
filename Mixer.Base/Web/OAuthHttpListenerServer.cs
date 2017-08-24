using System.Net;
using System.Threading.Tasks;

namespace Mixer.Base.Web
{
    public class OAuthHttpListenerServer : HttpListenerServerBase
    {
        public const string URL_CODE_IDENTIFIER = "/?code=";

        private string OAuthTokenModel = null;

        public OAuthHttpListenerServer(string address) : base(address) { }

        public async Task<string> WaitForAuthorizationCode()
        {
            for (int i = 0; i < 30; i++)
            {
                if (!string.IsNullOrEmpty(this.OAuthTokenModel))
                {
                    break;
                }
                await Task.Delay(1000);
            }
            return this.OAuthTokenModel;
        }

        protected override HttpStatusCode RequestReceived(HttpListenerRequest request, string data, out string result)
        {
            if (request.RawUrl.Contains(URL_CODE_IDENTIFIER))
            {
                string token = request.RawUrl.Substring(URL_CODE_IDENTIFIER.Length);
                this.OAuthTokenModel = token.Substring(0, token.IndexOf("&"));
            }
            result = "OK";
            return HttpStatusCode.OK;
        }
    }
}
