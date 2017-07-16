using Mixer.Base.Model.Client;

namespace Mixer.Base.Model.Interactive
{
    public  class InteractiveGiveInputModel : MethodPacket
    {
        public string participantID { get; set; }
        public string transactionID { get; set; }
        public InteractiveInputModel input { get; set; }
    }
}
