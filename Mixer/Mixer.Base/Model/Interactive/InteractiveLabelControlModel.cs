namespace Mixer.Base.Model.Interactive
{
    public class InteractiveConnectedLabelControlModel : InteractiveLabelControlModel
    {
    }

    public class InteractiveLabelControlModel : InteractiveControlModel
    {
        public const string LabelControlKind = "label";

        public InteractiveLabelControlModel() { this.kind = LabelControlKind; }

        public string text { get; set; }

        public bool bold { get; set; }
        public bool italic { get; set; }
        public bool underline { get; set; }
        public string textSize { get; set; }
        public string textColor { get; set; }
    }
}
