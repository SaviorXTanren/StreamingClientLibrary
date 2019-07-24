namespace Mixer.Base.Model.MixPlay
{
    /// <summary>
    /// The updatable metadata for a MixPlay game version.
    /// </summary>
    public class MixPlayGameVersionUpdateableModel : TimeStampedModel
    {
        /// <summary>
        /// Creates a new instance of the MixPlayGameVersionUpdateableModel class.
        /// </summary>
        public MixPlayGameVersionUpdateableModel()
        {
            this.controls = new MixPlaySceneCollectionModel();
        }

        /// <summary>
        /// The state of the version.
        /// </summary>
        public string state { get; set; }
        /// <summary>
        /// The installation instructions for the version.
        /// </summary>
        public string installation { get; set; }
        /// <summary>
        /// The download instructions for the verison.
        /// </summary>
        public string download { get; set; }
        /// <summary>
        /// The controls of the version.
        /// </summary>
        public MixPlaySceneCollectionModel controls { get; set; }
        /// <summary>
        /// The version of the controls.
        /// </summary>
        public string controlVersion { get; set; }
    }

    /// <summary>
    /// The version of a MixPlay game.
    /// </summary>
    public class MixPlayGameVersionModel : MixPlayGameVersionUpdateableModel
    {
        /// <summary>
        /// The ID of the version.
        /// </summary>
        public uint id { get; set; }
        /// <summary>
        /// The ID of the game.
        /// </summary>
        public uint gameId { get; set; }
        /// <summary>
        /// The version text.
        /// </summary>
        public string version { get; set; }
        /// <summary>
        /// The ordering of the version.
        /// </summary>
        public uint versionOrder { get; set; }
        /// <summary>
        /// The changelog for the version.
        /// </summary>
        public string changelog { get; set; }
    }
}
