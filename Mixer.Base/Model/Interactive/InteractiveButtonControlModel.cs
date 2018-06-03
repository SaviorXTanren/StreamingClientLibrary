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
        public int? cost { get; set; }
        public int? keyCode { get; set; }

        public string textSize { get; set; }
        public string textColor { get; set; }
        public string focusColor { get; set; }
        public string accentColor { get; set; }
        public string borderColor { get; set; }
        public string backgroundColor { get; set; }
        public string backgroundImage { get; set; }

        public string tooltip { get; set; }
    }
}
