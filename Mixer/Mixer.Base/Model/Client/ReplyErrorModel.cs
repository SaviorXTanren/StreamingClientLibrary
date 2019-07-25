using Newtonsoft.Json.Linq;

namespace Mixer.Base.Model.Client
{
    /// <summary>
    /// Error information related to a web socket client message.
    /// </summary>
    public class ReplyErrorModel
    {
        /// <summary>
        /// The ID of the error.
        /// </summary>
        public uint code { get; set; }
        /// <summary>
        /// The message of the error.
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// The stack trace of the error.
        /// </summary>
        public JArray stacktrace { get; set; }
        /// <summary>
        /// Any data associated with the error.
        /// </summary>
        public JObject data { get; set; }
        /// <summary>
        /// The path of the error.
        /// </summary>
        public string path { get; set; }
    }
}
