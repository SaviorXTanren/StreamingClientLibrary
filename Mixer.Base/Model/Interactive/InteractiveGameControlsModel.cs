using System.Collections.Generic;

namespace Mixer.Base.Model.Interactive
{
    public class InteractiveGameControlsModel
    {
        public InteractiveGameControlsModel()
        {
            this.scenes = new List<InteractiveSceneModel>();
        }

        public List<InteractiveSceneModel> scenes { get; set; }
    }
}
