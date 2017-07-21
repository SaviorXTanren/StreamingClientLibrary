using System.Collections.Generic;
using System.Linq;

namespace Mixer.Base.Model.Interactive
{
    public class InteractiveSceneCollectionModel
    {
        public InteractiveSceneCollectionModel() { }

        public InteractiveSceneCollectionModel(IEnumerable<InteractiveSceneModel> scenes)
        {
            this.scenes = scenes.ToList();
        }

        public List<InteractiveSceneModel> scenes { get; set; }
    }
}
