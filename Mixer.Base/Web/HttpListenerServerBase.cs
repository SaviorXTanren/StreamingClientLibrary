using Mixer.Base.Util;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Mixer.Base.Web
{
    public abstract class HttpListenerServerBase
    {
        private string address;

        private HttpListener httpListener;

        public HttpListenerServerBase(string address) { this.address = address; }

        public bool Start()
        {
            try
            {
                this.httpListener = new HttpListener();
                this.httpListener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
                this.httpListener.Prefixes.Add(this.address);

                this.httpListener.Start();

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                this.WaitForConnection();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

                return true;
            }
            catch (Exception ex) { Logger.Log(ex); }

            this.End();

            return false;
        }

        public void End()
        {
            try
            {
                if (this.httpListener != null)
                {
                    this.httpListener.Stop();
                }
            }
            catch (HttpListenerException) { }
            catch (Exception ex) { Logger.Log(ex); }
            this.httpListener = null;
        }

        protected abstract Task ProcessConnection(HttpListenerContext listenerContext);

        protected async Task WaitForConnection()
        {
            try
            {
                while (this.httpListener != null && this.httpListener.IsListening)
                {
                    try
                    {
                        HttpListenerContext listenerContext = await this.httpListener.GetContextAsync();
                        await this.ProcessConnection(listenerContext);
                        listenerContext.Response.Close();
                    }
                    catch (HttpListenerException) { }
                    catch (Exception ex) { Logger.Log(ex); }
                }
            }
            catch (Exception ex) { Logger.Log(ex); }
        }

        protected async Task<string> GetRequestData(HttpListenerContext listenerContext)
        {
            string data = await new StreamReader(listenerContext.Request.InputStream, listenerContext.Request.ContentEncoding).ReadToEndAsync();
            return HttpUtility.UrlDecode(data);
        }

        protected async Task CloseConnection(HttpListenerContext listenerContext, HttpStatusCode statusCode, string result)
        {
            listenerContext.Response.Headers["Access-Control-Allow-Origin"] = "*";
            listenerContext.Response.StatusCode = (int)statusCode;
            listenerContext.Response.StatusDescription = statusCode.ToString();

            byte[] buffer = Encoding.UTF8.GetBytes(result);
            await listenerContext.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        }
    }
}
