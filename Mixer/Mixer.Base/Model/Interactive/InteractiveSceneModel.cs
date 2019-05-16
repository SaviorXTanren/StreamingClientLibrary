using System.Collections.Generic;

namespace Mixer.Base.Model.Interactive
{
    public class InteractiveSceneCollectionModel
    {
        public InteractiveSceneCollectionModel() { this.scenes = new List<InteractiveSceneModel>(); }

        public List<InteractiveSceneModel> scenes { get; set; }
    }

    public class InteractiveSceneModel : InteractiveControlCollectionModel
    {
        public InteractiveSceneModel() { }

        public string sceneID { get; set; }
        public string etag { get; set; }
    }
}
