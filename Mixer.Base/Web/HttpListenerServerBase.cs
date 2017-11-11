using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;

namespace Mixer.Base.Web
{
    public abstract class HttpListenerServerBase
    {
        private string address;

        private HttpListener listener;
        private Thread listenerThread;
        private CancellationTokenSource listenerThreadCancellationTokenSource = new CancellationTokenSource();

        public HttpListenerServerBase(string address)
        {
            this.address = address;
            this.listener = new HttpListener();
        }

        public void Start()
        {
            this.listener.Prefixes.Add(this.address);
            this.listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;

            this.listener.Start();
            this.listenerThread = new Thread(new ParameterizedThreadStart(this.Listen));
            this.listenerThread.Start();
        }

        public void End()
        {
            this.listenerThreadCancellationTokenSource.Cancel();
            this.listener.Abort();
        }

        protected virtual HttpStatusCode RequestReceived(HttpListenerRequest request, string data, out string result)
        {
            result = string.Empty;
            return HttpStatusCode.OK;
        }

        private void Listen(object s)
        {
            while (!this.listenerThreadCancellationTokenSource.IsCancellationRequested)
            {
                this.listenerThreadCancellationTokenSource.Token.ThrowIfCancellationRequested();

                var result = this.listener.BeginGetContext(this.ListenerCallback, this.listener);
                result.AsyncWaitHandle.WaitOne();
            }

            this.listenerThreadCancellationTokenSource.Token.ThrowIfCancellationRequested();
        }

        private void ListenerCallback(IAsyncResult result)
        {
            try
            {
                var context = listener.EndGetContext(result);
                Thread.Sleep(1000);
                var data_text = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding).ReadToEnd();

                var cleanedData = HttpUtility.UrlDecode(data_text);

                string streamResult = string.Empty;
                HttpStatusCode code = this.RequestReceived(context.Request, cleanedData, out streamResult);

                context.Response.Headers["Access-Control-Allow-Origin"] = "*";
                context.Response.StatusCode = (int)code;
                context.Response.StatusDescription = code.ToString();

                byte[] buffer = Encoding.UTF8.GetBytes(streamResult);
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);

                context.Response.Close();
            }
            catch (Exception) { }
        }
    }
}
