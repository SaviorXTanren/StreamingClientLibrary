namespace Mixer.Base.Model.Game
{
    public class GameTypeSimpleModel
    {
        public uint id { get; set; }
        public string name { get; set; }
        public string coverUrl { get; set; }
        public string backgroundUrl { get; set; }
        public bool exact { get; set; }
    }
}
