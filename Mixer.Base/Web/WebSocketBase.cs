using Mixer.Base.Util;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mixer.Base.Web
{
    public abstract class WebSocketBase
    {
        private const int bufferSize = 1000000;

        public event EventHandler<string> OnSentOccurred;
        public event EventHandler<string> OnReceivedOccurred;

        public event EventHandler<WebSocketCloseStatus> OnDisconnectOccurred;

        protected WebSocket webSocket;

        protected UTF8Encoding encoder = new UTF8Encoding();

        protected SemaphoreSlim webSocketSemaphore = new SemaphoreSlim(1);

        public Task Disconnect(WebSocketCloseStatus closeStatus = WebSocketCloseStatus.NormalClosure)
        {
            if (this.webSocket != null)
            {
                try
                {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    this.webSocket.CloseAsync(closeStatus, string.Empty, CancellationToken.None);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                }
                catch (Exception ex) { Logger.Log(ex); }
            }
            this.webSocket = null;

            this.DisconnectOccurred(closeStatus);

            return Task.FromResult(0);
        }

        public virtual async Task Send(string packet)
        {
            byte[] buffer = this.encoder.GetBytes(packet);

            await this.webSocketSemaphore.WaitAsync();

            try
            {
                if (this.IsOpen())
                {
                    await this.SendInternal(buffer);
                }
            }
            finally
            {
                this.webSocketSemaphore.Release();
            }

            if (this.OnSentOccurred != null)
            {
                this.OnSentOccurred(this, packet);
            };
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

        protected void SetWebSocket(WebSocket webSocket) { this.webSocket = webSocket; }

        protected abstract Task SendInternal(byte[] buffer);

        protected abstract Task ProcessReceivedPacket(string packetJSON);

        protected virtual void DisconnectOccurred(WebSocketCloseStatus closeStatus)
        {
            if (this.OnDisconnectOccurred != null)
            {
                this.OnDisconnectOccurred(this, closeStatus);
            }
        }

        protected virtual async Task<WebSocketCloseStatus> Receive()
        {
            string jsonBuffer = string.Empty;
            byte[] buffer = new byte[WebSocketBase.bufferSize];

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
                                    if (this.OnReceivedOccurred != null)
                                    {
                                        this.OnReceivedOccurred(this, jsonBuffer);
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

            return closeStatus;
        }

        protected async Task WaitForResponse(Func<bool> valueToCheck)
        {
            for (int i = 0; i < 50 && !valueToCheck(); i++)
            {
                await Task.Delay(100);
            }
        }
    }
}
