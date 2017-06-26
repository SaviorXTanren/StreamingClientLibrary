using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mixer.Base.Model.Interactive
{
   public class InteractiveOnParticipantLeaveModel
    {
        public List<InteractiveParticipantModel> participants{ get; set; }
    }
}
