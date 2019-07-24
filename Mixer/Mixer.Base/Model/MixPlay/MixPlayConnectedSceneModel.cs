using System.Collections.Generic;

namespace Mixer.Base.Model.MixPlay
{
    /// <summary>
    /// A connected MixPlay set of MixPlay scenes with groups.
    /// </summary>
    public class MixPlayConnectedSceneGroupCollectionModel
    {
        /// <summary>
        /// Creates a new instance of the MixPlayConnectedSceneGroupCollectionModel class.
        /// </summary>
        public MixPlayConnectedSceneGroupCollectionModel() { this.scenes = new List<MixPlayConnectedSceneGroupModel>(); }

        /// <summary>
        /// The set of scenes.
        /// </summary>
        public List<MixPlayConnectedSceneGroupModel> scenes { get; set; }
    }

    /// <summary>
    /// A connected MixPlay scene with a set of groups.
    /// </summary>
    public class MixPlayConnectedSceneGroupModel : MixPlayConnectedSceneModel
    {
        /// <summary>
        /// The set of groups.
        /// </summary>
        public List<MixPlayGroupModel> groups { get; set; }
    }

    /// <summary>
    /// A connected set of MixPlay scenes.
    /// </summary>
    public class MixPlayConnectedSceneCollectionModel
    {
        /// <summary>
        /// Creates a new instance of the MixPlayConnectedSceneCollectionModel class.
        /// </summary>
        public MixPlayConnectedSceneCollectionModel() { this.scenes = new List<MixPlayConnectedSceneModel>(); }

        /// <summary>
        /// The set of scenes.
        /// </summary>
        public List<MixPlayConnectedSceneModel> scenes { get; set; }
    }

    /// <summary>
    /// A connected MixPlay scene.
    /// </summary>
    public class MixPlayConnectedSceneModel : MixPlayConnectedControlCollectionModel
    {
        /// <summary>
        /// Creates a new instance of the MixPlayConnectedSceneModel class.
        /// </summary>
        public MixPlayConnectedSceneModel() { }

        /// <summary>
        /// The ID of the scene.
        /// </summary>
        public string sceneID { get; set; }
    }
}
