using System.Collections.Generic;

namespace Mixer.Base.Model.Interactive
{
    public class InteractiveSceneModel : InteractiveModelBase
    {
        public string sceneID { get; set; }
        public List<InteractiveGroupModel> groups { get; set; }
        public List<InteractiveControlModel> controls { get; set; }
    }
}
