using System.Collections.Generic;

namespace Mixer.Base.Model.Interactive
{
    public class InteractiveConnectedSceneGroupCollectionModel
    {
        public InteractiveConnectedSceneGroupCollectionModel() { this.scenes = new List<InteractiveConnectedSceneGroupModel>(); }

        public List<InteractiveConnectedSceneGroupModel> scenes { get; set; }
    }

    public class InteractiveConnectedSceneGroupModel : InteractiveConnectedSceneModel
    {
        public List<InteractiveGroupModel> groups { get; set; }
    }

    public class InteractiveConnectedSceneCollectionModel
    {
        public InteractiveConnectedSceneCollectionModel() { this.scenes = new List<InteractiveConnectedSceneModel>(); }

        public List<InteractiveConnectedSceneModel> scenes { get; set; }
    }

    public class InteractiveConnectedSceneModel : InteractiveConnectedControlCollectionModel
    {
        public InteractiveConnectedSceneModel() { }

        public string sceneID { get; set; }
    }
}
