using Newtonsoft.Json.Linq;

namespace Mixer.Base.Model.MixPlay
{
    /// <summary>
    /// A MixPlay control.
    /// </summary>
    public class MixPlayControlModel
    {
        /// <summary>
        /// A unique control ID.
        /// </summary>
        public string controlID { get; set; }
        /// <summary>
        /// The kind of MixPlay control.
        /// </summary>
        public string kind { get; set; }
        /// <summary>
        /// The versioning tag.
        /// </summary>
        public string etag { get; set; }
        /// <summary>
        /// Whether the control is disabled.
        /// </summary>
        public bool disabled { get; set; }
        /// <summary>
        /// The position of the control.
        /// </summary>
        public MixPlayControlPositionModel[] position { get; set; }
        /// <summary>
        /// The metadata of the control.
        /// </summary>
        public JObject meta { get; set; } = new JObject();
    }
}
