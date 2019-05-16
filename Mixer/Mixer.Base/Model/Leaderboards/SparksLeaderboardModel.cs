using System.Runtime.Serialization;

namespace Mixer.Base.Model.Leaderboards
{
    /// <summary>
    /// The spark leaderboard information for an individual user.
    /// </summary>
    [DataContract]
    public class SparksLeaderboardModel
    {
        /// <summary>
        /// The username of the Mixer user.
        /// </summary>
        [DataMember]
        public string username { get; set; }

        /// <summary>
        /// The avatar URL of the Mixer user.
        /// </summary>
        [DataMember]
        public string avatarUrl { get; set; }

        /// <summary>
        /// The spark statistics value for the Mixer user.
        /// </summary>
        [DataMember]
        public int statValue { get; set; }
    }
}
