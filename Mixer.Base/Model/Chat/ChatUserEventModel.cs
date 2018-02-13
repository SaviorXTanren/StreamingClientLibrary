namespace Mixer.Base.Model.Chat
{
    public class ChatUserEventModel
    {
        public uint id { get; set; }
        public string username { get; set; }
        public string[] roles { get; set; }
        public string[] permissions { get; set; }

        public uint user { get { return id; } set { this.id = value; } }
    }
}
