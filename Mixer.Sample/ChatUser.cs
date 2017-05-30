using Mixer.Base.Model.Chat;
using Mixer.Base.Model.User;

namespace Mixer.Sample
{
    public class ChatUser
    {
        public uint ID { get; private set; }

        public string UserName { get; private set; }

        public ChatUser(ChatUserModel user)
        {
            this.ID = user.userId.GetValueOrDefault();
            this.UserName = user.userName;
        }

        public ChatUser(ChatUserEventModel userEvent)
        {
            this.ID = userEvent.id;
            this.UserName = userEvent.username;
        }

        public override string ToString() { return string.Format("{0}", this.UserName); }
    }
}
