using Mixer.Base.Model.Client;
using Mixer.Base.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        private const string ClientNotConnectedExceptionMessage = "Client is not connected";

        public event EventHandler<WebSocketPacket> OnPacketSentOccurred;

        public event EventHandler<string> OnPacketReceivedOccurred;

        public event EventHandler<WebSocketCloseStatus> OnDisconnectOccurred;

        protected ClientWebSocket webSocket;
        private UTF8Encoding encoder = new UTF8Encoding();

        private SemaphoreSlim packetIDSemaphore = new SemaphoreSlim(1);
        private SemaphoreSlim sendSemaphore = new SemaphoreSlim(1);

        private int randomPacketIDSeed = (int)DateTime.Now.Ticks;
        private Dictionary<uint, ReplyPacket> replyIDListeners = new Dictionary<uint, ReplyPacket>();

        public WebSocketClientBase() { }

        protected async Task ConnectInternal(string endpoint)
        {
            try
            {
                this.webSocket = new ClientWebSocket();
                await this.webSocket.ConnectAsync(new Uri(endpoint), CancellationToken.None);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                this.Receive();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }
            catch (WebSocketException ex)
            {
                if (ex.InnerException is WebException)
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
                throw ex;
            }
        }

        public Task Disconnect()
        {
            try
            {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                this.webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                this.webSocket = null;
            }
            catch (Exception ex) { Logger.Log(ex); }
            return Task.FromResult(0);
        }

        public WebSocketState GetState()
        {
            if (this.webSocket != null)
            {
                return this.webSocket.State;
            }
            return WebSocketState.Closed;
        }

        public bool IsOpen() { return (this.GetState() == WebSocketState.Open || this.GetState() == WebSocketState.Connecting); }

        protected abstract Task ProcessReceivedPacket(string packetJSON);

        protected virtual async Task<uint> Send(WebSocketPacket packet, bool checkIfAuthenticated = true)
        {
            await this.AssignPacketID(packet);

            string packetJson = JsonConvert.SerializeObject(packet);
            byte[] buffer = this.encoder.GetBytes(packetJson);

            await this.sendSemaphore.WaitAsync();
            try
            {
                await this.webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch (InvalidOperationException ex)
            {
                Logger.Log(ex);
                if (!string.IsNullOrEmpty(ex.Message) && ex.Message.Contains(ClientNotConnectedExceptionMessage))
                {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    this.DisconnectOccurred(WebSocketCloseStatus.Empty);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                }
                else
                {
                    throw;
                }
            }
            finally
            {
                this.sendSemaphore.Release();
            }

            if (this.OnPacketSentOccurred != null)
            {
                this.OnPacketSentOccurred(this, packet);
            }

            return packet.id;
        }

        protected async Task<ReplyPacket> SendAndListen(WebSocketPacket packet, bool checkIfAuthenticated = true)
        {
            ReplyPacket replyPacket = null;

            await this.AssignPacketID(packet);
            this.replyIDListeners[packet.id] = null;

            await this.Send(packet, checkIfAuthenticated);

            await this.WaitForResponse(() =>
            {
                if (this.replyIDListeners.ContainsKey(packet.id) && this.replyIDListeners[packet.id] != null)
                {
                    replyPacket = this.replyIDListeners[packet.id];
                    return true;
                }
                return false;
            });

            this.replyIDListeners.Remove(packet.id);

            return replyPacket;
        }

        protected async Task<T> SendAndListen<T>(WebSocketPacket packet, bool checkIfAuthenticated = true)
        {
            ReplyPacket reply = await this.SendAndListen(packet);
            return this.GetSpecificReplyResultValue<T>(reply);
        }

        protected bool VerifyDataExists(ReplyPacket replyPacket)
        {
            return (replyPacket != null && replyPacket.data != null && !string.IsNullOrEmpty(replyPacket.data.ToString()));
        }

        protected bool VerifyNoErrors(ReplyPacket replyPacket)
        {
            if (replyPacket == null)
            {
                return false;
            }
            if (replyPacket.errorObject != null)
            {
                throw new ReplyPacketException(JsonConvert.DeserializeObject<ReplyErrorModel>(replyPacket.error.ToString()));
            }
            return true;
        }

        protected T GetSpecificReplyResultValue<T>(ReplyPacket replyPacket)
        {
            this.VerifyNoErrors(replyPacket);

            if (replyPacket != null)
            {
                if (replyPacket.resultObject != null)
                {
                    return JsonConvert.DeserializeObject<T>(replyPacket.resultObject.ToString());
                }
                else if (replyPacket.dataObject != null)
                {
                    return JsonConvert.DeserializeObject<T>(replyPacket.dataObject.ToString());
                }
            }
            return default(T);
        }

        protected async Task WaitForResponse(Func<bool> valueToCheck)
        {
            for (int i = 0; i < 50 && !valueToCheck(); i++)
            {
                await Task.Delay(100);
            }
        }

        protected void SendSpecificPacket<T>(T packet, EventHandler<T> eventHandler)
        {
            if (eventHandler != null)
            {
                eventHandler(this, packet);
            }
        }

        protected void AddReplyPacketForListeners(ReplyPacket packet)
        {
            if (this.replyIDListeners.ContainsKey(packet.id))
            {
                this.replyIDListeners[packet.id] = packet;
            }
        }

        protected virtual void DisconnectOccurred(WebSocketCloseStatus? result)
        {
            if (this.OnDisconnectOccurred != null)
            {
                this.OnDisconnectOccurred(this, result.GetValueOrDefault());
            }
        }

        private async Task Receive()
        {
            string jsonBuffer = string.Empty;
            byte[] buffer = new byte[WebSocketClientBase.bufferSize];

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
                            if (result.CloseStatus == null || result.CloseStatus != WebSocketCloseStatus.Empty)
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
                                this.DisconnectOccurred(result.CloseStatus);
                                return;
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

            if (this.GetState() == WebSocketState.Aborted)
            {
                this.DisconnectOccurred(this.webSocket.CloseStatus);
            }
            else
            {
                this.DisconnectOccurred(WebSocketCloseStatus.NormalClosure);
            }
        }

        private async Task AssignPacketID(WebSocketPacket packet)
        {
            if (packet.id == 0)
            {
                await this.packetIDSemaphore.WaitAsync();

                this.randomPacketIDSeed -= 1000;
                Random random = new Random(this.randomPacketIDSeed);
                packet.id = (uint)random.Next(100, int.MaxValue);

                this.packetIDSemaphore.Release();
            }
        }
    }
}
