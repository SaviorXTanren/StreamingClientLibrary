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
    /// <summary>
    /// The real-time client for Interactive interactions.
    /// </summary>
    public class InteractiveClient : MixerWebSocketClientBase
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
        public InteractiveGameModel InteractiveGame { get; private set; }
        public InteractiveGameVersionModel Version { get; private set; }
        public string ShareCode { get; private set; }

        private IEnumerable<string> interactiveConnections;

        private string oauthAccessToken;

        private int lastSequenceNumber = 0;

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
            Validator.ValidateList(interactiveGame.versions, "interactiveGame.versions");

            return await InteractiveClient.CreateFromChannel(connection, channel, interactiveGame, interactiveGame.versions.OrderByDescending(v => v.versionOrder).First());
        }

        /// <summary>
        /// Creates an interactive client using the specified connection to the specified channel and game.
        /// </summary>
        /// <param name="connection">The connection to use</param>
        /// <param name="channel">The channel to connect to</param>
        /// <param name="interactiveGame">The game to use</param>
        /// <param name="version">The version of the game to use</param>
        /// <returns>The interactive client for the specified channel and game</returns>
        public static async Task<InteractiveClient> CreateFromChannel(MixerConnection connection, ChannelModel channel, InteractiveGameModel interactiveGame, InteractiveGameVersionModel version)
        {
            Validator.ValidateVariable(connection, "connection");
            Validator.ValidateVariable(channel, "channel");
            Validator.ValidateVariable(interactiveGame, "interactiveGame");
            Validator.ValidateVariable(version, "version");

            return await InteractiveClient.CreateFromChannel(connection, channel, interactiveGame, version, null);
        }

        /// <summary>
        /// Creates an interactive client using the specified connection to the specified channel and game.
        /// </summary>
        /// <param name="connection">The connection to use</param>
        /// <param name="channel">The channel to connect to</param>
        /// <param name="interactiveGame">The game to use</param>
        /// <param name="version">The version of the game to use</param>
        /// <param name="shareCode">The share code used to connect to a game shared with you</param>
        /// <returns>The interactive client for the specified channel and game</returns>
        public static async Task<InteractiveClient> CreateFromChannel(MixerConnection connection, ChannelModel channel, InteractiveGameModel interactiveGame, InteractiveGameVersionModel version, string shareCode)
        {
            Validator.ValidateVariable(connection, "connection");
            Validator.ValidateVariable(channel, "channel");
            Validator.ValidateVariable(interactiveGame, "interactiveGame");
            Validator.ValidateVariable(version, "version");

            OAuthTokenModel authToken = await connection.GetOAuthToken();

            IEnumerable<string> interactiveConnections = await connection.Interactive.GetInteractiveHosts();

            return new InteractiveClient(channel, interactiveGame, version, shareCode, authToken, interactiveConnections);
        }

        private InteractiveClient(ChannelModel channel, InteractiveGameModel interactiveGame, InteractiveGameVersionModel version, string shareCode, OAuthTokenModel authToken, IEnumerable<string> interactiveConnections)
        {
            Validator.ValidateVariable(channel, "channel");
            Validator.ValidateVariable(interactiveGame, "interactiveGame");
            Validator.ValidateVariable(version, "version");
            Validator.ValidateVariable(authToken, "authToken");
            Validator.ValidateList(interactiveConnections, "interactiveConnections");

            this.Channel = channel;
            this.InteractiveGame = interactiveGame;
            this.Version = version;
            this.ShareCode = shareCode;
            this.interactiveConnections = interactiveConnections;

            this.oauthAccessToken = authToken.accessToken;
        }

        /// <summary>
        /// Connects to the channel and game.
        /// </summary>
        /// <returns>Whether the operation succeeded</returns>
        public async Task<bool> Connect()
        {
            Random random = new Random();
            int endpointToUse = random.Next(this.interactiveConnections.Count());
            return await this.Connect(this.interactiveConnections.ElementAt(endpointToUse));
        }

        /// <summary>
        /// Connects to the Constellation service.
        /// </summary>
        /// <param name="endpoint">The endpoint to connect to</param>
        /// <returns>Whether the operation succeeded</returns>
        public override async Task<bool> Connect(string endpoint)
        {
            this.OnMethodOccurred -= InteractiveClient_OnMethodOccurred;

            this.OnMethodOccurred += InteractiveClient_HelloMethodHandler;

            await base.Connect(endpoint);

            await this.WaitForResponse(() => { return this.Connected; });

            this.OnMethodOccurred -= InteractiveClient_HelloMethodHandler;

            if (this.Connected)
            {
                this.OnMethodOccurred += InteractiveClient_OnMethodOccurred;
                this.OnReplyOccurred += InteractiveClient_OnReplyOccurred;
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

            return this.Authenticated;
        }

        /// <summary>
        /// Gets the current server time.
        /// </summary>
        /// <returns>The current time on the server</returns>
        public async Task<DateTimeOffset?> GetTime()
        {
            ReplyPacket reply = await this.SendAndListen(new MethodPacket("getTime"));
            if (reply != null && reply.resultObject["time"] != null)
            {
                return DateTimeHelper.UnixTimestampToDateTimeOffset((long)reply.resultObject["time"]);
            }
            return null;
        }

        /// <summary>
        /// Gets the allocated memory state for this client.
        /// </summary>
        /// <returns>The allocated memory</returns>
        public async Task<InteractiveIssueMemoryWarningModel> GetMemoryStates()
        {
            return await this.SendAndListen<InteractiveIssueMemoryWarningModel>(new MethodPacket("getMemoryStats"));
        }

        /// <summary>
        /// Sets the memory throttling for the specified interactive APIs
        /// </summary>
        /// <param name="throttling">The throttling to set</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        public async Task SetBandwidthThrottle(InteractiveSetBandwidthThrottleModel throttling)
        {
            await this.Send(this.BuildBandwidthThrottlePacket(throttling));
        }

        /// <summary>
        /// Sets the memory throttling for the specified interactive APIs
        /// </summary>
        /// <param name="throttling">The throttling to set</param>
        /// <returns>Whether the operation succeeded</returns>
        public async Task<bool> SetBandwidthThrottleWithResponse(InteractiveSetBandwidthThrottleModel throttling)
        {
            return this.VerifyNoErrors(await this.SendAndListen(this.BuildBandwidthThrottlePacket(throttling)));
        }

        private MethodPacket BuildBandwidthThrottlePacket(InteractiveSetBandwidthThrottleModel throttling)
        {
            Validator.ValidateVariable(throttling, "throttling");
            return new MethodParamsPacket("setBandwidthThrottle", throttling);
        }

        /// <summary>
        /// Gets the memory throttling for all interactive APIs
        /// </summary>
        /// <returns>The memory throttling for all interactive APIs</returns>
        public async Task<InteractiveGetThrottleStateModel> GetThrottleState()
        {
            ReplyPacket reply = await this.SendAndListen(new MethodPacket("getThrottleState"));
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

            return await this.SendAndListen<InteractiveParticipantCollectionModel>(new MethodParamsPacket("getAllParticipants", parameters));
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

            return await this.SendAndListen<InteractiveParticipantCollectionModel>(new MethodParamsPacket("getActiveParticipants", parameters));
        }

        /// <summary>
        /// Updates the specified participants
        /// </summary>
        /// <param name="participants">The participants to update</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        public async Task UpdateParticipants(IEnumerable<InteractiveParticipantModel> participants)
        {
            await this.Send(this.BuildUpdateParticipantsPacket(participants));
        }

        /// <summary>
        /// Updates the specified participants
        /// </summary>
        /// <param name="participants">The participants to update</param>
        /// <returns>The updated participants</returns>
        public async Task<InteractiveParticipantCollectionModel> UpdateParticipantsWithResponse(IEnumerable<InteractiveParticipantModel> participants)
        {
            return await this.SendAndListen<InteractiveParticipantCollectionModel>(this.BuildUpdateParticipantsPacket(participants));
        }

        protected override ClientWebSocket CreateWebSocket()
        {
            ClientWebSocket webSocket = base.CreateWebSocket();

            AuthenticationHeaderValue authHeader = new AuthenticationHeaderValue("Bearer", this.oauthAccessToken);
            webSocket.Options.SetRequestHeader("Authorization", authHeader.ToString());
            webSocket.Options.SetRequestHeader("X-Interactive-Version", this.Version.id.ToString());
            webSocket.Options.SetRequestHeader("X-Protocol-Version", "2.0");
            if (!string.IsNullOrEmpty(this.ShareCode))
            {
                webSocket.Options.SetRequestHeader("X-Interactive-Sharecode", this.ShareCode);
            }

            return webSocket;
        }

        private MethodPacket BuildUpdateParticipantsPacket(IEnumerable<InteractiveParticipantModel> participants)
        {
            Validator.ValidateList(participants, "participants");
            JObject parameters = new JObject();
            parameters.Add("participants", JArray.FromObject(participants));
            return new MethodParamsPacket("updateParticipants", parameters);
        }

        /// <summary>
        /// Creates the specified groups
        /// </summary>
        /// <param name="groups">The groups to create</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        public async Task CreateGroups(IEnumerable<InteractiveGroupModel> groups)
        {
            await this.Send(this.BuildCreateGroupsPacket(groups));
        }

        /// <summary>
        /// Creates the specified groups
        /// </summary>
        /// <param name="groups">The groups to create</param>
        /// <returns>Whether the operation succeeded</returns>
        public async Task<bool> CreateGroupsWithResponse(IEnumerable<InteractiveGroupModel> groups)
        {
            return this.VerifyNoErrors(await this.SendAndListen(this.BuildCreateGroupsPacket(groups)));
        }

        private MethodPacket BuildCreateGroupsPacket(IEnumerable<InteractiveGroupModel> groups)
        {
            Validator.ValidateList(groups, "groups");
            InteractiveGroupCollectionModel collection = new InteractiveGroupCollectionModel() { groups = groups.ToList() };
            return new MethodParamsPacket("createGroups", JObject.FromObject(collection));
        }

        /// <summary>
        /// Gets all groups.
        /// </summary>
        /// <returns>All groups</returns>
        public async Task<InteractiveGroupCollectionModel> GetGroups()
        {
            return await this.SendAndListen<InteractiveGroupCollectionModel>(new MethodPacket("getGroups"));
        }

        /// <summary>
        /// Updates the specified groups.
        /// </summary>
        /// <param name="groups">The groups to update</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        public async Task UpdateGroups(IEnumerable<InteractiveGroupModel> groups)
        {
            await this.Send(this.BuildUpdateGroupsPacket(groups));
        }

        /// <summary>
        /// Updates the specified groups.
        /// </summary>
        /// <param name="groups">The groups to update</param>
        /// <returns>The updated groups</returns>
        public async Task<InteractiveGroupCollectionModel> UpdateGroupsWithResponse(IEnumerable<InteractiveGroupModel> groups)
        {
            return await this.SendAndListen<InteractiveGroupCollectionModel>(this.BuildUpdateGroupsPacket(groups));
        }

        private MethodPacket BuildUpdateGroupsPacket(IEnumerable<InteractiveGroupModel> groups)
        {
            Validator.ValidateList(groups, "groups");
            InteractiveGroupCollectionModel collection = new InteractiveGroupCollectionModel() { groups = groups.ToList() };
            return new MethodParamsPacket("updateGroups", JObject.FromObject(collection));
        }

        /// <summary>
        /// Deletes and replaces the specified group
        /// </summary>
        /// <param name="groupToDelete">The group to delete</param>
        /// <param name="groupToReplace">The group to replace with</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        public async Task DeleteGroup(InteractiveGroupModel groupToDelete, InteractiveGroupModel groupToReplace)
        {
            await this.Send(this.BuildDeleteGroupPacket(groupToDelete, groupToReplace));
        }

        /// <summary>
        /// Deletes and replaces the specified group
        /// </summary>
        /// <param name="groupToDelete">The group to delete</param>
        /// <param name="groupToReplace">The group to replace with</param>
        /// <returns>Whether the operation succeeded</returns>
        public async Task<bool> DeleteGroupWithResponse(InteractiveGroupModel groupToDelete, InteractiveGroupModel groupToReplace)
        {
            return this.VerifyNoErrors(await this.SendAndListen(this.BuildDeleteGroupPacket(groupToDelete, groupToReplace)));
        }

        private MethodPacket BuildDeleteGroupPacket(InteractiveGroupModel groupToDelete, InteractiveGroupModel groupToReplace)
        {
            Validator.ValidateVariable(groupToDelete, "groupToDelete");
            Validator.ValidateVariable(groupToReplace, "groupToReplace");
            JObject parameters = new JObject();
            parameters.Add("groupID", groupToDelete.groupID);
            parameters.Add("reassignGroupID", groupToReplace.groupID);
            return new MethodParamsPacket("deleteGroup", parameters);
        }

        /// <summary>
        /// Creates the specified scenes.
        /// </summary>
        /// <param name="scenes">The scenes to create</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        public async Task CreateScenes(IEnumerable<InteractiveConnectedSceneModel> scenes)
        {
            await this.Send(this.BuildCreateScenesPacket(scenes));
        }

        /// <summary>
        /// Creates the specified scenes.
        /// </summary>
        /// <param name="scenes">The scenes to create</param>
        /// <returns>The created scenes</returns>
        public async Task<InteractiveConnectedSceneCollectionModel> CreateScenesWithResponse(IEnumerable<InteractiveConnectedSceneModel> scenes)
        {
            return await this.SendAndListen<InteractiveConnectedSceneCollectionModel>(this.BuildCreateScenesPacket(scenes));
        }

        private MethodPacket BuildCreateScenesPacket(IEnumerable<InteractiveConnectedSceneModel> scenes)
        {
            Validator.ValidateList(scenes, "scenes");
            InteractiveConnectedSceneCollectionModel collection = new InteractiveConnectedSceneCollectionModel();
            foreach (InteractiveConnectedSceneModel scene in scenes)
            {
                // Need to strip out all of the non-updateable fields in order for the API to not return a 403 error
                collection.scenes.Add(JsonHelper.ConvertToDifferentType<InteractiveConnectedSceneModel>(scene));
            }
            return new MethodParamsPacket("createScenes", JObject.FromObject(collection));
        }

        /// <summary>
        /// Gets all scenes.
        /// </summary>
        /// <returns>All scenes</returns>
        public async Task<InteractiveConnectedSceneGroupCollectionModel> GetScenes()
        {
            return await this.SendAndListen<InteractiveConnectedSceneGroupCollectionModel>(new MethodPacket("getScenes"));
        }

        /// <summary>
        /// Updates the specified scenes.
        /// </summary>
        /// <param name="scenes">The scenes to update</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        public async Task UpdateScenes(IEnumerable<InteractiveConnectedSceneModel> scenes)
        {
            await this.Send(this.BuildUpdateScenesPacket(scenes));
        }

        /// <summary>
        /// Updates the specified scenes.
        /// </summary>
        /// <param name="scenes">The scenes to update</param>
        /// <returns>The updated scenes</returns>
        public async Task<InteractiveConnectedSceneCollectionModel> UpdateScenesWithResponse(IEnumerable<InteractiveConnectedSceneModel> scenes)
        {
            return await this.SendAndListen<InteractiveConnectedSceneCollectionModel>(this.BuildUpdateScenesPacket(scenes));
        }

        private MethodPacket BuildUpdateScenesPacket(IEnumerable<InteractiveConnectedSceneModel> scenes)
        {
            Validator.ValidateList(scenes, "scenes");
            InteractiveConnectedSceneCollectionModel collection = new InteractiveConnectedSceneCollectionModel();
            foreach (InteractiveConnectedSceneModel scene in scenes)
            {
                // Need to strip out all of the non-updateable fields in order for the API to not return a 403 error
                collection.scenes.Add(JsonHelper.ConvertToDifferentType<InteractiveConnectedSceneModel>(scene));
            }
            return new MethodParamsPacket("updateScenes", JObject.FromObject(collection));
        }

        /// <summary>
        /// Deletes and replaced the specified scene.
        /// </summary>
        /// <param name="sceneToDelete">The scene to delete</param>
        /// <param name="sceneToReplace">The scene to replace with</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        public async Task DeleteScene(InteractiveConnectedSceneModel sceneToDelete, InteractiveConnectedSceneModel sceneToReplace)
        {
            await this.Send(this.BuildDeleteScenePacket(sceneToDelete, sceneToReplace));
        }

        /// <summary>
        /// Deletes and replaced the specified scene.
        /// </summary>
        /// <param name="sceneToDelete">The scene to delete</param>
        /// <param name="sceneToReplace">The scene to replace with</param>
        /// <returns>Whether the operation succeeded</returns>
        public async Task<bool> DeleteSceneWithResponse(InteractiveConnectedSceneModel sceneToDelete, InteractiveConnectedSceneModel sceneToReplace)
        {
            return this.VerifyNoErrors(await this.SendAndListen(this.BuildDeleteScenePacket(sceneToDelete, sceneToReplace)));
        }

        private MethodPacket BuildDeleteScenePacket(InteractiveConnectedSceneModel sceneToDelete, InteractiveConnectedSceneModel sceneToReplace)
        {
            Validator.ValidateVariable(sceneToDelete, "sceneToDelete");
            Validator.ValidateVariable(sceneToReplace, "sceneToReplace");
            JObject parameters = new JObject();
            parameters.Add("sceneID", sceneToDelete.sceneID);
            parameters.Add("reassignSceneID", sceneToReplace.sceneID);
            return new MethodParamsPacket("deleteScene", parameters);
        }

        /// <summary>
        /// Creates the specified controls for the specified scene.
        /// </summary>
        /// <param name="scene">The scene to add controls to</param>
        /// <param name="controls">The controls to create</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        public async Task CreateControls(InteractiveConnectedSceneModel scene, IEnumerable<InteractiveControlModel> controls)
        {
            await this.Send(this.BuildCreateControlsPacket(scene, controls));
        }

        /// <summary>
        /// Creates the specified controls for the specified scene.
        /// </summary>
        /// <param name="scene">The scene to add controls to</param>
        /// <param name="controls">The controls to create</param>
        /// <returns>Whether the operation succeed</returns>
        public async Task<bool> CreateControlsWithResponse(InteractiveConnectedSceneModel scene, IEnumerable<InteractiveControlModel> controls)
        {
            return this.VerifyNoErrors(await this.SendAndListen(this.BuildCreateControlsPacket(scene, controls)));
        }

        private MethodPacket BuildCreateControlsPacket(InteractiveConnectedSceneModel scene, IEnumerable<InteractiveControlModel> controls)
        {
            Validator.ValidateVariable(scene, "scene");
            Validator.ValidateList(controls, "controls");
            JObject parameters = new JObject();
            parameters.Add("sceneID", scene.sceneID);
            parameters.Add("controls", JArray.FromObject(controls));
            return new MethodParamsPacket("createControls", parameters);
        }

        /// <summary>
        /// Updates the specified controls for the specified scene.
        /// </summary>
        /// <param name="scene">The scene to update controls for</param>
        /// <param name="controls">The controls to update</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        public async Task UpdateControls(InteractiveConnectedSceneModel scene, IEnumerable<InteractiveControlModel> controls)
        {
            await this.Send(this.BuildUpdateControlsPacket(scene, controls));
        }

        /// <summary>
        /// Updates the specified controls for the specified scene.
        /// </summary>
        /// <param name="scene">The scene to update controls for</param>
        /// <param name="controls">The controls to update</param>
        /// <returns>The updated controls</returns>
        public async Task<InteractiveConnectedControlCollectionModel> UpdateControlsWithResponse(InteractiveConnectedSceneModel scene, IEnumerable<InteractiveControlModel> controls)
        {
            return await this.SendAndListen<InteractiveConnectedControlCollectionModel>(this.BuildUpdateControlsPacket(scene, controls));
        }

        private MethodPacket BuildUpdateControlsPacket(InteractiveConnectedSceneModel scene, IEnumerable<InteractiveControlModel> controls)
        {
            Validator.ValidateVariable(scene, "scene");
            Validator.ValidateList(controls, "controls");
            JObject parameters = new JObject();
            parameters.Add("sceneID", scene.sceneID);
            parameters.Add("controls", JArray.FromObject(controls));
            return new MethodParamsPacket("updateControls", parameters);
        }

        /// <summary>
        /// Deletes the specified controls from the specified scene.
        /// </summary>
        /// <param name="scene">The scene to delete controls from</param>
        /// <param name="controls">The controls to delete</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        public async Task DeleteControls(InteractiveConnectedSceneModel scene, IEnumerable<InteractiveControlModel> controls)
        {
            await this.Send(this.BuildDeleteControlsPacket(scene, controls));
        }

        /// <summary>
        /// Deletes the specified controls from the specified scene.
        /// </summary>
        /// <param name="scene">The scene to delete controls from</param>
        /// <param name="controls">The controls to delete</param>
        /// <returns>Whether the operation succeeded</returns>
        public async Task<bool> DeleteControlsWithResponse(InteractiveConnectedSceneModel scene, IEnumerable<InteractiveControlModel> controls)
        {
            return this.VerifyNoErrors(await this.SendAndListen(this.BuildDeleteControlsPacket(scene, controls)));
        }

        private MethodPacket BuildDeleteControlsPacket(InteractiveConnectedSceneModel scene, IEnumerable<InteractiveControlModel> controls)
        {
            Validator.ValidateVariable(scene, "scene");
            Validator.ValidateList(controls, "controls");
            JObject parameters = new JObject();
            parameters.Add("sceneID", scene.sceneID);
            parameters.Add("controlIDs", JArray.FromObject(controls.Select(c => c.controlID)));
            return new MethodParamsPacket("deleteControls", parameters);
        }

        /// <summary>
        /// Captures the spark transaction for the specified id.
        /// </summary>
        /// <param name="transactionID">The id of the spark transaction</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        public async Task CaptureSparkTransaction(string transactionID)
        {
            await this.Send(this.BuildCaptureSparkTransactionPacket(transactionID));
        }

        /// <summary>
        /// Captures the spark transaction for the specified id.
        /// </summary>
        /// <param name="transactionID">The id of the spark transaction</param>
        /// <returns>Whether the operation succeeded</returns>
        public async Task<bool> CaptureSparkTransactionWithResponse(string transactionID)
        {
            return this.VerifyNoErrors(await this.SendAndListen(this.BuildCaptureSparkTransactionPacket(transactionID)));
        }

        protected async override Task<uint> Send(WebSocketPacket packet, bool checkIfAuthenticated = true)
        {
            this.AssignLatestSequence(packet);
            return await base.Send(packet, checkIfAuthenticated);
        }

        private void AssignLatestSequence(WebSocketPacket packet)
        {
            if (packet is MethodPacket)
            {
                MethodPacket mPacket = (MethodPacket)packet;
                mPacket.seq = this.lastSequenceNumber;
            }
            else if (packet is ReplyPacket)
            {
                ReplyPacket rPacket = (ReplyPacket)packet;
                rPacket.seq = this.lastSequenceNumber;
            }
        }

        private MethodPacket BuildCaptureSparkTransactionPacket(string transactionID)
        {
            Validator.ValidateString(transactionID, "transactionID");
            JObject parameters = new JObject();
            parameters.Add("transactionID", transactionID);
            return new MethodParamsPacket("capture", parameters);
        }

        private void InteractiveClient_OnMethodOccurred(object sender, MethodPacket methodPacket)
        {
            this.lastSequenceNumber = Math.Max(methodPacket.seq, this.lastSequenceNumber);

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

        private void InteractiveClient_OnReplyOccurred(object sender, ReplyPacket e)
        {
            this.lastSequenceNumber = Math.Max(e.seq, this.lastSequenceNumber);
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
