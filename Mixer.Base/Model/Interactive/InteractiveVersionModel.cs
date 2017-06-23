namespace Mixer.Base.Model.Interactive
{
    public class InteractiveVersionModel : TimeStampedModel
    {
        public uint id { get; set; }
        public uint gameId { get; set; }
        public string version { get; set; }
        public uint versionOrder { get; set; }
        public string changelog { get; set; }
        public string state { get; set; }
        public string installation { get; set; }
        public string download { get; set; }
        public InteractiveGameControlsModel controls { get; set; }
        public string controlVersion { get; set; }
        public InteractiveGameModel game { get; set; }
    }
}
