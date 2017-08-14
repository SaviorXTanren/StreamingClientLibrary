using System.Collections.Generic;

namespace Mixer.Base.Model.Interactive
{
    public class InteractiveSceneGroupModel : InteractiveSceneModel
    {
        public List<InteractiveGroupModel> groups { get; set; }
    }

    public class InteractiveSceneModel : InteractiveControlCollectionModel
    {
        public InteractiveSceneModel() { }

        public InteractiveSceneModel(string sceneID) { this.sceneID = sceneID; }

        public string sceneID { get; set; }
    }
}
