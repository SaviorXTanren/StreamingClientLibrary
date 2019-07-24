using Newtonsoft.Json.Linq;

namespace Mixer.Base.Model.Client
{
    /// <summary>
    /// A reply is received from the server in response to a Method packet.
    /// </summary>
    public class ReplyPacket : WebSocketPacket
    {
        /// <summary>
        /// Creates a new instance of the ReplyPacket class.
        /// </summary>
        public ReplyPacket() { this.type = "reply"; }

        /// <summary>
        /// A 32-bit signed integer incremented by the server on each packet that is sent. The client must include the last seq number it saw when sending information to Interactive.
        /// </summary>
        public int seq { get; set; }

        /// <summary>
        /// The unstructured result of the method. It SHOULD be null if an error occurs, but it MAY be null for successful replies.
        /// </summary>
        public JToken result { get; set; }
        /// <summary>
        /// If an error has not occurred null, otherwise an error message.
        /// </summary>
        public JToken error { get; set; }
        /// <summary>
        /// Associated event data - may be of any type, specific to the event.
        /// </summary>
        public JToken data { get; set; }

        /// <summary>
        /// The JObject version of the result.
        /// </summary>
        public JObject resultObject { get { return (this.result != null && this.result.HasValues) ? JObject.Parse(this.result.ToString()) : null; } }
        /// <summary>
        /// The JObject version of the error.
        /// </summary>
        public JObject errorObject { get { return (this.error != null && this.error.HasValues) ? JObject.Parse(this.error.ToString()) : null; } }
        /// <summary>
        /// The JObject version of the data.
        /// </summary>
        public JObject dataObject { get { return (this.data != null && this.data.HasValues) ? JObject.Parse(this.data.ToString()) : null; } }
    }
}
