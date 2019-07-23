namespace Mixer.Base.Model.Game
{
    /// <summary>
    /// Base game type.
    /// </summary>
    public class GameTypeSimpleModel
    {
        /// <summary>
        /// The unique ID of the game type.
        /// </summary>
        public uint id { get; set; }
        /// <summary>
        /// The name of the type.
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// The url to the type's cover.
        /// </summary>
        public string coverUrl { get; set; }
        /// <summary>
        /// The url to the type's background.
        /// </summary>
        public string backgroundUrl { get; set; }
    }
}
