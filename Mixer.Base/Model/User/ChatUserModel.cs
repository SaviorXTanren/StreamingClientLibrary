namespace Mixer.Base.Model.User
{
    public class ChatUserModel
    {
        public uint? userId { get; set; }
        public string userName { get; set; }
        public string[] userRoles { get; set; }
        public bool? lurking { get; set; }
    }
}
