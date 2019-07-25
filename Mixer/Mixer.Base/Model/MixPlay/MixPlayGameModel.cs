namespace Mixer.Base.Model.MixPlay
{
    /// <summary>
    /// A MixPlay game.
    /// </summary>
    public class MixPlayGameModel : TimeStampedModel
    {
        /// <summary>
        /// The ID of the game.
        /// </summary>
        public uint id { get; set; }
        /// <summary>
        /// The ID of the owner.
        /// </summary>
        public uint ownerId { get; set; }
        /// <summary>
        /// The name of the game.
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// The cover image of the game.
        /// </summary>
        public string coverUrl { get; set; }
        /// <summary>
        /// The description of the game.
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// Whether the game has any versions that are published.
        /// </summary>
        public bool hasPublishedVersions { get; set; }
        /// <summary>
        /// The installation instructions for the game.
        /// </summary>
        public string installation { get; set; }
        /// <summary>
        /// The version of the controls.
        /// </summary>
        public string controlVersion { get; set; }
    }
}
