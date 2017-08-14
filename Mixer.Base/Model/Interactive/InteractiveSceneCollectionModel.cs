using System.Collections.Generic;

namespace Mixer.Base.Model.Interactive
{
    public class InteractiveSceneGroupCollectionModel
    {
        public InteractiveSceneGroupCollectionModel() { this.scenes = new List<InteractiveSceneGroupModel>(); }

        public List<InteractiveSceneGroupModel> scenes { get; set; }
    }

    public class InteractiveSceneCollectionModel
    {
        public InteractiveSceneCollectionModel() { this.scenes = new List<InteractiveSceneModel>(); }

        public List<InteractiveSceneModel> scenes { get; set; }
    }
}
