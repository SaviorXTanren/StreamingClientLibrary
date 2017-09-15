using Newtonsoft.Json.Linq;

namespace Mixer.Base.Model.Interactive
{
    public class InteractiveParticipantModel
    {
        public string sessionID { get; set; }
        public uint userID { get; set; }
        public string username { get; set; }
        public uint level { get; set; }
        public long lastInputAt { get; set; }
        public long connectedAt { get; set; }
        public bool disabled { get; set; }
        public string groupID { get; set; }
        public JObject meta { get; set; }
    }
}