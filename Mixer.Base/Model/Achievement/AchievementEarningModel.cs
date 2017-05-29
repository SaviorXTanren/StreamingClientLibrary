namespace Mixer.Base.Model.Achievement
{
    public class AchievementEarningModel : TimeStampedModel
    {
        public uint id { get; set; }
        public bool earned { get; set; }
        public int progress { get; set; }
        public string achievement { get; set; }
        public uint user { get; set; }
        public AchievementModel Achievement { get; set; }
    }
}
