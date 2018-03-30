using System;
using System.Net;
using System.Net.Http;

namespace Mixer.Base.Util
{
    public class RestServiceRequestException : HttpRequestException
    {
        public string Request { get; set; }

        public HttpStatusCode StatusCode { get; private set; }

        public string Reason { get; private set; }

        public string Content { get; private set; }

        public RestServiceRequestException() : base() { }

        public RestServiceRequestException(string message) : base(message) { }

        public RestServiceRequestException(string message, Exception inner) : base(message, inner) { }

        public RestServiceRequestException(HttpResponseMessage response)
            : this(response.ReasonPhrase)
        {
            this.Request = response.RequestMessage.RequestUri.ToString();
            this.StatusCode = response.StatusCode;
            this.Reason = response.ReasonPhrase;
            this.Content = response.Content.ReadAsStringAsync().Result;
        }

        public override string ToString()
        {
            return this.Request + Environment.NewLine + this.Content + Environment.NewLine + base.ToString();
        }
    }
}
