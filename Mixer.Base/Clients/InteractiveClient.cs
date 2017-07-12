using Mixer.Base.Model.Channel;
using Mixer.Base.Model.Client;
using Mixer.Base.Model.Interactive;
using Mixer.Base.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Mixer.Base.Clients
{
    public class InteractiveClient : WebSocketClientBase
    {
        public event EventHandler<InteractiveIssueMemoryWarningModel> OnIssueMemoryWarningOccurred;

        public ChannelModel Channel { get; private set; }
        public InteractiveGameListingModel InteractiveGame { get; private set; }

        private IEnumerable<string> interactiveConnections;

        public static async Task<InteractiveClient> CreateFromChannel(MixerConnection connection, ChannelModel channel, InteractiveGameListingModel interactiveGame)
        {
            Validator.ValidateVariable(connection, "connection");
            Validator.ValidateVariable(channel, "channel");
            Validator.ValidateVariable(interactiveGame, "interactiveGame");

            AuthorizationToken authToken = await connection.GetAuthorizationToken();

            IEnumerable<string> interactiveConnections = await connection.Interactive.GetInteractiveHosts();

            return new InteractiveClient(channel, interactiveGame, authToken, interactiveConnections);
        }

        private InteractiveClient(ChannelModel channel, InteractiveGameListingModel interactiveGame, AuthorizationToken authToken, IEnumerable<string> interactiveConnections)
        {
            Validator.ValidateVariable(channel, "channel");
            Validator.ValidateVariable(interactiveGame, "interactiveGame");
            Validator.ValidateVariable(authToken, "authToken");
            Validator.ValidateList(interactiveConnections, "interactiveConnections");

            this.Channel = channel;
            this.InteractiveGame = interactiveGame;
            this.interactiveConnections = interactiveConnections;

            AuthenticationHeaderValue authHeader = new AuthenticationHeaderValue("Bearer", authToken.AccessToken);
            this.webSocket.Options.SetRequestHeader("Authorization", authHeader.ToString());
            this.webSocket.Options.SetRequestHeader("X-Interactive-Version", this.InteractiveGame.versions.First().id.ToString());
            this.webSocket.Options.SetRequestHeader("X-Protocol-Version", "2.0");
        }

        public async Task<bool> Connect()
        {
            this.OnDisconnectOccurred -= InteractiveClient_OnDisconnectOccurred;
            this.OnMethodOccurred -= InteractiveClient_OnMethodOccurred;

            int totalEndpoints = this.interactiveConnections.Count();
            Random random = new Random();
            int endpointToUse = random.Next() % totalEndpoints;

            this.OnMethodOccurred += InteractiveClient_HelloMethodHandler;

            await this.ConnectInternal(this.interactiveConnections.ElementAt(endpointToUse));

            await this.WaitForResponse(() => { return this.connectSuccessful; });

            this.OnMethodOccurred -= InteractiveClient_HelloMethodHandler;

            if (this.connectSuccessful)
            {
                this.OnDisconnectOccurred += InteractiveClient_OnDisconnectOccurred;
                this.OnMethodOccurred += InteractiveClient_OnMethodOccurred;
            }

            return this.connectSuccessful;
        }

        public async Task<bool> Ready()
        {
            this.authenticateSuccessful = false;

            this.OnMethodOccurred += InteractiveClient_ReadyMethodHandler;

            JObject parameters = new JObject();
            parameters.Add("isReady", true);
            MethodPacket packet = new MethodPacket()
            {
                method = "ready",
                parameters = parameters,
                discard = true
            };

            await this.Send(packet, checkIfAuthenticated: false);

            await this.WaitForResponse(() => { return this.authenticateSuccessful; });

            this.OnMethodOccurred -= InteractiveClient_ReadyMethodHandler;

            return this.authenticateSuccessful;
        }

        public async Task<DateTimeOffset?> GetTime()
        {
            MethodPacket packet = new MethodPacket() { method = "getTime" };
            ReplyPacket reply = await this.SendAndListen(packet);
            if (reply != null && reply.resultObject["time"] != null)
            {
                return DateTimeHelper.ParseUnixTimestamp((long)reply.resultObject["time"]);
            }
            return null;
        }

        public async Task<InteractiveIssueMemoryWarningModel> GetMemoryStates()
        {
            MethodPacket packet = new MethodPacket() { method = "getMemoryStats" };
            ReplyPacket reply = await this.SendAndListen(packet);
            return this.GetSpecificReplyResultValue<InteractiveIssueMemoryWarningModel>(reply);
        }

        private void InteractiveClient_OnMethodOccurred(object sender, MethodPacket methodPacket)
        {
            switch (methodPacket.method)
            {
                case "issueMemoryWarning":
                    this.SendSpecificMethod<InteractiveIssueMemoryWarningModel>(methodPacket, OnIssueMemoryWarningOccurred);
                    break;
            }
        }

        private void SendSpecificMethod<T>(MethodPacket methodPacket, EventHandler<T> eventHandler)
        {
            if (eventHandler != null)
            {
                eventHandler(this, JsonConvert.DeserializeObject<T>(methodPacket.parameters.ToString()));
            }
        }

        private void InteractiveClient_HelloMethodHandler(object sender, MethodPacket e)
        {
            if (e.method.Equals("hello"))
            {
                this.connectSuccessful = true;
            }
        }

        private void InteractiveClient_ReadyMethodHandler(object sender, MethodPacket e)
        {
            JToken value;
            if (e.method.Equals("onReady") && e.parameters.TryGetValue("isReady", out value) && (bool)value)
            {
                this.authenticateSuccessful = true;
            }
        }

        private async void InteractiveClient_OnDisconnectOccurred(object sender, WebSocketCloseStatus e)
        {
            this.connectSuccessful = false;
            this.authenticateSuccessful = false;
            if (await this.Connect())
            {
                await this.Ready();
            }
        }
    }
}
