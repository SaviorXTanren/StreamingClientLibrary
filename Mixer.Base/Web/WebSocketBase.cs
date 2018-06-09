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

        /// <summary>
        /// Occurs when this web socket experiences an unexpected disconnection.
        /// </summary>
        public event EventHandler<WebSocketCloseStatus> OnDisconnectOccurred;

        protected WebSocket webSocket;

        protected SemaphoreSlim webSocketSemaphore = new SemaphoreSlim(1);

        /// <summary>
        /// Disconnects the web socket.
        /// </summary>
        /// <param name="closeStatus">Optional status to send to partner web socket as to why the web socket is being closed</param>
        /// <returns>A task for the closing of the web socket</returns>
        public virtual Task Disconnect(WebSocketCloseStatus closeStatus = WebSocketCloseStatus.NormalClosure)
        {
            if (this.webSocket != null)
            {
                try
                {
                    if (GetState() != WebSocketState.Closed)
                    {
                        this.webSocket.CloseAsync(closeStatus, string.Empty, CancellationToken.None).Wait(1);
                    }
                }
                catch (Exception ex) { Logger.Log(ex); }
            }
            this.webSocket = null;

            return Task.FromResult(0);
        }

        /// <summary>
        /// Sends a text packet to the connected web socket.
        /// </summary>
        /// <param name="packet">The text packet to send</param>
        /// <returns>A task for the sending of the packet</returns>
        public virtual async Task Send(string packet)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(packet);

            await this.webSocketSemaphore.WaitAsync();

            try
            {
                await this.SendInternal(buffer);
            }
            finally
            {
                this.webSocketSemaphore.Release();
            }

            this.OnSentOccurred?.Invoke(this, packet);
        }

        /// <summary>
        /// Gets whether the web socket is currently open.
        /// </summary>
        /// <returns>Whether the web socket is currently open</returns>
        public bool IsOpen() { return this.GetState() == WebSocketState.Open; }

        /// <summary>
        /// Gets the current state of the web socket.
        /// </summary>
        /// <returns>The current state of the web socket</returns>
        public WebSocketState GetState()
        {
            try
            {
                if (this.webSocket != null && (this.webSocket.CloseStatus == null || this.webSocket.CloseStatus == WebSocketCloseStatus.Empty))
                {
                    return this.webSocket.State;
                }
            }
            finally { }
            return WebSocketState.Closed;
        }

        protected void SetWebSocket(WebSocket webSocket) { this.webSocket = webSocket; }

        protected abstract Task SendInternal(byte[] buffer);

        protected abstract Task ProcessReceivedPacket(string packetJSON);

        protected virtual async Task<WebSocketCloseStatus> Receive()
        {
            string jsonBuffer = string.Empty;
            byte[] buffer = new byte[WebSocketBase.bufferSize];
            ArraySegment<byte> arrayBuffer = new ArraySegment<byte>(buffer);

            WebSocketCloseStatus closeStatus = WebSocketCloseStatus.NormalClosure;

            try
            {
                while (this.IsOpen())
                {
                    try
                    {
                        Array.Clear(buffer, 0, buffer.Length);
                        WebSocketReceiveResult result = await this.webSocket.ReceiveAsync(arrayBuffer, CancellationToken.None);

                        if (result != null)
                        {
                            if (result.MessageType == WebSocketMessageType.Close || (result.CloseStatus != null && result.CloseStatus.GetValueOrDefault() != WebSocketCloseStatus.Empty))
                            {
                                closeStatus = result.CloseStatus.GetValueOrDefault();
                            }
                            else if (result.MessageType == WebSocketMessageType.Text)
                            {
                                jsonBuffer += Encoding.UTF8.GetString(buffer, 0, result.Count);
                                if (result.EndOfMessage)
                                {
                                    this.OnReceivedOccurred?.Invoke(this, jsonBuffer);

                                    await this.ProcessReceivedPacket(jsonBuffer);
                                    jsonBuffer = string.Empty;
                                }
                            }
                            else
                            {
                                Logger.Log("Unsupported packet received");
                            }
                        }
                    }
                    catch (TaskCanceledException) { }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                        closeStatus = WebSocketCloseStatus.InternalServerError;
                        jsonBuffer = string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                closeStatus = WebSocketCloseStatus.InternalServerError;
            }

            if (closeStatus != WebSocketCloseStatus.NormalClosure)
            {
                await this.DisconnectAndFireEvent(closeStatus);
            }
            else
            {
                await this.Disconnect(closeStatus);
            }

            return closeStatus;
        }

        protected async Task DisconnectAndFireEvent(WebSocketCloseStatus closeStatus = WebSocketCloseStatus.NormalClosure)
        {
            await this.Disconnect(closeStatus);

            Task.Run(() => { this.OnDisconnectOccurred?.Invoke(this, closeStatus); }).Wait(1);
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
