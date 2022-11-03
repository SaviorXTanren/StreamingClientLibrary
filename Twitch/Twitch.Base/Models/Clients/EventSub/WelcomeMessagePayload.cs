namespace Twitch.Base.Models.Clients.EventSub
{
    /// <summary>
    /// Contains information about the session.
    /// <see cref="Session"/>
    /// </summary>
    public class WelcomeMessagePayload
    {
        /// <summary>
        /// The session information.
        /// </summary>
        public Session Session { get; set; }
    }
}
