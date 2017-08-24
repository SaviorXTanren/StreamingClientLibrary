using Mixer.Base.Model.Channel;
using Mixer.Base.Model.Client;
using Mixer.Base.Model.Interactive;
using Mixer.Base.Model.OAuth;
using Mixer.Base.Util;
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
        public event EventHandler<InteractiveIssueMemoryWarningModel> OnIssueMemoryWarning;

        public event EventHandler<InteractiveParticipantCollectionModel> OnParticipantLeave;
        public event EventHandler<InteractiveParticipantCollectionModel> OnParticipantJoin;
        public event EventHandler<InteractiveParticipantCollectionModel> OnParticipantUpdate;

        public event EventHandler<InteractiveGroupCollectionModel> OnGroupCreate;
        public event EventHandler<Tuple<InteractiveGroupModel, InteractiveGroupModel>> OnGroupDelete;
        public event EventHandler<InteractiveGroupCollectionModel> OnGroupUpdate;

        public event EventHandler<InteractiveConnectedSceneCollectionModel> OnSceneCreate;
        public event EventHandler<Tuple<InteractiveConnectedSceneModel, InteractiveConnectedSceneModel>> OnSceneDelete;
        public event EventHandler<InteractiveConnectedSceneCollectionModel> OnSceneUpdate;

        public event EventHandler<InteractiveConnectedSceneModel> OnControlCreate;
        public event EventHandler<InteractiveConnectedSceneModel> OnControlDelete;
        public event EventHandler<InteractiveConnectedSceneModel> OnControlUpdate;

        public event EventHandler<InteractiveGiveInputModel> OnGiveInput;

        public ChannelModel Channel { get; private set; }
        public InteractiveGameListingModel InteractiveGame { get; private set; }

        private IEnumerable<string> interactiveConnections;

        public static async Task<InteractiveClient> CreateFromChannel(MixerConnection connection, ChannelModel channel, InteractiveGameListingModel interactiveGame)
        {
            Validator.ValidateVariable(connection, "connection");
            Validator.ValidateVariable(channel, "channel");
            Validator.ValidateVariable(interactiveGame, "interactiveGame");

            OAuthTokenModel authToken = await connection.GetOAuthTokenModel();

            IEnumerable<string> interactiveConnections = await connection.Interactive.GetInteractiveHosts();

            return new InteractiveClient(channel, interactiveGame, authToken, interactiveConnections);
        }

        private InteractiveClient(ChannelModel channel, InteractiveGameListingModel interactiveGame, OAuthTokenModel authToken, IEnumerable<string> interactiveConnections)
        {
            Validator.ValidateVariable(channel, "channel");
            Validator.ValidateVariable(interactiveGame, "interactiveGame");
            Validator.ValidateVariable(authToken, "authToken");
            Validator.ValidateList(interactiveConnections, "interactiveConnections");

            this.Channel = channel;
            this.InteractiveGame = interactiveGame;
            this.interactiveConnections = interactiveConnections;

            AuthenticationHeaderValue authHeader = new AuthenticationHeaderValue("Bearer", authToken.accessToken);
            this.webSocket.Options.SetRequestHeader("Authorization", authHeader.ToString());
            this.webSocket.Options.SetRequestHeader("X-Interactive-Version", this.InteractiveGame.versions.OrderByDescending(v => v.versionOrder).First().id.ToString());
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
                return DateTimeHelper.UnixTimestampToDateTimeOffset((long)reply.resultObject["time"]);
            }
            return null;
        }

        public async Task<InteractiveIssueMemoryWarningModel> GetMemoryStates()
        {
            MethodPacket packet = new MethodPacket() { method = "getMemoryStats" };
            ReplyPacket reply = await this.SendAndListen(packet);
            return this.GetSpecificReplyResultValue<InteractiveIssueMemoryWarningModel>(reply);
        }

        public async Task<bool> SetBandwidthThrottle(InteractiveSetBandwidthThrottleModel throttling)
        {
            MethodPacket packet = new MethodPacket() { method = "setBandwidthThrottle", parameters = throttling };
            ReplyPacket reply = await this.SendAndListen(packet);
            return this.VerifyNoErrors(reply);
        }

        public async Task<InteractiveGetThrottleStateModel> GetThrottleState()
        {
            MethodPacket packet = new MethodPacket() { method = "getThrottleState" };
            ReplyPacket reply = await this.SendAndListen(packet);
            if (this.VerifyNoErrors(reply))
            {
                return new InteractiveGetThrottleStateModel(reply.resultObject);
            }
            return new InteractiveGetThrottleStateModel();
        }

        public async Task<InteractiveParticipantCollectionModel> GetAllParticipants(uint from = 0) //TODO - TT - Not sure if we need to iterate through entire list here or not.
        {
            /* Spec p.19-20, 'from' is used to indicate earliest connect timestamp of the Participants. 
             * Initial request should be at 0 and each subsequent call should be max participant 'connectedAt' per result set */
            JObject parameters = new JObject();
            parameters.Add("from", from);
            MethodPacket packet = new MethodPacket() { method = "getAllParticipants", parameters = parameters };
            ReplyPacket reply = await this.SendAndListen(packet);
            return this.GetSpecificReplyResultValue<InteractiveParticipantCollectionModel>(reply);
        }

        public async Task<InteractiveParticipantCollectionModel> GetActiveParticipants(DateTimeOffset startThreshold)
        {
            JObject parameters = new JObject();
            parameters.Add("threshold", DateTimeHelper.DateTimeOffsetToUnixTimestamp(startThreshold));
            MethodPacket packet = new MethodPacket() { method = "getActiveParticipants", parameters = parameters };
            ReplyPacket reply = await this.SendAndListen(packet);
            return this.GetSpecificReplyResultValue<InteractiveParticipantCollectionModel>(reply);
        }

        public async Task<InteractiveParticipantCollectionModel> UpdateParticipants(IEnumerable<InteractiveParticipantModel> participants)
        {
            JObject parameters = new JObject();
            parameters.Add("participants", JArray.FromObject(participants));
            MethodPacket packet = new MethodPacket() { method = "updateParticipants", parameters = parameters };
            ReplyPacket reply = await this.SendAndListen(packet);
            return this.GetSpecificReplyResultValue<InteractiveParticipantCollectionModel>(reply);
        }

        public async Task<bool> CreateGroups(IEnumerable<InteractiveGroupModel> groups)
        {
            InteractiveGroupCollectionModel collection = new InteractiveGroupCollectionModel() { groups = groups.ToList() };
            JObject parameters = JObject.FromObject(collection);
            MethodPacket packet = new MethodPacket() { method = "createGroups", parameters = parameters };
            ReplyPacket reply = await this.SendAndListen(packet);
            return this.VerifyNoErrors(reply);
        }

        public async Task<InteractiveGroupCollectionModel> GetGroups()
        {
            MethodPacket packet = new MethodPacket() { method = "getGroups" };
            ReplyPacket reply = await this.SendAndListen(packet);
            return this.GetSpecificReplyResultValue<InteractiveGroupCollectionModel>(reply);
        }

        public async Task<InteractiveGroupCollectionModel> UpdateGroups(IEnumerable<InteractiveGroupModel> groups)
        {
            InteractiveGroupCollectionModel collection = new InteractiveGroupCollectionModel() { groups = groups.ToList() };
            JObject parameters = JObject.FromObject(collection);
            MethodPacket packet = new MethodPacket() { method = "updateGroups", parameters = parameters };
            ReplyPacket reply = await this.SendAndListen(packet);
            return this.GetSpecificReplyResultValue<InteractiveGroupCollectionModel>(reply);
        }

        public async Task<bool> DeleteGroup(InteractiveGroupModel groupToDelete, InteractiveGroupModel groupToReplace)
        {
            JObject parameters = new JObject();
            parameters.Add("groupID", groupToDelete.groupID);
            parameters.Add("reassignGroupID", groupToReplace.groupID);
            MethodPacket packet = new MethodPacket() { method = "deleteGroup", parameters = parameters };
            ReplyPacket reply = await this.SendAndListen(packet);
            return this.VerifyNoErrors(reply);
        }

        public async Task<InteractiveConnectedSceneCollectionModel> CreateScenes(IEnumerable<InteractiveConnectedSceneModel> scenes)
        {
            InteractiveConnectedSceneCollectionModel collection = new InteractiveConnectedSceneCollectionModel();
            foreach (InteractiveConnectedSceneModel scene in scenes)
            {
                // Need to strip out all of the non-updateable fields in order for the API to not return a 403 error
                collection.scenes.Add(JsonHelper.ConvertToDifferentType<InteractiveConnectedSceneModel>(scene));
            }

            JObject parameters = JObject.FromObject(collection);
            MethodPacket packet = new MethodPacket() { method = "createScenes", parameters = parameters };
            ReplyPacket reply = await this.SendAndListen(packet);
            return this.GetSpecificReplyResultValue<InteractiveConnectedSceneCollectionModel>(reply);
        }

        public async Task<InteractiveConnectedSceneGroupCollectionModel> GetScenes()
        {
            MethodPacket packet = new MethodPacket() { method = "getScenes" };
            ReplyPacket reply = await this.SendAndListen(packet);
            return this.GetSpecificReplyResultValue<InteractiveConnectedSceneGroupCollectionModel>(reply);
        }

        public async Task<InteractiveConnectedSceneCollectionModel> UpdateScenes(IEnumerable<InteractiveConnectedSceneModel> scenes)
        {
            InteractiveConnectedSceneCollectionModel collection = new InteractiveConnectedSceneCollectionModel();
            foreach (InteractiveConnectedSceneModel scene in scenes)
            {
                // Need to strip out all of the non-updateable fields in order for the API to not return a 403 error
                collection.scenes.Add(JsonHelper.ConvertToDifferentType<InteractiveConnectedSceneModel>(scene));
            }

            JObject parameters = JObject.FromObject(collection);
            MethodPacket packet = new MethodPacket() { method = "updateScenes", parameters = parameters };
            ReplyPacket reply = await this.SendAndListen(packet);
            return this.GetSpecificReplyResultValue<InteractiveConnectedSceneCollectionModel>(reply);
        }

        public async Task<bool> DeleteScene(InteractiveConnectedSceneModel sceneToDelete, InteractiveConnectedSceneModel sceneToReplace)
        {
            JObject parameters = new JObject();
            parameters.Add("sceneID", sceneToDelete.sceneID);
            parameters.Add("reassignSceneID", sceneToReplace.sceneID);
            MethodPacket packet = new MethodPacket() { method = "deleteScene", parameters = parameters };
            ReplyPacket reply = await this.SendAndListen(packet);
            return this.VerifyNoErrors(reply);
        }

        public async Task<bool> CreateControls(InteractiveConnectedSceneModel scene, IEnumerable<InteractiveControlModel> controls)
        {
            JObject parameters = new JObject();
            parameters.Add("sceneID", scene.sceneID);
            parameters.Add("controls", JArray.FromObject(controls));
            MethodPacket packet = new MethodPacket() { method = "createControls", parameters = parameters };
            ReplyPacket reply = await this.SendAndListen(packet);
            return this.VerifyNoErrors(reply);
        }

        public async Task<InteractiveConnectedControlCollectionModel> UpdateControls(InteractiveConnectedSceneModel scene, IEnumerable<InteractiveControlModel> controls)
        {
            JObject parameters = new JObject();
            parameters.Add("sceneID", scene.sceneID);
            parameters.Add("controls", JArray.FromObject(controls));
            MethodPacket packet = new MethodPacket() { method = "updateControls", parameters = parameters };
            ReplyPacket reply = await this.SendAndListen(packet);
            return this.GetSpecificReplyResultValue<InteractiveConnectedControlCollectionModel>(reply);
        }

        public async Task<bool> DeleteControls(InteractiveConnectedSceneModel scene, IEnumerable<InteractiveControlModel> controls)
        {
            JObject parameters = new JObject();
            parameters.Add("sceneID", scene.sceneID);
            parameters.Add("controlIDs", JArray.FromObject(controls.Select(c => c.controlID)));
            MethodPacket packet = new MethodPacket() { method = "deleteControls", parameters = parameters };
            ReplyPacket reply = await this.SendAndListen(packet);
            return this.VerifyNoErrors(reply);
        }

        public async Task<bool> CaptureSparkTransaction(string transactionID)
        {
            JObject parameters = new JObject();
            parameters.Add("transactionID", transactionID);
            MethodPacket packet = new MethodPacket() { method = "capture", parameters = parameters };
            ReplyPacket reply = await this.SendAndListen(packet);
            return this.VerifyNoErrors(reply);
        }

        private void InteractiveClient_OnMethodOccurred(object sender, MethodPacket methodPacket)
        {
            switch (methodPacket.method)
            {
                case "issueMemoryWarning":
                    this.SendSpecificMethod(methodPacket, this.OnIssueMemoryWarning);
                    break;

                case "onParticipantLeave":
                    this.SendSpecificMethod(methodPacket, this.OnParticipantLeave);
                    break;
                case "onParticipantJoin":
                    this.SendSpecificMethod(methodPacket, this.OnParticipantJoin);
                    break;
                case "onParticipantUpdate":
                    this.SendSpecificMethod(methodPacket, this.OnParticipantUpdate);
                    break;

                case "onGroupCreate":
                    this.SendSpecificMethod(methodPacket, this.OnGroupCreate);
                    break;
                case "onGroupDelete":
                    if (this.OnGroupDelete != null)
                    {
                        Tuple<InteractiveGroupModel, InteractiveGroupModel> groupDeleted = new Tuple<InteractiveGroupModel, InteractiveGroupModel>(
                            new InteractiveGroupModel() { groupID = methodPacket.parameters["groupID"].ToString() },
                            new InteractiveGroupModel() { groupID = methodPacket.parameters["reassignGroupID"].ToString() });

                        this.OnGroupDelete(this, groupDeleted);
                    }
                    break;
                case "onGroupUpdate":
                    this.SendSpecificMethod(methodPacket, this.OnGroupUpdate);
                    break;

                case "onSceneCreate":
                    this.SendSpecificMethod(methodPacket, this.OnSceneCreate);
                    break;
                case "onSceneDelete":
                    if (this.OnSceneDelete != null)
                    {
                        Tuple<InteractiveConnectedSceneModel, InteractiveConnectedSceneModel> sceneDeleted = new Tuple<InteractiveConnectedSceneModel, InteractiveConnectedSceneModel>(
                            new InteractiveConnectedSceneModel() { sceneID = methodPacket.parameters["sceneID"].ToString() },
                            new InteractiveConnectedSceneModel() { sceneID = methodPacket.parameters["reassignSceneID"].ToString() });

                        this.OnSceneDelete(this, sceneDeleted);
                    }
                    break;
                case "onSceneUpdate":
                    this.SendSpecificMethod(methodPacket, this.OnSceneUpdate);
                    break;

                case "onControlCreate":
                    this.SendSpecificMethod(methodPacket, this.OnControlCreate);
                    break;
                case "onControlDelete":
                    this.SendSpecificMethod(methodPacket, this.OnControlDelete);
                    break;
                case "onControlUpdate":
                    this.SendSpecificMethod(methodPacket, this.OnControlUpdate);
                    break;

                case "giveInput":
                    this.SendSpecificMethod(methodPacket, this.OnGiveInput);
                    break;
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
