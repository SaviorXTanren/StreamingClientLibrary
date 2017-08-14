namespace Mixer.Base.Model.Interactive
{
    public class InteractiveConnectedButtonControlModel : InteractiveButtonControlModel
    {
        public long cooldown { get; set; }
        public float progress { get; set; }
    }

    public class InteractiveButtonControlModel : InteractiveControlModel
    {
        public const string ButtonControlKind = "button";

        public InteractiveButtonControlModel() { this.kind = ButtonControlKind; }

        public string text { get; set; }
        public int cost { get; set; }
        public int keyCode { get; set; }
    }
}
