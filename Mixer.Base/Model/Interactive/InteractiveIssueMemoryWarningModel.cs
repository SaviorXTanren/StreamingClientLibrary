using Newtonsoft.Json.Linq;

namespace Mixer.Base.Model.Interactive
{
    public class InteractiveIssueMemoryWarningModel
    {
        public uint usedBytes { get; set; }
        public uint totalBytes { get; set; }
        public JArray resources { get; set; }
    }
}
