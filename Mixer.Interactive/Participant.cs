using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mixer.Interactive
{
  public  class Participant
    {
  
        public uint userID { get; set; }
        public string username { get; set; }
        public uint level { get; set; }
        public UInt64 lastInputAt { get; set; }
        public UInt64 connectedAt { get; set; }


        public override string ToString() { return string.Format("{0} - {1}", this.userID, this.username); }
    }
}
