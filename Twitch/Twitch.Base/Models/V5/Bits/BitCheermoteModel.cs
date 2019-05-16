using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Twitch.Base.Models.V5.Bits
{
    /// <summary>
    /// Information about a tier for a Bits cheermote.
    /// </summary>
    public class BitCheermoteTierModel
    {
        /// <summary>
        /// The ID of the tier.
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// The color of the tier.
        /// </summary>
        public string color { get; set; }
        /// <summary>
        /// The images associated with the tier.
        /// </summary>
        public JObject images { get; set; }
        /// <summary>
        /// The minimum number of bits required to meet the tier.
        /// </summary>
        public int min_bits { get; set; }
    }

    /// <summary>
    /// Information about a Bits cheermote.
    /// </summary>
    public class BitCheermoteModel
    {
        /// <summary>
        /// The backgrounds of the cheermote.
        /// </summary>
        public List<string> background { get; set; }
        /// <summary>
        /// The prefix of the cheermote.
        /// </summary>
        public string prefix { get; set; }
        /// <summary>
        /// The scale amounts of the cheermote.
        /// </summary>
        public List<string> scales { get; set; }
        /// <summary>
        /// The states of the cheermote.
        /// </summary>
        public List<string> states { get; set; }
        /// <summary>
        /// The tiers of the cheermote.
        /// </summary>
        public List<BitCheermoteTierModel> tiers { get; set; }

    }
}
