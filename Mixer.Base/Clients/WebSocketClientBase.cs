using Mixer.Base.Util;
using System;
using System.IO;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mixer.Base.Clients
{
    public abstract class WebSocketClientBase
    {
        private const int bufferSize = 1000000;

        public event EventHandler<string> OnPacketSentOccurred;
        public event EventHandler<string> OnPacketReceivedOccurred;

        public event EventHandler<WebSocketCloseStatus> OnDisconnectOccurred;

        private string endpoint;
        private bool autoReconnect;

        private CancellationTokenSource cancellationTokenSource;
        private ClientWebSocket webSocket;

        private UTF8Encoding encoder = new UTF8Encoding();

        private SemaphoreSlim webSocketSemaphore = new SemaphoreSlim(1);

        public virtual async Task<bool> Connect(string endpoint, bool autoReconnect = true)
        {
            this.endpoint = endpoint;
            this.autoReconnect = autoReconnect;

            try
            {
                this.cancellationTokenSource = new CancellationTokenSource();
                this.webSocket = this.CreateWebSocket();

                await this.webSocket.ConnectAsync(new Uri(endpoint), this.cancellationTokenSource.Token);

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

        public Task Disconnect(WebSocketCloseStatus closeStatus = WebSocketCloseStatus.NormalClosure)
        {
            if (this.webSocket != null)
            {
                try
                {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    this.webSocket.CloseAsync(closeStatus, string.Empty, this.cancellationTokenSource.Token);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                }
                catch (Exception ex) { Logger.Log(ex); }
            }
            this.webSocket = null;

            if (this.OnDisconnectOccurred != null)
            {
                this.OnDisconnectOccurred(this, closeStatus);
            }

            return Task.FromResult(0);
        }

        public bool IsOpen() { return (this.GetState() == WebSocketState.Open || this.GetState() == WebSocketState.Connecting); }

        public WebSocketState GetState()
        {
            if (this.webSocket != null)
            {
                return this.webSocket.State;
            }
            return WebSocketState.Closed;
        }

        public virtual async Task Send(string packet, bool checkIfAuthenticated = true)
        {
            byte[] buffer = this.encoder.GetBytes(packet);

            await this.webSocketSemaphore.WaitAsync();

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
            finally
            {
                this.webSocketSemaphore.Release();
            }

            if (this.OnPacketSentOccurred != null)
            {
                this.OnPacketSentOccurred(this, packet);
            };
        }

        protected abstract Task ProcessReceivedPacket(string packetJSON);

        protected virtual ClientWebSocket CreateWebSocket()
        {
            return new ClientWebSocket();
        }

        protected virtual void DisconnectOccurred(WebSocketCloseStatus? result)
        {
            if (this.OnDisconnectOccurred != null)
            {
                this.OnDisconnectOccurred(this, result.GetValueOrDefault());
            }
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
        }

        protected async Task WaitForResponse(Func<bool> valueToCheck)
        {
            for (int i = 0; i < 50 && !valueToCheck(); i++)
            {
                await Task.Delay(100);
            }
        }

        private async Task Receive()
        {
            string jsonBuffer = string.Empty;
            byte[] buffer = new byte[WebSocketClientBase.bufferSize];

            WebSocketCloseStatus closeStatus = WebSocketCloseStatus.NormalClosure;

            try
            {
                while (this.IsOpen())
                {
                    try
                    {
                        Array.Clear(buffer, 0, buffer.Length);
                        WebSocketReceiveResult result = await this.webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                        if (result != null)
                        {
                            if (result.CloseStatus == null || result.CloseStatus == WebSocketCloseStatus.Empty)
                            {
                                jsonBuffer += this.encoder.GetString(buffer);
                                if (result.EndOfMessage)
                                {
                                    if (this.OnPacketReceivedOccurred != null)
                                    {
                                        this.OnPacketReceivedOccurred(this, jsonBuffer);
                                    }

                                    await this.ProcessReceivedPacket(jsonBuffer);

                                    jsonBuffer = string.Empty;
                                }
                            }
                            else
                            {
                                closeStatus = result.CloseStatus.GetValueOrDefault();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }

            if (this.GetState() == WebSocketState.Aborted && this.autoReconnect)
            {
                await this.Reconnect();
            }
            else
            {
                await this.Disconnect(closeStatus);
            }
        }
    }
}
