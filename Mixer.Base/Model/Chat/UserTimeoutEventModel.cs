namespace Mixer.Base.Model.Chat
{
    public class UserTimeoutEventModel
    {
        public UserEventModel user { get; set; }
        public uint duration { get; set; }
    }
}
