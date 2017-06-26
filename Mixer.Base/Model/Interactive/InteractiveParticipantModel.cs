using Newtonsoft.Json.Linq;
using System;

namespace Mixer.Base.Model.Interactive
{
    public class InteractiveParticipantModel
    {
        public string sessionID { get; set; }
        public string etag { get; set; }
        public uint userID { get; set; }
        public string username { get; set; }
        public uint level { get; set; }
        public UInt64 lastInputAt { get; set; }
        public UInt64 connectedAt { get; set; }
        public bool disabled { get; set; }
        public string groupID { get; set; }
        public JObject meta { get; set; }
    }
}

/*
 * "sessionID": "efe5e1d6-a870-4f77-b7e4-1cfaf30b097e",
"etag": "54600913",
"userID": 146,
"username": "connor",
"level": 67,
"lastInputAt": 1484763854277,
"connectedAt": 1484763846242,
"disabled": false,
"groupID": "default",
"meta": {
"is_awesome": {
"etag": 37849560,
"value": true
}
}
*/