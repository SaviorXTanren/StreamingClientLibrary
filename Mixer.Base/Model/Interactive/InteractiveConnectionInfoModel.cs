namespace Mixer.Base.Model.Interactive
{
    public class InteractiveConnectionInfoModel
    {
        public string address { get; set; }
        public string key { get; set; }
        public int? influence { get; set; }
        public InteractiveVersionModel version { get; set; }
    }
}
