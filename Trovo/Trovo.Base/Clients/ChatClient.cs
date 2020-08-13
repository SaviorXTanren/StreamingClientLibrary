using Newtonsoft.Json.Linq;
using StreamingClient.Base.Util;
using StreamingClient.Base.Web;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Trovo.Base.Models.Chat;

namespace Trovo.Base.Clients
{
    public class ChatClient : ClientWebSocketBase
    {
        public const string TrovoChatConnectionURL = "wss://open-chat.trovo.live/chat";

        public event EventHandler<ChatMessageContainerModel> OnChatMessageReceived = delegate { };

        private TrovoConnection connection;

        private CancellationTokenSource backgroundPingCancellationTokenSource;

        private readonly Dictionary<string, ChatPacketModel> replyIDListeners = new Dictionary<string, ChatPacketModel>();

        public ChatClient(TrovoConnection connection)
        {
            this.connection = connection;
        }

        public async Task<bool> Connect()
        {
            string token = await this.connection.Chat.GetToken();
            if (!string.IsNullOrEmpty(token))
            {
                if (await base.Connect(TrovoChatConnectionURL))
                {
                    ChatPacketModel authReply = await this.SendAndListen(new ChatPacketModel("AUTH", new JObject() { { "token", token } }));
                    if (authReply != null && string.IsNullOrEmpty(authReply.error))
                    {
                        this.backgroundPingCancellationTokenSource = new CancellationTokenSource();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                        Task.Run(() => this.BackgroundPing(this.backgroundPingCancellationTokenSource.Token), this.backgroundPingCancellationTokenSource.Token);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

                        return true;
                    }
                }
            }
            return false;
        }

        public async Task Disconnect()
        {
            if (this.backgroundPingCancellationTokenSource != null)
            {
                this.backgroundPingCancellationTokenSource.Cancel();
                this.backgroundPingCancellationTokenSource = null;
            }
            await base.Disconnect();
        }

        public async Task SendMessage(string message)
        {
            await this.connection.Chat.SendMessage(message);
        }

        public async Task<ChatPacketModel> Ping()
        {
            return await this.SendAndListen(new ChatPacketModel("PING"));
        }

        protected async Task Send(ChatPacketModel packet) { await this.Send(JSONSerializerHelper.SerializeToString(packet)); }

        /// <summary>
        /// Sends a packet to the server and listens for a reply.
        /// </summary>
        /// <param name="packet">The packet to send</param>
        /// <returns>An awaitable task with the reply packet</returns>
        protected async Task<ChatPacketModel> SendAndListen(ChatPacketModel packet)
        {
            ChatPacketModel replyPacket = null;
            this.replyIDListeners[packet.nonce] = null;
            await this.Send(packet);

            await this.WaitForSuccess(() =>
            {
                if (this.replyIDListeners.ContainsKey(packet.nonce) && this.replyIDListeners[packet.nonce] != null)
                {
                    replyPacket = this.replyIDListeners[packet.nonce];
                    return true;
                }
                return false;
            }, secondsToWait: 5);

            this.replyIDListeners.Remove(packet.nonce);
            return replyPacket;
        }

        protected override Task ProcessReceivedPacket(string packet)
        {
            Logger.Log(LogLevel.Debug, "Trovo Chat Packet: " + packet);

            ChatPacketModel response = JSONSerializerHelper.DeserializeFromString<ChatPacketModel>(packet);
            if (response != null && !string.IsNullOrEmpty(response.type))
            {
                switch (response.type)
                {
                    case "RESPONSE":
                        if (this.replyIDListeners.ContainsKey(response.nonce))
                        {
                            this.replyIDListeners[response.nonce] = response;
                        }
                        break;
                    case "CHAT":
                        this.SendSpecificPacket(response, this.OnChatMessageReceived);
                        break;
                }
            }

            return Task.FromResult(0);
        }

        private void SendSpecificPacket<T>(ChatPacketModel packet, EventHandler<T> eventHandler)
        {
            if (packet.data != null)
            {
                eventHandler?.Invoke(this, packet.data.ToObject<T>());
            }
        }

        private async Task BackgroundPing(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                int delay = 30;
                try
                {
                    ChatPacketModel reply = await this.Ping();
                    if (reply != null && reply.data != null && reply.data.ContainsKey("gap"))
                    {
                        int.TryParse(reply.data["gap"].ToString(), out delay);
                    }
                    await Task.Delay(delay * 1000);
                }
                catch (ThreadAbortException) { return; }
                catch (OperationCanceledException) { return; }
                catch (Exception ex) { Logger.Log(ex); }
            }
        }
    }
}
