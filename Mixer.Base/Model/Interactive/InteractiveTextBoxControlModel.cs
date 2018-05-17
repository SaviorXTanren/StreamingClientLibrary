namespace Mixer.Base.Model.Interactive
{
    public class InteractiveConnectedTextBoxControlModel : InteractiveTextBoxControlModel
    {
    }

    public class InteractiveTextBoxControlModel : InteractiveControlModel
    {
        public const string TextBoxControlKind = "textbox";

        public InteractiveTextBoxControlModel() { this.kind = TextBoxControlKind; }

        public int? cost { get; set; }

        public string submitText { get; set; }
        public string placeholder { get; set; }

        public bool hasSubmit { get; set; }
        public bool multiline { get; set; }
    }
}
