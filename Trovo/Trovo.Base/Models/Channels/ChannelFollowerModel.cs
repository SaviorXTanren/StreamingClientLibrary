using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Trovo.Base.Models.Channels
{
    /// <summary>
    /// Information about the followers of a channel.
    /// </summary>
    [DataContract]
    public class ChannelFollowersModel
    {
        /// <summary>
        /// Number of followers of the channel.
        /// </summary>
        [DataMember]
        public string total { get; set; }

        /// <summary>
        /// An array of followers’ information.
        /// </summary>
        [DataMember]
        public List<ChannelFollowerModel> follower { get; set; } = new List<ChannelFollowerModel>();

        /// <summary>
        /// The total number of pages of this multi-page response.
        /// </summary>
        [DataMember]
        public int total_page { get; set; }

        /// <summary>
        /// The current page number.
        /// </summary>
        [DataMember]
        public int cursor { get; set; }
    }

    /// <summary>
    /// Information about a channel follower.
    /// </summary>
    [DataContract]
    public class ChannelFollowerModel
    {
        /// <summary>
        /// Unique id of a user.
        /// </summary>
        [DataMember]
        public string user_id { get; set; }

        /// <summary>
        /// The display name of a user, displayed in chats, channels and all across Trovo. This could be different from username.
        /// </summary>
        [DataMember]
        public string nickname { get; set; }

        /// <summary>
        /// User's profile picture
        /// </summary>
        [DataMember]
        public string profile_pic { get; set; }

        /// <summary>
        /// Return the following time
        /// </summary>
        [DataMember]
        public string followed_at { get; set; }
    }
}
