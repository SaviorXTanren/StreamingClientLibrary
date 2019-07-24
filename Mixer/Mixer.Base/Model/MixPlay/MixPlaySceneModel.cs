using System.Collections.Generic;

namespace Mixer.Base.Model.MixPlay
{
    /// <summary>
    /// A connected instance of MixPlay scenes.
    /// </summary>
    public class MixPlaySceneCollectionModel
    {
        /// <summary>
        /// Creates a new instance of the MixPlaySceneCollectionModel class.
        /// </summary>
        public MixPlaySceneCollectionModel() { this.scenes = new List<MixPlaySceneModel>(); }

        /// <summary>
        /// The set of scenes.
        /// </summary>
        public List<MixPlaySceneModel> scenes { get; set; }
    }

    /// <summary>
    /// A set of MixPlay scenes.
    /// </summary>
    public class MixPlaySceneModel : MixPlayControlCollectionModel
    {
        /// <summary>
        /// Creates a new instance of the MixPlaySceneModel class.
        /// </summary>
        public MixPlaySceneModel() { }

        /// <summary>
        /// The ID of the scene.
        /// </summary>
        public string sceneID { get; set; }
        /// <summary>
        /// The verisoning tag.
        /// </summary>
        public string etag { get; set; }
    }
}
