using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mixer.Base.Web
{
    public abstract class WebSocketClientBase : IDisposable
    {
        public event EventHandler<WebSocketCloseStatus> DisconnectOccurred;

        internal uint CurrentPacketID { get; private set; }

        private ClientWebSocket webSocket = new ClientWebSocket();
        private UTF8Encoding encoder = new UTF8Encoding();

        protected bool connectSuccessful { get; set; }

        private bool authenticateSuccessful = false;

        private int bufferSize = 4096 * 20;

        public WebSocketClientBase()
        {
            this.CurrentPacketID = 0;
        }

        protected async Task<bool> ConnectInternal(string endpoint)
        {
            this.connectSuccessful = false;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            this.ReceiveInternal();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            try
            {
                await this.webSocket.ConnectAsync(new Uri(endpoint), CancellationToken.None);
            }
            catch (WebSocketException ex)
            {
                WebException webException = (WebException)ex.InnerException;
                HttpWebResponse response = (HttpWebResponse)webException.Response;
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string responseString = reader.ReadToEnd();

                throw ex;
            }

            for (int i = 0; i < 10 && !this.connectSuccessful; i++)
            {
                await Task.Delay(500);
            }

            return this.connectSuccessful;
        }

        public async Task Disconnect()
        {
            try
            {
                await this.webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                this.Dispose();
            }
            catch (Exception) { }
        }

        public void Dispose()
        {
            if (this.webSocket != null)
            {
                this.webSocket.Dispose();
            }
        }

        protected async Task SendInternal(object packet)
        {
            string packetJson = JsonConvert.SerializeObject(packet);
            byte[] buffer = this.encoder.GetBytes(packetJson);

            await this.webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);

            this.CurrentPacketID++;
        }

        protected abstract void Receive(string jsonBuffer);

        private async Task ReceiveInternal()
        {
            await Task.Delay(100);
            while (this.webSocket != null)
            {
                if (this.webSocket.State == WebSocketState.Open)
                {
                    byte[] buffer = new byte[this.bufferSize];
                    WebSocketReceiveResult result = await this.webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result != null)
                    {
                        if (result.CloseStatus == null || result.CloseStatus != WebSocketCloseStatus.Empty)
                        {
                            string jsonBuffer = this.encoder.GetString(buffer);
                            this.Receive(jsonBuffer);
                        }
                        else
                        {
                            this.OnDisconnectOccurred(result);
                        }
                    }
                }
            }
        }

        private void OnDisconnectOccurred(WebSocketReceiveResult result)
        {
            if (this.DisconnectOccurred != null)
            {
                this.DisconnectOccurred(this, result.CloseStatus.GetValueOrDefault());
            }
        }
    }
}
