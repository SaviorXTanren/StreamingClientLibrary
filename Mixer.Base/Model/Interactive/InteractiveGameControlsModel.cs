using System.Collections.Generic;

namespace Mixer.Base.Model.Interactive
{
    public class InteractiveGameControlsModel
    {
        public InteractiveGameControlsModel()
        {
            this.scenes = new List<InteractiveGameControlsSceneModel>();
        }

        public List<InteractiveGameControlsSceneModel> scenes { get; set; }
    }

    public class InteractiveGameControlsSceneModel
    {
        public InteractiveGameControlsSceneModel()
        {
            this.controls = new List<InteractiveControlModel>();
        }

        public string sceneID { get; set; }
        public List<InteractiveControlModel> controls { get; set; }
    }
}
