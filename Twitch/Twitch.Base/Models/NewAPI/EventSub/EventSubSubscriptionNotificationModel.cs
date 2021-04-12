using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StreamingClient.Base.Util;
using System.Collections.Generic;

namespace Twitch.Base.Models.NewAPI.EventSub
{
    /// <summary>
    /// The challenge sent by Twitch to the registered callback
    /// </summary>
    public class EventSubSubscriptionNotificationModel
    {
        /// <summary>
        /// The subscription that must be approved.
        /// </summary>
        public EventSubSubscriptionModel subscription { get; set; }

        /// <summary>
        /// The event being notified.
        /// </summary>
        public JObject @event { get; set; }
    }
}
