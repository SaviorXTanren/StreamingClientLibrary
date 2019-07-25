using System.Collections.Generic;

namespace Mixer.Base.Model.MixPlay
{
    /// <summary>
    /// A collection of MixPlay participants.
    /// </summary>
    public class MixPlayParticipantCollectionModel
    {
        /// <summary>
        /// The MixPlay participants.
        /// </summary>
        public List<MixPlayParticipantModel> participants { get; set; }
        /// <summary>
        /// The total number of participants.
        /// </summary>
        public uint total { get; set; }
        /// <summary>
        /// Whether there are more participants.
        /// </summary>
        public bool hasMore { get; set; }
    }
}