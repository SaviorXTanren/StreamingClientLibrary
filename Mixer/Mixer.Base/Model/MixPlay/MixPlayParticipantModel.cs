using Newtonsoft.Json.Linq;

namespace Mixer.Base.Model.MixPlay
{
    /// <summary>
    /// A participant who has connected to the Interactive client.
    /// </summary>
    public class MixPlayParticipantModel
    {
        /// <summary>
        /// The unique session ID for a participant.
        /// </summary>
        public string sessionID { get; set; }
        /// <summary>
        /// The Mixer ID, if they have a registered account, of the participant.
        /// </summary>
        public uint userID { get; set; }
        /// <summary>
        /// The username of the participant.
        /// </summary>
        public string username { get; set; }
        /// <summary>
        /// The Mixer level of the participant.
        /// </summary>
        public uint level { get; set; }
        /// <summary>
        /// The last time the participant interacted.
        /// </summary>
        public long lastInputAt { get; set; }
        /// <summary>
        /// The time the participant connected.
        /// </summary>
        public long connectedAt { get; set; }
        /// <summary>
        /// Whether the participant is currently disabled from interacting.
        /// </summary>
        public bool disabled { get; set; }
        /// <summary>
        /// The ID of the group that participant is a part of.
        /// </summary>
        public string groupID { get; set; }
        /// <summary>
        /// If the participant is an unathenticated user.
        /// </summary>
        public bool? anonymous { get; set; }
        /// <summary>
        /// Metadata properties for the participant.
        /// </summary>
        public JObject meta { get; set; }
    }
}