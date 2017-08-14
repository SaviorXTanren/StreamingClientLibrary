namespace Mixer.Base.Model.Interactive
{
    public class InteractiveGameModel : TimeStampedModel
    {
        public uint id { get; set; }
        public uint ownerId { get; set; }
        public string name { get; set; }
        public string coverUrl { get; set; }
        public string description { get; set; }
        public bool hasPublishedVersions { get; set; }
        public string installation { get; set; }
        public string controlVersion { get; set; }
    }
}
