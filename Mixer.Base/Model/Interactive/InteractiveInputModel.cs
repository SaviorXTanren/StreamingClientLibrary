using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mixer.Base.Model.Interactive
{
    public class InteractiveInputModel
    {
        public string @event { get; set; }
        public int button { get; set; }
        public string controlID { get; set; }
        public double x { get; set; }
        public double y { get; set; }
    }
}
