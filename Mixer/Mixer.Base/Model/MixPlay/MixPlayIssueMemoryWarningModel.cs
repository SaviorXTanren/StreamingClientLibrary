using Newtonsoft.Json.Linq;

namespace Mixer.Base.Model.MixPlay
{
    /// <summary>
    /// A MixPlay memory warning.
    /// </summary>
    public class MixPlayIssueMemoryWarningModel
    {
        /// <summary>
        /// The number of bytes used.
        /// </summary>
        public uint usedBytes { get; set; }
        /// <summary>
        /// The total bytes allowed.
        /// </summary>
        public uint totalBytes { get; set; }
        /// <summary>
        /// Resource information.
        /// </summary>
        public JArray resources { get; set; }
    }
}
