using Mixer.Base.Model.OAuth;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Mixer.Base.Web
{
    public class HttpClientWrapper : HttpClient
    {
        public HttpClientWrapper(string baseAddress)
            : base()
        {
            this.BaseAddress = new Uri(baseAddress);
            this.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public HttpClientWrapper(string baseAddress, OAuthTokenModel token)
            : this(baseAddress)
        {
            this.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.accessToken);
        }
    }
}
