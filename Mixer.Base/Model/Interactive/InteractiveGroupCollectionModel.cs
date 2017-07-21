using System.Collections.Generic;
using System.Linq;

namespace Mixer.Base.Model.Interactive
{
    public class InteractiveGroupCollectionModel
    {
        public InteractiveGroupCollectionModel() { }

        public InteractiveGroupCollectionModel(IEnumerable<InteractiveGroupModel> groups)
        {
            this.groups = groups.ToList();
        }

        public List<InteractiveGroupModel> groups { get; set; }
    }
}
