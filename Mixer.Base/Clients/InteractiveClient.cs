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
using System.Threading.Tasks;

namespace Mixer.Base.Clients
{
    /// <summary>
    /// The real-time client for Interactive interactions.
    /// </summary>
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

        /// <summary>
        /// Creates an interactive client using the specified connection to the specified channel and game.
        /// </summary>
        /// <param name="connection">The connection to use</param>
        /// <param name="channel">The channel to connect to</param>
        /// <param name="interactiveGame">The game to use</param>
        /// <returns>The interactive client for the specified channel and game</returns>
        public static async Task<InteractiveClient> CreateFromChannel(MixerConnection connection, ChannelModel channel, InteractiveGameListingModel interactiveGame)
        {
            Validator.ValidateVariable(connection, "connection");
            Validator.ValidateVariable(channel, "channel");
            Validator.ValidateVariable(interactiveGame, "interactiveGame");

            OAuthTokenModel authToken = await connection.GetOAuthToken();

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

        /// <summary>
        /// Connects to the channel and game.
        /// </summary>
        /// <returns>Whether the operation succeeded</returns>
        public async Task<bool> Connect()
        {
            this.OnMethodOccurred -= InteractiveClient_OnMethodOccurred;

            int totalEndpoints = this.interactiveConnections.Count();
            Random random = new Random();
            int endpointToUse = random.Next() % totalEndpoints;

            this.OnMethodOccurred += InteractiveClient_HelloMethodHandler;

            await this.ConnectInternal(this.interactiveConnections.ElementAt(endpointToUse));

            await this.WaitForResponse(() => { return this.Connected; });

            this.OnMethodOccurred -= InteractiveClient_HelloMethodHandler;

            if (this.Connected)
            {
                this.OnMethodOccurred += InteractiveClient_OnMethodOccurred;
            }

            return this.Connected;
        }

        /// <summary>
        /// Prepares the client to receive interactive events.
        /// </summary>
        /// <returns>Whether the operation succeeded</returns>
        public async Task<bool> Ready()
        {
            this.Authenticated = false;

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

            await this.WaitForResponse(() => { return this.Authenticated; });

            this.OnMethodOccurred -= InteractiveClient_ReadyMethodHandler;

            if (this.Authenticated)
            {
                this.StartBackgroundPing();
            }

            return this.Authenticated;
        }

        /// <summary>
        /// Gets the current server time.
        /// </summary>
        /// <returns>The current time on the server</returns>
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

        /// <summary>
        /// Gets the allocated memory state for this client.
        /// </summary>
        /// <returns>The allocated memory<returns>
        public async Task<InteractiveIssueMemoryWarningModel> GetMemoryStates()
        {
            MethodPacket packet = new MethodPacket() { method = "getMemoryStats" };
            ReplyPacket reply = await this.SendAndListen(packet);
            return this.GetSpecificReplyResultValue<InteractiveIssueMemoryWarningModel>(reply);
        }

        /// <summary>
        /// Sets the memory throttling for the specified interactive APIs
        /// </summary>
        /// <param name="throttling">The throttling to set</param>
        /// <returns>Whether the operation succeeded</returns>
        public async Task<bool> SetBandwidthThrottle(InteractiveSetBandwidthThrottleModel throttling)
        {
            Validator.ValidateVariable(throttling, "throttling");

            MethodPacket packet = new MethodPacket() { method = "setBandwidthThrottle", parameters = throttling };
            ReplyPacket reply = await this.SendAndListen(packet);
            return this.VerifyNoErrors(reply);
        }

        /// <summary>
        /// Gets the memory throttling for all interactive APIs
        /// </summary>
        /// <returns>The memory throttling for all interactive APIs</returns>
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

        /// <summary>
        /// Gets the first 100 participants connected based on the specified connection time. For subsequent participants, specify
        /// a later connection time, typically using the latest connected participant's connection time as the new earliest connection time.
        /// </summary>
        /// <param name="startTime">The starting connection time</param>
        /// <returns>The first 100 participants</returns>
        public async Task<InteractiveParticipantCollectionModel> GetAllParticipants(DateTimeOffset? startTime = null)
        {
            if (startTime == null) { startTime = DateTimeOffset.FromUnixTimeSeconds(0); }

            JObject parameters = new JObject();
            parameters.Add("from", DateTimeHelper.DateTimeOffsetToUnixTimestamp(startTime.GetValueOrDefault()));
            MethodPacket packet = new MethodPacket() { method = "getAllParticipants", parameters = parameters };
            ReplyPacket reply = await this.SendAndListen(packet);
            return this.GetSpecificReplyResultValue<InteractiveParticipantCollectionModel>(reply);
        }

        /// <summary>
        /// Returns a set of participants who have performed an interactive action after the specified start. For subsequent participants, specify
        /// a later connection time, typically using the latest connected participant's connection time as the new earliest connection time.
        /// </summary>
        /// <param name="startTime">The start time for last interaction</param>
        /// <returns>The set of last interacted participants</returns>
        public async Task<InteractiveParticipantCollectionModel> GetActiveParticipants(DateTimeOffset startTime)
        {
            Validator.ValidateVariable(startTime, "startTime");

            JObject parameters = new JObject();
            parameters.Add("threshold", DateTimeHelper.DateTimeOffsetToUnixTimestamp(startTime));
            MethodPacket packet = new MethodPacket() { method = "getActiveParticipants", parameters = parameters };
            ReplyPacket reply = await this.SendAndListen(packet);
            return this.GetSpecificReplyResultValue<InteractiveParticipantCollectionModel>(reply);
        }

        /// <summary>
        /// Updates the specified participants
        /// </summary>
        /// <param name="participants">The participants to update</param>
        /// <returns>The updated participants</returns>
        public async Task<InteractiveParticipantCollectionModel> UpdateParticipants(IEnumerable<InteractiveParticipantModel> participants)
        {
            Validator.ValidateList(participants, "participants");

            JObject parameters = new JObject();
            parameters.Add("participants", JArray.FromObject(participants));
            MethodPacket packet = new MethodPacket() { method = "updateParticipants", parameters = parameters };
            ReplyPacket reply = await this.SendAndListen(packet);
            return this.GetSpecificReplyResultValue<InteractiveParticipantCollectionModel>(reply);
        }

        /// <summary>
        /// Creates the specified groups
        /// </summary>
        /// <param name="groups">The groups to create</param>
        /// <returns>Whether the operation succeeded</returns>
        public async Task<bool> CreateGroups(IEnumerable<InteractiveGroupModel> groups)
        {
            Validator.ValidateList(groups, "groups");

            InteractiveGroupCollectionModel collection = new InteractiveGroupCollectionModel() { groups = groups.ToList() };
            JObject parameters = JObject.FromObject(collection);
            MethodPacket packet = new MethodPacket() { method = "createGroups", parameters = parameters };
            ReplyPacket reply = await this.SendAndListen(packet);
            return this.VerifyNoErrors(reply);
        }

        /// <summary>
        /// Gets all groups.
        /// </summary>
        /// <returns>All groups</returns>
        public async Task<InteractiveGroupCollectionModel> GetGroups()
        {
            MethodPacket packet = new MethodPacket() { method = "getGroups" };
            ReplyPacket reply = await this.SendAndListen(packet);
            return this.GetSpecificReplyResultValue<InteractiveGroupCollectionModel>(reply);
        }

        /// <summary>
        /// Updates the specified groups.
        /// </summary>
        /// <param name="groups">The groups to update</param>
        /// <returns>The updated groups</returns>
        public async Task<InteractiveGroupCollectionModel> UpdateGroups(IEnumerable<InteractiveGroupModel> groups)
        {
            Validator.ValidateList(groups, "groups");

            InteractiveGroupCollectionModel collection = new InteractiveGroupCollectionModel() { groups = groups.ToList() };
            JObject parameters = JObject.FromObject(collection);
            MethodPacket packet = new MethodPacket() { method = "updateGroups", parameters = parameters };
            ReplyPacket reply = await this.SendAndListen(packet);
            return this.GetSpecificReplyResultValue<InteractiveGroupCollectionModel>(reply);
        }

        /// <summary>
        /// Deletes and replaces the specified group
        /// </summary>
        /// <param name="groupToDelete">The group to delete</param>
        /// <param name="groupToReplace">The group to replace with</param>
        /// <returns>Whether the operation succeeded</returns>
        public async Task<bool> DeleteGroup(InteractiveGroupModel groupToDelete, InteractiveGroupModel groupToReplace)
        {
            Validator.ValidateVariable(groupToDelete, "groupToDelete");
            Validator.ValidateVariable(groupToReplace, "groupToReplace");

            JObject parameters = new JObject();
            parameters.Add("groupID", groupToDelete.groupID);
            parameters.Add("reassignGroupID", groupToReplace.groupID);
            MethodPacket packet = new MethodPacket() { method = "deleteGroup", parameters = parameters };
            ReplyPacket reply = await this.SendAndListen(packet);
            return this.VerifyNoErrors(reply);
        }

        /// <summary>
        /// Creates the specified scenes.
        /// </summary>
        /// <param name="scenes">The scenes to create</param>
        /// <returns>The created scenes</returns>
        public async Task<InteractiveConnectedSceneCollectionModel> CreateScenes(IEnumerable<InteractiveConnectedSceneModel> scenes)
        {
            Validator.ValidateList(scenes, "scenes");

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

        /// <summary>
        /// Gets all scenes.
        /// </summary>
        /// <returns>All scenes</returns>
        public async Task<InteractiveConnectedSceneGroupCollectionModel> GetScenes()
        {
            MethodPacket packet = new MethodPacket() { method = "getScenes" };
            ReplyPacket reply = await this.SendAndListen(packet);
            return this.GetSpecificReplyResultValue<InteractiveConnectedSceneGroupCollectionModel>(reply);
        }

        /// <summary>
        /// Updates the specified scenes.
        /// </summary>
        /// <param name="scenes">The scenes to update</param>
        /// <returns>The updated scenes</returns>
        public async Task<InteractiveConnectedSceneCollectionModel> UpdateScenes(IEnumerable<InteractiveConnectedSceneModel> scenes)
        {
            Validator.ValidateList(scenes, "scenes");

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

        /// <summary>
        /// Deletes and replaced the specified scene.
        /// </summary>
        /// <param name="sceneToDelete">The scene to delete</param>
        /// <param name="sceneToReplace">The scene to replace with</param>
        /// <returns>Whether the operation succeeded</returns>
        public async Task<bool> DeleteScene(InteractiveConnectedSceneModel sceneToDelete, InteractiveConnectedSceneModel sceneToReplace)
        {
            Validator.ValidateVariable(sceneToDelete, "sceneToDelete");
            Validator.ValidateVariable(sceneToReplace, "sceneToReplace");

            JObject parameters = new JObject();
            parameters.Add("sceneID", sceneToDelete.sceneID);
            parameters.Add("reassignSceneID", sceneToReplace.sceneID);
            MethodPacket packet = new MethodPacket() { method = "deleteScene", parameters = parameters };
            ReplyPacket reply = await this.SendAndListen(packet);
            return this.VerifyNoErrors(reply);
        }

        /// <summary>
        /// Creates the specified controls for the specified scene.
        /// </summary>
        /// <param name="scene">The scene to add controls to</param>
        /// <param name="controls">The controls to create</param>
        /// <returns>Whether the operation succeed</returns>
        public async Task<bool> CreateControls(InteractiveConnectedSceneModel scene, IEnumerable<InteractiveControlModel> controls)
        {
            Validator.ValidateVariable(scene, "scene");
            Validator.ValidateList(controls, "controls");

            JObject parameters = new JObject();
            parameters.Add("sceneID", scene.sceneID);
            parameters.Add("controls", JArray.FromObject(controls));
            MethodPacket packet = new MethodPacket() { method = "createControls", parameters = parameters };
            ReplyPacket reply = await this.SendAndListen(packet);
            return this.VerifyNoErrors(reply);
        }

        /// <summary>
        /// Updates the specified controls for the specified scene.
        /// </summary>
        /// <param name="scene">The scene to update controls for</param>
        /// <param name="controls">The controls to update</param>
        /// <returns>The updated controls</returns>
        public async Task<InteractiveConnectedControlCollectionModel> UpdateControls(InteractiveConnectedSceneModel scene, IEnumerable<InteractiveControlModel> controls)
        {
            Validator.ValidateVariable(scene, "scene");
            Validator.ValidateList(controls, "controls");

            JObject parameters = new JObject();
            parameters.Add("sceneID", scene.sceneID);
            parameters.Add("controls", JArray.FromObject(controls));
            MethodPacket packet = new MethodPacket() { method = "updateControls", parameters = parameters };
            ReplyPacket reply = await this.SendAndListen(packet);
            return this.GetSpecificReplyResultValue<InteractiveConnectedControlCollectionModel>(reply);
        }

        /// <summary>
        /// Deletes the specified controls from the specified scene.
        /// </summary>
        /// <param name="scene">The scene to delete controls from</param>
        /// <param name="controls">The controls to delete</param>
        /// <returns>Whether the operation succeeded</returns>
        public async Task<bool> DeleteControls(InteractiveConnectedSceneModel scene, IEnumerable<InteractiveControlModel> controls)
        {
            Validator.ValidateVariable(scene, "scene");
            Validator.ValidateList(controls, "controls");

            JObject parameters = new JObject();
            parameters.Add("sceneID", scene.sceneID);
            parameters.Add("controlIDs", JArray.FromObject(controls.Select(c => c.controlID)));
            MethodPacket packet = new MethodPacket() { method = "deleteControls", parameters = parameters };
            ReplyPacket reply = await this.SendAndListen(packet);
            return this.VerifyNoErrors(reply);
        }

        /// <summary>
        /// Captures the spark transaction for the specified id.
        /// </summary>
        /// <param name="transactionID">The id of the spark transaction</param>
        /// <returns>Whether the operation succeeded</returns>
        public async Task<bool> CaptureSparkTransaction(string transactionID)
        {
            Validator.ValidateString(transactionID, "transactionID");

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

        protected override async Task<bool> KeepAlivePing()
        {
            DateTimeOffset? dateTime = await this.GetTime();
            if (dateTime != null)
            {
                return DateTimeOffset.Now.Date.Equals(dateTime.GetValueOrDefault().Date);
            }
            return false;
        }

        private void InteractiveClient_HelloMethodHandler(object sender, MethodPacket e)
        {
            if (e.method.Equals("hello"))
            {
                this.Connected = true;
            }
        }

        private void InteractiveClient_ReadyMethodHandler(object sender, MethodPacket e)
        {
            JToken value;
            if (e.method.Equals("onReady") && e.parameters.TryGetValue("isReady", out value) && (bool)value)
            {
                this.Authenticated = true;
            }
        }
    }
}
