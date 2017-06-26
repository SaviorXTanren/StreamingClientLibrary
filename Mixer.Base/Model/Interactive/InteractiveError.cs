using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mixer.Base.Model.Interactive
{
    public class InteractiveError
    {
        public uint code { get; set; }
        public string message { get; set; }
        public string path { get; set; }

    }
}
