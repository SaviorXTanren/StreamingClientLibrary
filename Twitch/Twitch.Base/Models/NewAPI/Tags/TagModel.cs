using Newtonsoft.Json.Linq;

namespace Twitch.Base.Models.NewAPI.Tags
{
    /// <summary>
    /// Information about a tag.
    /// </summary>
    public class TagModel
    {
        /// <summary>
        /// The ID of the tag.
        /// </summary>
        public string tag_id { get; set; }
        /// <summary>
        /// Whether the tag was auto-generated.
        /// </summary>
        public bool is_auto { get; set; }
        /// <summary>
        /// The localized names for the tag.
        /// </summary>
        public JObject localization_names { get; set; }
        /// <summary>
        /// The localized descriptions for the tag.
        /// </summary>
        public JObject localization_descriptions { get; set; }
    }
}
