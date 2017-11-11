using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Mixer.Base.Web
{
    public class OAuthHttpListenerServer : HttpListenerServerBase
    {
        public const string URL_CODE_IDENTIFIER = "/?code=";
        private const string defaultSuccessResponse = "<!DOCTYPE html><html><body><h1 style=\"text-align:center;\">Logged In Successfully</h1><p style=\"text-align:center;\">You have been logged in, you may now close this webpage</p></body></html>";

        private string loginSuccessHtmlPageFilePath = null;
        private string OAuthTokenModel = null;

        public OAuthHttpListenerServer(string address) : base(address) { }

        public OAuthHttpListenerServer(string address, string loginSuccessHtmlPageFilePath) : this(address) { this.loginSuccessHtmlPageFilePath = loginSuccessHtmlPageFilePath; }

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
            result = "OK";
            if (request.RawUrl.Contains(URL_CODE_IDENTIFIER))
            {
                string token = request.RawUrl.Substring(URL_CODE_IDENTIFIER.Length);
                this.OAuthTokenModel = token.Substring(0, token.IndexOf("&"));

                result = defaultSuccessResponse;
                if (this.loginSuccessHtmlPageFilePath != null && File.Exists(this.loginSuccessHtmlPageFilePath))
                {
                    result = File.ReadAllText(this.loginSuccessHtmlPageFilePath);
                }
            }
            return HttpStatusCode.OK;
        }
    }
}
