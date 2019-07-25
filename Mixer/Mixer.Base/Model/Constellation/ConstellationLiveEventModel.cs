using Newtonsoft.Json.Linq;

namespace Mixer.Base.Model.Constellation
{
    /// <summary>
    /// A live event from the Constellation web socket.
    /// </summary>
    public class ConstellationLiveEventModel
    {
        /// <summary>
        /// The channel of the event.
        /// </summary>
        public string channel { get; set; }

        /// <summary>
        /// The payload of the event.
        /// </summary>
        public JObject payload { get; set; }
    }
}
