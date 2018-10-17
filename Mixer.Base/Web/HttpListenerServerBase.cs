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

                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        while (this.httpListener != null && this.httpListener.IsListening)
                        {
                            try
                            {
                                HttpListenerContext context = this.httpListener.GetContext();
                                Task.Factory.StartNew(async (ctx) =>
                                {
                                    await this.ProcessConnection((HttpListenerContext)ctx);
                                    ((HttpListenerContext)ctx).Response.Close();
                                }, context, TaskCreationOptions.LongRunning);
                            }
                            catch (HttpListenerException) { }
                            catch (Exception ex) { Logger.Log(ex); }
                        }
                    }
                    catch (Exception ex) { Logger.Log(ex); }

                    this.End();
                }, TaskCreationOptions.LongRunning);

                return true;
            }
            catch (Exception ex) { Logger.Log(ex); }

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
