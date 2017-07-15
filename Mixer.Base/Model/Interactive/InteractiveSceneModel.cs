using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mixer.Base.Model.Interactive
{
    public class InteractiveSceneModel
    {
        public string sceneID { get; set; }
        public string etag { get; set; }
        public List<InteractiveControlModel> controls { get; set; }
        public List<InteractiveGroupModel> groups { get; set; }
    }
}
