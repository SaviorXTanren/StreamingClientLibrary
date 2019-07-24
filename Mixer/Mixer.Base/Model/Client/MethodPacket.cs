using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mixer.Base.Model.Client
{
    /// <summary>
    /// A method is sent to the chat server the server will respond with a Reply packet.
    /// </summary>
    public class MethodPacket : WebSocketPacket
    {
        /// <summary>
        /// Creates a new instance of the MethodPacket class.
        /// </summary>
        public MethodPacket()
        {
            this.type = "method";
            this.arguments = new JArray();
        }

        /// <summary>
        /// Creates a new instance of the MethodPacket class.
        /// </summary>
        /// <param name="method">The name of the method</param>
        public MethodPacket(string method) : this() { this.method = method; }

        /// <summary>
        /// The method name to execute.
        /// </summary>
        public string method { get; set; }

        /// <summary>
        /// A 32-bit signed integer incremented by the server on each packet that is sent. The client must include the last seq number it saw when sending information to Interactive.
        /// </summary>
        public int seq { get; set; }

        /// <summary>
        /// MAY be set to true if the server does not require a reply for the method call. The client MUST effect any state changes regardless of the value of discard. The client MAY respect the server’s request to discard the successful response, but MUST reply with an error if one does occur.
        /// </summary>
        public bool discard { get; set; }

        /// <summary>
        /// An array of arguments, specific per method type.
        /// </summary>
        public JArray arguments { get; set; }

        /// <summary>
        /// Should be an object, not an array, of named arguments to pass into the method.
        /// </summary>
        [JsonProperty("params")]
        public JObject parameters { get; set; }
    }

    /// <summary>
    /// A method packet with a set of arguments.
    /// </summary>
    public class MethodArgPacket : MethodPacket
    {
        /// <summary>
        /// Creates a new instance of the MethodArgPacket class.
        /// </summary>
        /// <param name="method">The method to call</param>
        /// <param name="arguments">The arguments to pass in</param>
        public MethodArgPacket(string method, JArray arguments)
            : base()
        {
            this.method = method;
            this.arguments = arguments;
        }
    }

    /// <summary>
    /// A method packet with a set of parameters.
    /// </summary>
    public class MethodParamsPacket : MethodPacket
    {
        /// <summary>
        /// Creates a new instance of the MethodParamsPacket
        /// </summary>
        /// <param name="method">The method to call</param>
        /// <param name="parameters">The parameters to pass in</param>
        public MethodParamsPacket(string method, JObject parameters)
            : base()
        {
            this.method = method;
            this.parameters = parameters;
        }
    }
}
