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
using System.Threading.Tasks;

namespace Mixer.Base.Clients
{
    /// <summary>
    /// The events types that can be registered for the Constellation Client.
    /// </summary>
    public enum ConstellationEventTypeEnum
    {
        /// <summary>
        /// https://dev.mixer.com/reference/constellation/events/live/announcement
        /// </summary>
        [Name("Announcements")]
        announcement__announce,

        /// <summary>
        /// https://dev.mixer.com/reference/constellation/events/live/channel%20followed
        /// </summary>
        [Name("Channel Followed")]
        channel__id__followed,
        /// <summary>
        /// https://dev.mixer.com/reference/constellation/events/live/channel%20hosted
        /// </summary>
        [Name("Channel Hosted")]
        channel__id__hosted,
        /// <summary>
        /// https://dev.mixer.com/reference/constellation/events/live/channel%20unhosted
        /// </summary>
        [Name("Channel Unhosted")]
        channel__id__unhosted,
        /// <summary>
        /// Obsolete constellation event
        /// </summary>
        [Name("Channel Status")]
        [Obsolete]
        channel__id__status,
        /// <summary>
        /// https://dev.mixer.com/reference/constellation/events/live/channel%20subscribed
        /// </summary>
        [Name("Channel Subscribed")]
        channel__id__subscribed,
        /// <summary>
        /// https://dev.mixer.com/reference/constellation/events/live/channel%20resubscribed
        /// </summary>
        [Name("Channel Resubscribed")]
        channel__id__resubscribed,
        /// <summary>
        /// https://dev.mixer.com/reference/constellation/events/live/channel%20resubshared
        /// </summary>
        [Name("Channel Resubscribed Shared")]
        channel__id__resubShared,
        /// <summary>
        /// https://dev.mixer.com/reference/constellation/events/live/channel%20update
        /// </summary>
        [Name("Channel Updated")]
        channel__id__update,

        /// <summary>
        /// https://dev.mixer.com/reference/constellation/events/live/team%20deleted
        /// </summary>
        [Name("Team Deleted")]
        team__id__deleted,
        /// <summary>
        /// https://dev.mixer.com/reference/constellation/events/live/team%20memberaccepted
        /// </summary>
        [Name("Team Member Accepted")]
        team__id__memberAccepted,
        /// <summary>
        /// https://dev.mixer.com/reference/constellation/events/live/team%20memberinvited
        /// </summary>
        [Name("Team Member Invited")]
        team__id__memberInvited,
        /// <summary>
        /// https://dev.mixer.com/reference/constellation/events/live/team%20memberremoved
        /// </summary>
        [Name("Team Member Removed")]
        team__id__memberRemoved,
        /// <summary>
        /// https://dev.mixer.com/reference/constellation/events/live/team%20ownerchanged
        /// </summary>
        [Name("Team Owner Changed")]
        team__id__ownerChanged,

        /// <summary>
        /// https://dev.mixer.com/reference/constellation/events/live/user%20achievement
        /// </summary>
        [Name("User Achievement")]
        user__id__achievement,
        /// <summary>
        /// https://dev.mixer.com/reference/constellation/events/live/user%20followed
        /// </summary>
        [Name("User Followed")]
        user__id__followed,
        /// <summary>
        /// https://dev.mixer.com/reference/constellation/events/live/user%20notify
        /// </summary>
        [Name("User Notification")]
        user__id__notify,
        /// <summary>
        /// https://dev.mixer.com/reference/constellation/events/live/user%20subscribed
        /// </summary>
        [Name("User Subscribed")]
        user__id__subscribed,
        /// <summary>
        /// https://dev.mixer.com/reference/constellation/events/live/user%20resubscribed
        /// </summary>
        [Name("User Resubscribed")]
        user__id__resubscribed,
        /// <summary>
        /// https://dev.mixer.com/reference/constellation/events/live/user%20teamaccepted
        /// </summary>
        [Name("User Joined Team")]
        user__id__teamAccepted,
        /// <summary>
        /// https://dev.mixer.com/reference/constellation/events/live/user%20teaminvited
        /// </summary>
        [Name("User Invited To Team")]
        user__id__teamInvited,
        /// <summary>
        /// https://dev.mixer.com/reference/constellation/events/live/user%20teamremoved
        /// </summary>
        [Name("User Left Team")]
        user__id__teamRemoved,
        /// <summary>
        /// https://dev.mixer.com/reference/constellation/events/live/user%20update
        /// </summary>
        [Name("User Updated")]
        user__id__update,

        /// <summary>
        /// https://dev.mixer.com/reference/constellation/events/live/channel%20directpurchased
        /// </summary>
        [Name("Channel Direct Purchased")]
        channel__id__directPurchased,

        /// <summary>
        /// https://dev.mixer.com/reference/constellation/events/live/patronage%20update
        /// </summary>
        [Name("Channel Patronage Update")]
        channel__id__patronageUpdate,

        /// <summary>
        /// https://dev.mixer.com/reference/constellation/events/live/channel%20skill
        /// </summary>
        [Name("Channel Skill Used")]
        channel__id__skill
}

    /// <summary>
    /// The real-time client for Constellation event interactions.
    /// </summary>
    public class ConstellationEventType : Object, IEquatable<ConstellationEventType>
    {
        /// <summary>
        /// The type of Constellation event to register for.
        /// </summary>
        public ConstellationEventTypeEnum Type { get; set; }
        /// <summary>
        /// The unique ID associated with the event type, if any.
        /// </summary>
        public uint ID { get; set; }

        /// <summary>
        /// Initializes a new instance of the ConstellationEventType class with the specified event type and unique ID.
        /// </summary>
        /// <param name="type">The type of event to register for</param>
        /// <param name="id">The unique ID for the event</param>
        public ConstellationEventType(ConstellationEventTypeEnum type, uint id = 0)
        {
            this.Type = type;
            this.ID = id;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return ConstellationClient.ConvertEventTypesToStrings(new List<ConstellationEventType>() { this }).First();
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode() { return this.Type.GetHashCode() + this.ID.GetHashCode(); }

        /// <summary>
        /// Determines whether the specified object instances are considered equal.
        /// </summary>
        /// <param name="obj">The other object to compare.</param>
        /// <returns>true if the objects are considered equal; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is ConstellationEventType)
            {
                return this.Equals((ConstellationEventType)obj);
            }
            return false;
        }

        /// <summary>
        /// Determines whether the specified ConstellationEventType instances are considered equal.
        /// </summary>
        /// <param name="other">The other ConstellationEventType to compare.</param>
        /// <returns>true if the ConstellationEventTypes are considered equal; otherwise, false.</returns>
        public bool Equals(ConstellationEventType other)
        {
            return this.Type.Equals(other.Type) && this.ID.Equals(other.ID);
        }
    }

    /// <summary>
    /// The Constellation WebSocket client.
    /// </summary>
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

        /// <summary>
        /// This event is triggered when a subscribed event occurs.
        /// </summary>
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
        public async Task<bool> Connect()
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

        /// <summary>
        /// Creates a WebSocket client for the Constellation service.
        /// </summary>
        /// <returns>The client WebSocket</returns>
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
