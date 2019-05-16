namespace Twitch.Base.Models.NewAPI.Games
{
    /// <summary>
    /// Information about a game.
    /// </summary>
    public class GameModel
    {
        /// <summary>
        /// The ID of the game.
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// The name of the game.
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// The url to the game's box art.
        /// </summary>
        public string box_art_url { get; set; }
    }
}
