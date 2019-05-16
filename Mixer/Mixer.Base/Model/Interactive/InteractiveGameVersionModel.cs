namespace Mixer.Base.Model.Interactive
{
    public class InteractiveGameVersionUpdateableModel : TimeStampedModel
    {
        public InteractiveGameVersionUpdateableModel()
        {
            this.controls = new InteractiveSceneCollectionModel();
        }

        public string state { get; set; }
        public string installation { get; set; }
        public string download { get; set; }
        public InteractiveSceneCollectionModel controls { get; set; }
        public string controlVersion { get; set; }
    }

    public class InteractiveGameVersionModel : InteractiveGameVersionUpdateableModel
    {
        public uint id { get; set; }
        public uint gameId { get; set; }
        public string version { get; set; }
        public uint versionOrder { get; set; }
        public string changelog { get; set; }
    }
}
