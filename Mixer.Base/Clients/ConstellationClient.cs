using Mixer.Base.Model.Client;
using Mixer.Base.Model.Constellation;
using Mixer.Base.Model.OAuth;
using Mixer.Base.Util;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Mixer.Base.Clients
{
    public enum ConstellationEventTypeEnum
    {
        [Name("Announcements")]
        announcement__announce,

        [Name("Channel Followed")]
        channel__id__followed,
        [Name("Channel Hosted")]
        channel__id__hosted,
        [Name("Channel Unhosted")]
        channel__id__unhosted,
        [Name("Channel Status")]
        channel__id__status,
        [Name("Channel Subscribed")]
        channel__id__subscribed,
        [Name("Channel Resubscribed")]
        channel__id__resubscribed,
        [Name("Channel Resubscribed Shared")]
        channel__id__resubShared,
        [Name("Channel Updated")]
        channel__id__update,

        [Name("Team Deleted")]
        team__id__deleted,
        [Name("Team Member Accepted")]
        team__id__memberAccepted,
        [Name("Team Member Invited")]
        team__id__memberInvited,
        [Name("Team Member Removed")]
        team__id__memberRemoved,
        [Name("Team Owner Changed")]
        team__id__ownerChanged,

        [Name("User Achievement")]
        user__id__achievement,
        [Name("User Followed")]
        user__id__followed,
        [Name("User Notification")]
        user__id__notify,
        [Name("User Subscribed")]
        user__id__subscribed,
        [Name("User Resubscribed")]
        user__id__resubscribed,
        [Name("User Joined Team")]
        user__id__teamAccepted,
        [Name("User Invited To Team")]
        user__id__teamInvited,
        [Name("User Left Team")]
        user__id__teamRemoved,
        [Name("User Updated")]
        user__id__update,
    }

    /// <summary>
    /// The real-time client for Constellation event interactions.
    /// </summary>
    public class ConstellationEventType : IEquatable<ConstellationEventType>
    {
        public ConstellationEventTypeEnum Type { get; set; }
        public uint ID { get; set; }

        public ConstellationEventType(ConstellationEventTypeEnum type, uint id = 0)
        {
            this.Type = type;
            this.ID = id;
        }

        public override string ToString()
        {
            return ConstellationClient.ConvertEventTypesToStrings(new List<ConstellationEventType>() { this }).First();
        }

        public override int GetHashCode() { return this.Type.GetHashCode() + this.ID.GetHashCode(); }

        public override bool Equals(object obj)
        {
            if (obj is ConstellationEventType)
            {
                return this.Equals((ConstellationEventType)obj);
            }
            return false;
        }

        public bool Equals(ConstellationEventType other)
        {
            return this.Type.Equals(other.Type) && this.ID.Equals(other.ID);
        }
    }

    public class ConstellationClient : MixerWebSocketClientBase
    {
        private string oauthAccessToken;

        internal static IEnumerable<string> ConvertEventTypesToStrings(IEnumerable<ConstellationEventType> eventTypes)
        {
            List<string> stringEventTypes = new List<string>();
            foreach (ConstellationEventType eventType in eventTypes)
            {
                string eventName = eventType.Type.ToString();
                eventName = eventName.Replace("__", ":");
                eventName = eventName.Replace(":id:", string.Format(":{0}:", eventType.ID));
                stringEventTypes.Add(eventName);
            }
            return stringEventTypes;
        }

        public event EventHandler<ConstellationLiveEventModel> OnSubscribedEventOccurred;

        /// <summary>
        /// Creates a constellation event client using the specified connection.
        /// </summary>
        /// <param name="connection">The connection to use</param>
        /// <returns>The constellation event client</returns>
        public static async Task<ConstellationClient> Create(MixerConnection connection)
        {
            Validator.ValidateVariable(connection, "connection");

            OAuthTokenModel authToken = await connection.GetOAuthToken();

            return new ConstellationClient(authToken);
        }

        private ConstellationClient(OAuthTokenModel authToken)
        {
            Validator.ValidateVariable(authToken, "authToken");

            this.oauthAccessToken = authToken.accessToken;
        }

        /// <summary>
        /// Connects to the Constellation service.
        /// </summary>
        /// <returns>Whether the operation succeeded</returns>
        public async Task<bool> Connect(bool autoReconnect = true)
        {
            return await this.Connect("wss://constellation.mixer.com");
        }

        /// <summary>
        /// Connects to the Constellation service.
        /// </summary>
        /// <param name="endpoint">The endpoint to connect to</param>
        /// <returns>Whether the operation succeeded</returns>
        public override async Task<bool> Connect(string endpoint)
        {
            this.OnEventOccurred -= ConstellationClient_OnEventOccurred;

            this.OnEventOccurred += ConstellationClient_HelloMethodHandler;

            await base.Connect(endpoint);

            await this.WaitForResponse(() => { return this.Connected; });

            this.OnEventOccurred -= ConstellationClient_HelloMethodHandler;

            if (this.Connected)
            {
                this.OnEventOccurred += ConstellationClient_OnEventOccurred;
            }

            return this.Connected;
        }

        /// <summary>
        /// Pings the Constellation service.
        /// </summary>
        /// <returns>Whether the operation succeeded</returns>
        public async Task<bool> Ping()
        {
            return this.VerifyNoErrors(await this.SendAndListen(new MethodPacket("ping")));
        }

        /// <summary>
        /// Subscribes to the specified events.
        /// </summary>
        /// <param name="events">The events to subscribe to</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        public async Task SubscribeToEvents(IEnumerable<ConstellationEventType> events)
        {
            await this.Send(this.BuildSubscribeToEventsPacket(events));
        }

        /// <summary>
        /// Subscribes to the specified events.
        /// </summary>
        /// <param name="events">The events to subscribe to</param>
        /// <returns>Whether the operation succeeded</returns>
        public async Task<bool> SubscribeToEventsWithResponse(IEnumerable<ConstellationEventType> events)
        {
            return this.VerifyNoErrors(await this.SendAndListen(this.BuildSubscribeToEventsPacket(events)));
        }

        private MethodPacket BuildSubscribeToEventsPacket(IEnumerable<ConstellationEventType> events)
        {
            Validator.ValidateList(events, "events");
            IEnumerable<string> eventStrings = ConstellationClient.ConvertEventTypesToStrings(events);
            JObject parameters = new JObject();
            parameters.Add("events", new JArray(eventStrings));
            return new MethodParamsPacket("livesubscribe", parameters);
        }

        /// <summary>
        /// Unsubscribes from the specified events.
        /// </summary>
        /// <param name="events">The events to unsubscribe from</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        public async Task UnsubscribeToEvents(IEnumerable<ConstellationEventType> events)
        {
            await this.Send(this.BuildUnsubscribeToEventsPacket(events));
        }

        /// <summary>
        /// Unsubscribes from the specified events.
        /// </summary>
        /// <param name="events">The events to unsubscribe from</param>
        /// <returns>Whether the operation succeeded</returns>
        public async Task<bool> UnsubscribeToEventsWithResponse(IEnumerable<ConstellationEventType> events)
        {
            return this.VerifyNoErrors(await this.SendAndListen(this.BuildUnsubscribeToEventsPacket(events)));
        }

        protected override ClientWebSocket CreateWebSocket()
        {
            ClientWebSocket webSocket = base.CreateWebSocket();

            AuthenticationHeaderValue authHeader = new AuthenticationHeaderValue("Bearer", this.oauthAccessToken);
            webSocket.Options.SetRequestHeader("Authorization", authHeader.ToString());
            webSocket.Options.SetRequestHeader("X-Is-Bot", true.ToString());

            return webSocket;
        }

        private MethodPacket BuildUnsubscribeToEventsPacket(IEnumerable<ConstellationEventType> events)
        {
            Validator.ValidateList(events, "events");
            IEnumerable<string> eventStrings = ConstellationClient.ConvertEventTypesToStrings(events);
            JObject parameters = new JObject();
            parameters.Add("events", new JArray(eventStrings));
            return new MethodParamsPacket("liveunsubscribe", parameters);
        }

        private void ConstellationClient_OnEventOccurred(object sender, EventPacket eventPacket)
        {
            switch (eventPacket.eventName)
            {
                case "live":
                    this.SendSpecificEvent<ConstellationLiveEventModel>(eventPacket, this.OnSubscribedEventOccurred);
                    break;
            }
        }

        private void ConstellationClient_HelloMethodHandler(object sender, EventPacket e)
        {
            JToken authenticationValue;
            if (e.eventName.Equals("hello") && e.data.TryGetValue("authenticated", out authenticationValue) && (bool)authenticationValue)
            {
                this.Connected = true;
                this.Authenticated = true;
            }
        }
    }
}
