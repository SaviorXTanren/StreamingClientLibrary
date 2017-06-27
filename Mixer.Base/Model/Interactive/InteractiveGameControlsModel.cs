using Newtonsoft.Json.Linq;

namespace Mixer.Base.Model.Interactive
{
    public class InteractiveGameControlsModel
    {
        public InteractiveGameControlsSceneModel[] scenes { get; set; }
    }

    public class InteractiveGameControlsSceneModel
    {
        public string sceneID { get; set; }
        public InteractiveGameControlModel[] controls { get; set; }
    }

    public class InteractiveGameControlModel
    {
        public string kind { get; set; }
        public string controlID { get; set; }
        public JArray position { get; set; }
        public string text { get; set; }
        public uint cost { get; set; }
    }
}
