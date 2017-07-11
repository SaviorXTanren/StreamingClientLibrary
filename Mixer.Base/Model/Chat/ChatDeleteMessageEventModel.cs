using System;

namespace Mixer.Base.Model.Chat
{
    public class ChatDeleteMessageEventModel
    {
        public Guid id { get; set; }
        public ChatMessageUserModel moderator { get; set; }
    }
}
