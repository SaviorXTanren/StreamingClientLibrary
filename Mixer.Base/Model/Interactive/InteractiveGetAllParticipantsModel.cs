using System.Collections.Generic;

namespace Mixer.Base.Model.Interactive
{
    public class InteractiveGetAllParticipantsModel
    {
        public List<InteractiveParticipantModel> participants { get; set; }
        public uint total { get; set; }
        public bool hasMore { get; set; }
    }
}