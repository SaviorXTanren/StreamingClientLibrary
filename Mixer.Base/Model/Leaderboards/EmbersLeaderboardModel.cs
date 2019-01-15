using System.Runtime.Serialization;

namespace Mixer.Base.Model.Leaderboards
{
    /// <summary>
    /// The embers leaderboard information for an individual user.
    /// </summary>
    [DataContract]
    public class EmbersLeaderboardModel
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
        /// The ember statistics value for the Mixer user.
        /// </summary>
        [DataMember]
        public int statValue { get; set; }
    }
}
