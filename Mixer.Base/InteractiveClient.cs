using Mixer.Base.Model.Channel;
using Mixer.Base.Model.Interactive;
using Mixer.Base.Util;
using Mixer.Base.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Mixer.Base
{
    public class InteractiveClient : WebSocketClientBase
    {
        public event EventHandler<InteractiveReplyPacket> ReplyOccurred;
        public event EventHandler<InteractiveMethodPacket> MethodOccurred;

        public event EventHandler<InteractiveIssueMemoryWarningModel> IssueMemoryWarningOccurred;

        public ChannelModel Channel { get; private set; }
        public InteractiveGameListingModel InteractiveGame { get; private set; }

        private IEnumerable<string> interactiveConnections;

        public static async Task<InteractiveClient> CreateFromChannel(MixerClient client, ChannelModel channel, InteractiveGameListingModel interactiveGame)
        {
            Validator.ValidateVariable(client, "client");
            Validator.ValidateVariable(channel, "channel");
            Validator.ValidateVariable(interactiveGame, "interactiveGame");

            AuthorizationToken authToken = await client.GetAuthorizationToken();

            IEnumerable<string> interactiveConnections = await client.Interactive.GetInteractiveHosts();

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
            this.DisconnectOccurred -= InteractiveClient_DisconnectOccurred;

            int totalEndpoints = this.interactiveConnections.Count();
            Random random = new Random();
            int endpointToUse = random.Next() % totalEndpoints;

            this.MethodOccurred += HelloMethodHandler;

            await this.ConnectInternal(this.interactiveConnections.ElementAt(endpointToUse));

            await this.WaitForResponse(() => { return this.connectSuccessful; });

            this.MethodOccurred -= HelloMethodHandler;

            if (this.connectSuccessful)
            {
                this.DisconnectOccurred += InteractiveClient_DisconnectOccurred;
            }

            return this.connectSuccessful;
        }

        public async Task<bool> Ready()
        {
            this.authenticateSuccessful = false;

            this.MethodOccurred += OnReadyMethodHandler;

            JObject parameters = new JObject();
            parameters.Add("isReady", true);
            InteractiveMethodPacket packet = new InteractiveMethodPacket()
            {
                method = "ready",
                parameters = parameters,
                discard = true
            };

            await this.Send(packet, checkIfAuthenticated: false);

            await this.WaitForResponse(() => { return this.authenticateSuccessful; });

            this.MethodOccurred -= OnReadyMethodHandler;

            return this.authenticateSuccessful;
        }

        public async Task GetTime()
        {
            InteractiveMethodPacket packet = new InteractiveMethodPacket() { method = "getTime" };
            await this.Send(packet);
        }

        protected override void Receive(string jsonBuffer)
        {
            InteractivePacket packet = JsonConvert.DeserializeObject<InteractivePacket>(jsonBuffer);
            if (packet.type.Equals("reply"))
            {
                InteractiveReplyPacket replyPacket = JsonConvert.DeserializeObject<InteractiveReplyPacket>(jsonBuffer);
                this.OnReplyOccurred(replyPacket);
            }
            else if (packet.type.Equals("method"))
            {
                InteractiveMethodPacket methodPacket = JsonConvert.DeserializeObject<InteractiveMethodPacket>(jsonBuffer);
                this.OnMethodOccurred(methodPacket);
            }
        }

        private void HelloMethodHandler(object sender, InteractiveMethodPacket e)
        {
            if (e.method.Equals("hello"))
            {
                this.connectSuccessful = true;
            }
        }

        private void OnReadyMethodHandler(object sender, InteractiveMethodPacket e)
        {
            JToken value;
            if (e.method.Equals("onReady") && e.parameters.TryGetValue("isReady", out value) && (bool)value)
            {
                this.authenticateSuccessful = true;
            }
        }

        private void OnReplyOccurred(InteractiveReplyPacket replyPacket)
        {
            if (this.ReplyOccurred != null)
            {
                this.ReplyOccurred(this, replyPacket);
            }
        }

        private void OnMethodOccurred(InteractiveMethodPacket methodPacket)
        {
            if (this.MethodOccurred != null)
            {
                this.MethodOccurred(this, methodPacket);
            }

            switch (methodPacket.method)
            {
                case "issueMemoryWarning":
                    this.SendSpecificMethod<InteractiveIssueMemoryWarningModel>(methodPacket, IssueMemoryWarningOccurred);
                    break;
            }
        }

        private async void InteractiveClient_DisconnectOccurred(object sender, WebSocketCloseStatus e)
        {
            if (await this.Connect())
            {
                await this.Ready();
            }
        }

        private void SendSpecificMethod<T>(InteractiveMethodPacket methodPacket, EventHandler<T> eventHandler)
        {
            if (eventHandler != null)
            {
                eventHandler(this, JsonConvert.DeserializeObject<T>(methodPacket.parameters.ToString()));
            }
        }
    }
}
