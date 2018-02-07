using Mixer.Base.Model.Client;
using Mixer.Base.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    public abstract class WebSocketClientBase : IDisposable
    {
        private const int bufferSize = 1000000;

        public event EventHandler<WebSocketPacket> OnPacketSentOccurred;
        public event EventHandler<MethodPacket> OnMethodOccurred;
        public event EventHandler<ReplyPacket> OnReplyOccurred;
        public event EventHandler<EventPacket> OnEventOccurred;

        public event EventHandler<WebSocketCloseStatus> OnDisconnectOccurred;

        public bool Connected { get; protected set; }
        public bool Authenticated { get; protected set; }

        protected ClientWebSocket webSocket = new ClientWebSocket();
        private UTF8Encoding encoder = new UTF8Encoding();

        private SemaphoreSlim packetIDSemaphore = new SemaphoreSlim(1);
        private SemaphoreSlim sendSemaphore = new SemaphoreSlim(1);

        private int randomPacketIDSeed = (int)DateTime.Now.Ticks;

        public WebSocketClientBase() { }

        protected async Task ConnectInternal(string endpoint)
        {
            try
            {
                await this.webSocket.ConnectAsync(new Uri(endpoint), CancellationToken.None);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                this.ReceiveInternal();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }
            catch (WebSocketException ex)
            {
                if (ex.InnerException is WebException)
                {
                    WebException webException = (WebException)ex.InnerException;
                    HttpWebResponse response = (HttpWebResponse)webException.Response;
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string responseString = reader.ReadToEnd();

                    throw new WebSocketException(string.Format("{0} - {1} - {2}", response.StatusCode, response.StatusDescription, responseString), ex);
                }
                throw ex;
            }
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

        protected virtual async Task<uint> Send(WebSocketPacket packet, bool checkIfAuthenticated = true)
        {
            if (!this.Connected)
            {
                throw new InvalidOperationException("Client is not connected");
            }

            if (checkIfAuthenticated && !this.Authenticated)
            {
                throw new InvalidOperationException("Client is not authenticated");
            }

            if (packet.id == 0)
            {
                await this.packetIDSemaphore.WaitAsync();

                this.randomPacketIDSeed -= 1000;
                Random random = new Random(this.randomPacketIDSeed);
                packet.id = (uint)random.Next(100, int.MaxValue);

                this.packetIDSemaphore.Release();
            }

            string packetJson = JsonConvert.SerializeObject(packet);
            byte[] buffer = this.encoder.GetBytes(packetJson);

            await this.sendSemaphore.WaitAsync();
            try
            {
                await this.webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
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
            uint? id = null;

            ReplyPacket replyPacket = null;

            EventHandler<ReplyPacket> listener = new EventHandler<ReplyPacket>(delegate (object sender, ReplyPacket reply)
            {
                if (id != null && reply.id == id)
                {
                    replyPacket = reply;
                }
            });

            this.OnReplyOccurred += listener;

            id = await this.Send(packet, checkIfAuthenticated);

            await this.WaitForResponse(() => { return (replyPacket != null); });

            this.OnReplyOccurred -= listener;

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

        protected void SendSpecificMethod<T>(MethodPacket methodPacket, EventHandler<T> eventHandler)
        {
            if (eventHandler != null)
            {
                eventHandler(this, JsonConvert.DeserializeObject<T>(methodPacket.parameters.ToString()));
            }
        }

        protected void SendSpecificEvent<T>(EventPacket eventPacket, EventHandler<T> eventHandler)
        {
            if (eventHandler != null)
            {
                eventHandler(this, JsonConvert.DeserializeObject<T>(eventPacket.data.ToString()));
            }
        }

        protected async Task WaitForResponse(Func<bool> valueToCheck)
        {
            for (int i = 0; i < 500 && !valueToCheck(); i++)
            {
                await Task.Delay(10);
            }
        }

        private async Task ReceiveInternal()
        {
            byte[] buffer = new byte[WebSocketClientBase.bufferSize];

            while (this.webSocket != null)
            {
                if (this.webSocket.State == WebSocketState.Open)
                {
                    Array.Clear(buffer, 0, buffer.Length);
                    WebSocketReceiveResult result = await this.webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (result != null)
                    {
                        if (result.CloseStatus == null || result.CloseStatus != WebSocketCloseStatus.Empty)
                        {
                            List<WebSocketPacket> packets = new List<WebSocketPacket>();

                            string jsonBuffer = this.encoder.GetString(buffer);
                            dynamic jsonObject = JsonConvert.DeserializeObject(jsonBuffer);

                            if (jsonObject.Type == JTokenType.Array)
                            {
                                JArray array = JArray.Parse(jsonBuffer);
                                foreach (JToken token in array.Children())
                                {
                                    packets.Add(token.ToObject<WebSocketPacket>());
                                }
                            }
                            else
                            {
                                packets.Add(JsonConvert.DeserializeObject<WebSocketPacket>(jsonBuffer));
                            }

                            foreach (WebSocketPacket packet in packets)
                            {
                                try
                                {
                                    if (packet.type.Equals("method"))
                                    {
                                        MethodPacket methodPacket = JsonConvert.DeserializeObject<MethodPacket>(jsonBuffer);
                                        this.SendSpecificPacket(methodPacket, this.OnMethodOccurred);
                                    }
                                    else if (packet.type.Equals("reply"))
                                    {
                                        ReplyPacket replyPacket = JsonConvert.DeserializeObject<ReplyPacket>(jsonBuffer);
                                        this.SendSpecificPacket(replyPacket, this.OnReplyOccurred);
                                    }
                                    else if (packet.type.Equals("event"))
                                    {
                                        EventPacket eventPacket = JsonConvert.DeserializeObject<EventPacket>(jsonBuffer);
                                        this.SendSpecificPacket(eventPacket, this.OnEventOccurred);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex);
                                }
                            }
                        }
                        else
                        {
                            this.DisconnectOccurred(result.CloseStatus);
                        }
                    }
                }
                else if (this.webSocket.State == WebSocketState.CloseReceived || this.webSocket.State == WebSocketState.Closed)
                {
                    this.DisconnectOccurred(WebSocketCloseStatus.NormalClosure);
                }
                else if (this.webSocket.State == WebSocketState.Aborted)
                {
                    this.DisconnectOccurred(this.webSocket.CloseStatus);
                }
            }
        }

        private void SendSpecificPacket<T>(T packet, EventHandler<T> eventHandler)
        {
            if (eventHandler != null)
            {
                eventHandler(this, packet);
            }
        }

        private void DisconnectOccurred(WebSocketCloseStatus? result)
        {
            this.Connected = this.Authenticated = false;
            if (this.OnDisconnectOccurred != null)
            {
                this.OnDisconnectOccurred(this, result.GetValueOrDefault());
            }
        }
    }
}
