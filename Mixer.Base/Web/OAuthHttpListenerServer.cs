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

        protected override async Task ProcessConnection(HttpListenerContext listenerContext)
        {
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            string result = string.Empty;

            string token = this.GetIdentifierValue(listenerContext, URL_CODE_IDENTIFIER);
            if (!string.IsNullOrEmpty(token))
            {
                statusCode = HttpStatusCode.OK;
                result = defaultSuccessResponse;
                if (this.loginSuccessHtmlPageFilePath != null && File.Exists(this.loginSuccessHtmlPageFilePath))
                {
                    result = File.ReadAllText(this.loginSuccessHtmlPageFilePath);
                }

                this.OAuthTokenModel = token;
            }

            await this.CloseConnection(listenerContext, statusCode, result);
        }

        protected string GetIdentifierValue(HttpListenerContext listenerContext, string identifier)
        {
            if (listenerContext.Request.RawUrl.Contains(identifier))
            {
                int startIndex = listenerContext.Request.RawUrl.IndexOf(identifier);

                string token = listenerContext.Request.RawUrl.Substring(startIndex + identifier.Length);

                int endIndex = token.IndexOf("&");
                if (endIndex > 0)
                {
                    token = token.Substring(0, endIndex);
                }
                return token;
            }
            return null;
        }
    }
}
