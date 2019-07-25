namespace Mixer.Base.Model.MixPlay
{
    /// <summary>
    /// A connected instance of a MixPlay joystick.
    /// </summary>
    public class MixPlayConnectedJoystickControlModel : MixPlayJoystickControlModel
    {
        /// <summary>
        /// The angle of the joystick.
        /// </summary>
        public double angle { get; set; }
        /// <summary>
        /// The intensity of the joystick.
        /// </summary>
        public double intensity { get; set; }
    }

    /// <summary>
    /// An instance of a MixPlay joystick.
    /// </summary>
    public class MixPlayJoystickControlModel : MixPlayControlModel
    {
        /// <summary>
        /// The kind of MixPlay control.
        /// </summary>
        public const string JoystickControlKind = "joystick";

        /// <summary>
        /// Creates a new instance of the MixPlayJoystickControlModel class.
        /// </summary>
        public MixPlayJoystickControlModel() { this.kind = JoystickControlKind; }

        /// <summary>
        /// The sample rate of the joystick.
        /// </summary>
        public int? sampleRate { get; set; }
    }
}
