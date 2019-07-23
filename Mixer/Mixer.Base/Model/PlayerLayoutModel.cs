using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Mixer.Base.Model
{
    /// <summary>
    /// The PlayerLayout configures the arrangement and positioning of the channels within a costream or hosting session.
    /// </summary>
    public class PlayerLayoutModel
    {
        /// <summary>
        /// Name of the layout to use for this arrangement. Possible options are mobile, grid, sidebar and chooser.
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// Contains settings for channels. Channels are arranged in the layout in the order they're described here.
        /// </summary>
        public List<JObject> players { get; set; } 
    }
}
