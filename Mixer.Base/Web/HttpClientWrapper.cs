using Mixer.Base.Model.OAuth;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Mixer.Base.Web
{
    public class HttpClientWrapper : HttpClient
    {
        private const string MixerRestAPIBaseAddress = "https://mixer.com/api/v1/";

        public HttpClientWrapper()
            : base()
        {
            this.BaseAddress = new Uri(MixerRestAPIBaseAddress);
            this.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public HttpClientWrapper(OAuthTokenModel token)
            : this()
        {
            this.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.accessToken);
        }
    }
}
