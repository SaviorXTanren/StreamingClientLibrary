using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Twitch.Base.Models.V5.Games
{
    /// <summary>
    /// Information about a Game.
    /// </summary>
    public class GameModel
    {
        /// <summary>
        /// The ID of the game.
        /// </summary>
        [JsonProperty("_id")]
        public long id { get; set; }
        /// <summary>
        /// The box art images for the game.
        /// </summary>
        public JObject box { get; set; }
        /// <summary>
        /// The reference ID for the game on Giant Bomb
        /// </summary>
        public long giantbomb_id { get; set; }
        /// <summary>
        /// The logo images for the game.
        /// </summary>
        public JObject logo { get; set; }
        /// <summary>
        /// The name of the game.
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// The popularity numbers of the game.
        /// </summary>
        public long popularity { get; set; }
    }
}
