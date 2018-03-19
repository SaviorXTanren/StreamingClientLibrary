namespace Mixer.Base.Model.Interactive
{
    public class InteractiveConnectedJoystickControlModel : InteractiveJoystickControlModel
    {
        public double angle { get; set; }
        public double intensity { get; set; }
    }

    public class InteractiveJoystickControlModel : InteractiveControlModel
    {
        public const string JoystickControlKind = "joystick";

        public InteractiveJoystickControlModel() { this.kind = JoystickControlKind; }

        public int? sampleRate { get; set; }
    }
}
