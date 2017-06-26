using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mixer.Base.Model.Interactive
{
    public class InteractiveScene
    {

        public string sceneID { get; set; }
        public string etag { get; set; }
        public List<InteractiveControl> controls { get; set; }
        public List<InteractiveGroup> groups { get; set; }

    }
}
