using Glimesh.Base.Models.Users;
using Newtonsoft.Json.Linq;

namespace Glimesh.Base.Models.Clients.Channel
{
    /// <summary>
    /// Packet received when an update has occurred in a channel.
    /// </summary>
    public class FollowPacketModel : ClientResponsePacketModel
    {
        /// <summary>
        /// The updated channel data.
        /// </summary>
        public UserFollowModel Follow { get; set; }

        /// <summary>
        /// Creates a new instance of the FollowPacketModel class.
        /// </summary>
        /// <param name="serializedChatPacketArray">The serialized packet array</param>
        public FollowPacketModel(string serializedChatPacketArray)
            : base(serializedChatPacketArray)
        {
            JObject channel = (JObject)this.Payload.SelectToken("result.data.followers");
            if (channel != null)
            {
                this.Follow = channel.ToObject<UserFollowModel>();
            }
        }
    }
}
