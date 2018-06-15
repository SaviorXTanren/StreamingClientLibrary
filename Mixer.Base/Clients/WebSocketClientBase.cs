using Mixer.Base.Util;
using Mixer.Base.Web;
using System;
using System.IO;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Mixer.Base.Clients
{
    public abstract class WebSocketClientBase : WebSocketBase
    {
        protected new ClientWebSocket webSocket;

        public string Endpoint { get; private set; }

        /// <summary>
        /// Connects the web socket to the server.
        /// </summary>
        /// <param name="endpoint">The endpoint to connect to</param>
        /// <returns>Whether the connection was successful</returns>
        public virtual async Task<bool> Connect(string endpoint)
        {
            this.Endpoint = endpoint;

            try
            {
                this.webSocket = this.CreateWebSocket();
                this.SetWebSocket(this.webSocket);

                await this.webSocket.ConnectAsync(new Uri(endpoint), CancellationToken.None);

                await Task.Delay(1000);

                this.Receive().Wait(1);

                return IsOpen();
            }
            catch (Exception ex)
            {
                await this.Disconnect();
                if (ex is WebSocketException && ex.InnerException is WebException)
                {
                    WebException webException = (WebException)ex.InnerException;
                    if (webException.Response != null && webException.Response is HttpWebResponse)
                    {
                        HttpWebResponse response = (HttpWebResponse)webException.Response;
                        StreamReader reader = new StreamReader(response.GetResponseStream());
                        string responseString = reader.ReadToEnd();
                        throw new WebSocketException(string.Format("{0} - {1} - {2}", response.StatusCode, response.StatusDescription, responseString), ex);
                    }
                }
                throw;
            }
        }

        protected override async Task SendInternal(byte[] buffer)
        {
            await this.webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        protected virtual ClientWebSocket CreateWebSocket()
        {
            return new ClientWebSocket();
        }
    }
}
