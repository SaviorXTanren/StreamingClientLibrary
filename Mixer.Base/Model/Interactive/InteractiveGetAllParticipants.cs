using System.Collections.Generic;

namespace Mixer.Base.Model.Interactive
{
    public class InteractiveGetAllParticipants
    {
        public List<InteractiveParticipantModel> participants { get; set; }
        public uint total { get; set; }
        public bool hasMore { get; set; }
    }
}