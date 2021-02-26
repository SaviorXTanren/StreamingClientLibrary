using Glimesh.Base.Models.Users;
using Newtonsoft.Json.Linq;

namespace Glimesh.Base.Models.Clients.Channel
{
    /// <summary>
    /// Packet sent when connecting to a specific channel's follow updates.
    /// </summary>
    public class FollowsJoinPacketModel : ClientPacketModelBase
    {
        /// <summary>
        /// Creates a new instance of the FollowsJoinPacketModel class.
        /// </summary>
        /// <param name="streamerID">The ID of the streamer to get follows for</param>
        public FollowsJoinPacketModel(string streamerID)
        {
            this.Topic = AbsintheControlTopicName;
            this.Event = DocEventName;
            this.Payload["query"] = $"subscription {{ followers(streamerId: {streamerID}) {{ {UserFollowModel.AllFields} }} }}";
            this.Payload["variables"] = new JObject();
        }
    }
}
