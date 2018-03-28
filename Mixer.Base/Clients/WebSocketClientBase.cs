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
        public event EventHandler OnReconnectionOccurred;

        protected new ClientWebSocket webSocket;

        private string endpoint;
        private bool autoReconnect;

        public virtual async Task<bool> Connect(string endpoint, bool autoReconnect = true)
        {
            this.endpoint = endpoint;
            this.autoReconnect = autoReconnect;

            try
            {
                this.webSocket = this.CreateWebSocket();
                this.SetWebSocket(this.webSocket);

                await this.webSocket.ConnectAsync(new Uri(endpoint), CancellationToken.None);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                this.Receive();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

                return true;
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
            try
            {
                if (this.IsOpen())
                {
                    await this.webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
            catch (InvalidOperationException)
            {
                if (this.autoReconnect)
                {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    this.Reconnect();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                }
                throw;
            }
        }

        protected virtual ClientWebSocket CreateWebSocket()
        {
            return new ClientWebSocket();
        }

        protected async Task Reconnect()
        {
            Logger.Log("Disconnection occurred, starting reconnection process");

            await this.webSocketSemaphore.WaitAsync();

            do
            {
                await this.Disconnect();

                Logger.Log("Attempting reconnection...");

            } while (!await this.Connect(this.endpoint));

            this.webSocketSemaphore.Release();

            Logger.Log("Reconnection successful");

            if (this.OnReconnectionOccurred != null)
            {
                this.OnReconnectionOccurred(this, new EventArgs());
            }
        }

        protected override async Task<WebSocketCloseStatus> Receive()
        {
            WebSocketCloseStatus closeStatus = await base.Receive();

            if (this.GetState() == WebSocketState.Aborted && this.autoReconnect)
            {
                await this.Reconnect();
            }
            else
            {
                await this.Disconnect(closeStatus);
            }

            return closeStatus;
        }
    }
}
