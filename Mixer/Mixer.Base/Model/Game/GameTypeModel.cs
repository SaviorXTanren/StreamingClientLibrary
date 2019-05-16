namespace Mixer.Base.Model.Game
{
    public class GameTypeModel : GameTypeSimpleModel
    {
        public string parent { get; set; }
        public string description { get; set; }
        public string source { get; set; }
        public uint viewersCurrent { get; set; }
        public uint online { get; set; }
    }
}
