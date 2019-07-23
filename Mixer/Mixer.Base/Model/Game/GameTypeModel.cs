namespace Mixer.Base.Model.Game
{
    /// <summary>
    /// A GameType can be set on a channel and represents the title they are broadcasting.
    /// </summary>
    public class GameTypeModel : GameTypeSimpleModel
    {
        /// <summary>
        /// The name of the parent type.
        /// </summary>
        public string parent { get; set; }
        /// <summary>
        /// The description of the type.
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// The source where the type has been imported from.
        /// </summary>
        public string source { get; set; }
        /// <summary>
        /// Total amount of users watching this type of stream.
        /// </summary>
        public uint viewersCurrent { get; set; }
        /// <summary>
        /// Amount of streams online with this type.
        /// </summary>
        public uint online { get; set; }
    }
}
