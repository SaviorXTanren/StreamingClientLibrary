namespace Mixer.Base.Model.MixPlay
{
    /// <summary>
    /// A connected instance of the Text Box MixPlay control
    /// </summary>
    public class MixPlayConnectedTextBoxControlModel : MixPlayTextBoxControlModel
    {
    }

    /// <summary>
    /// An Text Box MixPlay control.
    /// </summary>
    public class MixPlayTextBoxControlModel : MixPlayControlModel
    {
        /// <summary>
        /// The kind of MixPlay control.
        /// </summary>
        public const string TextBoxControlKind = "textbox";

        /// <summary>
        /// Creates a new instance of the MixPlayTextBoxControlModel class.
        /// </summary>
        public MixPlayTextBoxControlModel() { this.kind = TextBoxControlKind; }

        /// <summary>
        /// The cost of the MixPlay control.
        /// </summary>
        public int? cost { get; set; }

        /// <summary>
        /// The text to show in the submit button.
        /// </summary>
        public string submitText { get; set; }
        /// <summary>
        /// The placeholder text to show.
        /// </summary>
        public string placeholder { get; set; }

        /// <summary>
        /// Whether there is a submit button.
        /// </summary>
        public bool hasSubmit { get; set; }
        /// <summary>
        /// Whether the text box support multi-line text.
        /// </summary>
        public bool multiline { get; set; }
    }
}
