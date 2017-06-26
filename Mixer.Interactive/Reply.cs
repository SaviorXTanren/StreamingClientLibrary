using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mixer.Interactive
{
   public class Reply
    {

        public uint id { get; set; }
        public string message { get; set; }



        public override string ToString() { return string.Format("{0} - {1}", this.id, this.message); }
    }
}
