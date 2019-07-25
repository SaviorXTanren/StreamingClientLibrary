using Mixer.Base.Model.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mixer.Base.Model.MixPlay
{
    /// <summary>
    /// The input given on a MixPlay game.
    /// </summary>
    public class MixPlayGiveInputModel : MethodPacket
    {
        /// <summary>
        /// The ID of the participant.
        /// </summary>
        public string participantID { get; set; }
        /// <summary>
        /// The ID of the transaction.
        /// </summary>
        public string transactionID { get; set; }
        /// <summary>
        /// The input details.
        /// </summary>
        public MixPlayInputModel input { get; set; }
    }

    /// <summary>
    /// The input from a MixPlay game.
    /// </summary>
    public class MixPlayInputModel
    {
        /// <summary>
        /// The ID of the control.
        /// </summary>
        public string controlID { get; set; }
        /// <summary>
        /// The type of event.
        /// </summary>
        [JsonProperty("event")]
        public string eventType { get; set; }

        /// <summary>
        /// The X position for a joystick.
        /// </summary>
        public double x { get; set; }
        /// <summary>
        /// The Y position for a joystick.
        /// </summary>
        public double y { get; set; }

        /// <summary>
        /// The value of a text box.
        /// </summary>
        public string value { get; set; }

        /// <summary>
        /// The metadata properties of the control.
        /// </summary>
        public JObject meta { get; set; }
    }
}
