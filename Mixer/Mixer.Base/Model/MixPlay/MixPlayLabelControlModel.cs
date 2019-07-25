namespace Mixer.Base.Model.MixPlay
{
    /// <summary>
    /// A connected instance of a MixPlay label.
    /// </summary>
    public class MixPlayConnectedLabelControlModel : MixPlayLabelControlModel
    {
    }

    /// <summary>
    /// An instance of a MixPlay label.
    /// </summary>
    public class MixPlayLabelControlModel : MixPlayControlModel
    {
        /// <summary>
        /// The kind of MixPlay control.
        /// </summary>
        public const string LabelControlKind = "label";

        /// <summary>
        /// Creates a new instance of the MixPlayLabelControlModel class.
        /// </summary>
        public MixPlayLabelControlModel() { this.kind = LabelControlKind; }

        /// <summary>
        /// The text of the label.
        /// </summary>
        public string text { get; set; }

        /// <summary>
        /// Whether the text is bolded.
        /// </summary>
        public bool bold { get; set; }
        /// <summary>
        /// Whether the text is italicized.
        /// </summary>
        public bool italic { get; set; }
        /// <summary>
        /// Whether the text is underlined.
        /// </summary>
        public bool underline { get; set; }
        /// <summary>
        /// The size of the text.
        /// </summary>
        public string textSize { get; set; }
        /// <summary>
        /// The color of the text.
        /// </summary>
        public string textColor { get; set; }
    }
}
