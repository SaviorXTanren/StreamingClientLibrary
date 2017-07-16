using System.Collections.Generic;

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
