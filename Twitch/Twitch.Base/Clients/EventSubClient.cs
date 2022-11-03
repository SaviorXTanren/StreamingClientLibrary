using Newtonsoft.Json.Linq;
using StreamingClient.Base.Util;
using StreamingClient.Base.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Twitch.Base.Models.Clients.EventSub;

namespace Twitch.Base.Clients
{
    /// <summary>
    /// Web Socket client for interacting with EventSub service.
    /// </summary>
    public class EventSubClient : ClientWebSocketBase
    {
        /// <summary>
        /// The default event sub connection url.
        /// </summary>
        public const string EVENT_SUB_CONNECTION_URL = "wss://eventsub-beta.wss.twitch.tv/ws";

        private IReadOnlyDictionary<MessageType, (Type MessageType, MethodInfo MessageHandler)> MessageTypeMap;

        /// <summary>
        /// Invoked when a first connected, and the session ID is used to subscribe to events.
        /// IMPORTANT You have 10 seconds from the time you receive the Welcome message to subscribe to an event. 
        /// If you don’t subscribe within this timeframe, the server closes the connection.
        /// </summary>
        public event EventHandler<WelcomeMessage> OnWelcomeMessageReceived;
        /// <summary>
        /// The keepalive messages indicate that the WebSocket connection is healthy. The server sends this message 
        /// if Twitch doesn’t deliver an event notification within the keepalive_timeout_seconds window specified in the Welcome message.
        /// If your client doesn’t receive an event or keepalive message for longer than keepalive_timeout_seconds, 
        /// you should assume the connection is lost and reconnect to the server and resubscribe to the events.The
        /// keepalive timer is reset with each notification or keepalive message.
        /// </summary>
        public event EventHandler<KeepAliveMessage> OnKeepAliveMessageReceived;
        /// <summary>
        /// A reconnect message is sent if the server has to drop the connection. The message is sent 30 seconds prior to 
        /// dropping the connection.
        /// The message includes a URL in the reconnect_url field that you should immediately use to create a new connection.
        /// The connection will include the same subscriptions that the old connection had.You should not close the old connection
        /// until you receive a Welcome message on the new connection.
        /// The old connection receives events up until you connect to the new URL and receive the welcome message.
        /// NOTE Twitch sends the old connection a close frame with code 4004 if you connect to the new socket but never 
        /// disconnect from the old socket or you don’t connect to the new socket within the specified timeframe.
        /// </summary>
        public event EventHandler<ReconnectMessage> OnReconnectMessageReceived;
        /// <summary>
        /// A notification message is sent when an event that you subscribe to occurs. The message contains the event’s details.
        /// </summary>
        public event EventHandler<NotificationMessage> OnNotificationMessageReceived;
        /// <summary>
        /// A revocation message is sent if Twitch revokes a subscription. The subscription object’s type field identifies the subscription 
        /// that was revoked, and the status field identifies the reason why the subscription was revoked.
        /// </summary>
        public event EventHandler<RevocationMessage> OnRevocationMessageReceived;

        /// <summary>
        /// Creates a new EventSub client
        /// </summary>
        public EventSubClient()
        {
            IReadOnlyDictionary<Type, MethodInfo> messageHandlers = typeof(EventSubClient)
                .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                .Where(m => m.Name == "HandleMessage")
                .ToDictionary(e => e.GetParameters().First().ParameterType, e => e);
            MessageTypeMap = new Dictionary<MessageType, (Type, MethodInfo)>
            {
                { MessageType.session_welcome, (typeof(WelcomeMessage), messageHandlers[typeof(WelcomeMessage)]) },
                { MessageType.session_keepalive, (typeof(KeepAliveMessage), messageHandlers[typeof(KeepAliveMessage)]) },
                { MessageType.session_reconnect, (typeof(ReconnectMessage), messageHandlers[typeof(ReconnectMessage)]) },
                { MessageType.notification, (typeof(NotificationMessage), messageHandlers[typeof(NotificationMessage)]) },
                { MessageType.revocation, (typeof(RevocationMessage), messageHandlers[typeof(RevocationMessage)]) },
            };
        }

        /// <summary>
        /// Connects to the default EventSub connection.
        /// </summary>
        /// <returns>An awaitable Task</returns>
        public async Task Connect()
        {
            await base.Connect(EventSubClient.EVENT_SUB_CONNECTION_URL);
        }

        /// <inheritdoc />
        protected override Task ProcessReceivedPacket(string packet)
        {
            if (!string.IsNullOrEmpty(packet))
            {
                JObject jsonData = JObject.Parse(packet);

                string messageTypeString = jsonData["metadata"]?["message_type"]?.Value<string>();
                if (Enum.TryParse<MessageType>(messageTypeString, out MessageType messageType) &&
                    MessageTypeMap.TryGetValue(messageType, out var actualMessageInfo)) 
                {
                    var payload = jsonData.ToObject(actualMessageInfo.MessageType);
                    actualMessageInfo.MessageHandler.Invoke(this, new object[] { payload });
                }
            }

            return Task.CompletedTask;
        }

        private Task HandleMessage(WelcomeMessage message)
        {
            this.OnWelcomeMessageReceived?.Invoke(this, message);
            return Task.CompletedTask;
        }

        private Task HandleMessage(KeepAliveMessage message)
        {
            this.OnKeepAliveMessageReceived?.Invoke(this, message);
            return Task.CompletedTask;
        }

        private Task HandleMessage(ReconnectMessage message)
        {
            this.OnReconnectMessageReceived?.Invoke(this, message);
            return Task.CompletedTask;
        }

        private Task HandleMessage(NotificationMessage message)
        {
            this.OnNotificationMessageReceived?.Invoke(this, message);
            return Task.CompletedTask;
        }

        private Task HandleMessage(RevocationMessage message)
        {
            this.OnRevocationMessageReceived?.Invoke(this, message);
            return Task.CompletedTask;
        }
    }
}
