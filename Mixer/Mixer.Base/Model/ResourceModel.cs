using Newtonsoft.Json.Linq;

namespace Mixer.Base.Model
{
    /// <summary>
    /// Resources are general use items that are stored and linked to other entities within Mixer.They usually represent images or backgrounds for a channel or user.
    /// </summary>
    public class ResourceModel
    {
        /// <summary>
        /// The unique ID of the resource.
        /// </summary>
        public uint id { get; set; }
        /// <summary>
        /// The type of the resource.
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// Id linking to the parent object.
        /// </summary>
        public uint? relid { get; set; }
        /// <summary>
        /// The url of the resource.
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// The storage type of the resource.
        /// </summary>
        public string store { get; set; }
        /// <summary>
        /// Relative url to the resource.
        /// </summary>
        public string remotePath { get; set; }
        /// <summary>
        /// Additional resource information.
        /// </summary>
        public JObject meta { get; set; }
    }
}
