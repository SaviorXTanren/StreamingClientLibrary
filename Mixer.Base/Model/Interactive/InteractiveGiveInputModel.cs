using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mixer.Base.Model.Interactive
{
  public  class InteractiveGiveInputModel : InteractiveMethodPacket
    {
        public string participantID { get; set; }
        public string transactionID { get; set; }
        public InteractiveInputModel input { get; set; }
    }
}
