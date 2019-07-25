namespace Mixer.Base.Model.MixPlay
{
    /// <summary>
    /// A connected instance of a MixPlay button control.
    /// </summary>
    public class MixPlayConnectedButtonControlModel : MixPlayButtonControlModel
    {
        /// <summary>
        /// The cooldown duration of the control.
        /// </summary>
        public long cooldown { get; set; }
        /// <summary>
        /// The progress state of the control.
        /// </summary>
        public float progress { get; set; }
    }

    /// <summary>
    /// An instance of a MixPlay button control.
    /// </summary>
    public class MixPlayButtonControlModel : MixPlayControlModel
    {
        /// <summary>
        /// The kind of MixPlay control
        /// </summary>
        public const string ButtonControlKind = "button";

        /// <summary>
        /// Creates a new instance of the MixPlayButtonControlModel class.
        /// </summary>
        public MixPlayButtonControlModel() { this.kind = ButtonControlKind; }

        /// <summary>
        /// The text to show.
        /// </summary>
        public string text { get; set; }
        /// <summary>
        /// The cost of the control.
        /// </summary>
        public int? cost { get; set; }
        /// <summary>
        /// The keyboard key code associated with the control.
        /// </summary>
        public int? keyCode { get; set; }

        /// <summary>
        /// The size of the text.
        /// </summary>
        public string textSize { get; set; }
        /// <summary>
        /// The color of the text.
        /// </summary>
        public string textColor { get; set; }
        /// <summary>
        /// The focus color of the button.
        /// </summary>
        public string focusColor { get; set; }
        /// <summary>
        /// The accent color of the button.
        /// </summary>
        public string accentColor { get; set; }
        /// <summary>
        /// The border color of the button.
        /// </summary>
        public string borderColor { get; set; }
        /// <summary>
        /// The background color of the button.
        /// </summary>
        public string backgroundColor { get; set; }
        /// <summary>
        /// The background image of the button.
        /// </summary>
        public string backgroundImage { get; set; }

        /// <summary>
        /// The tooltip text of the button.
        /// </summary>
        public string tooltip { get; set; }
    }
}
