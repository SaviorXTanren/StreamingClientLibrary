using Newtonsoft.Json.Linq;

namespace Mixer.Base.Model.Interactive
{
    public class InteractiveControlModel
    {
        public string controlID { get; set; }
        public string kind { get; set; }
        public string etag { get; set; }
        public bool disabled { get; set; }
        public InteractiveControlPositionModel[] position { get; set; }
        public JObject meta { get; set; } = new JObject();
    }
}
