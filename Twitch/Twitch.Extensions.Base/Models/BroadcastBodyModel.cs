using StreamingClient.Base.Util;
using System.Runtime.Serialization;

namespace Twitch.Extensions.Base.Models
{
    /// <summary>
    /// The body of a request to be broadcasted out
    /// </summary>
    [DataContract]
    public class BroadcastBodyModel
    {
        /// <summary>
        /// The content type of the body
        /// </summary>
        [DataMember]
        public string content_type { get; set; } = "application/json";
        /// <summary>
        /// The target of the message.
        /// </summary>
        [DataMember]
        public string[] targets { get; set; } = new string[] { "broadcast" };
        /// <summary>
        /// The serialized message.
        /// </summary>
        [DataMember]
        public string message { get; set; }

        /// <summary>
        /// Creates a new instance of the BroadcastBodyModel class.
        /// </summary>
        /// <param name="data">The data to include in the broadcast</param>
        public BroadcastBodyModel(object data)
        {
            this.message = JSONSerializerHelper.SerializeToString(data);
        }
    }
}
