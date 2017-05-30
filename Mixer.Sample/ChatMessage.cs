using Mixer.Base.Model.Chat;
using System;

namespace Mixer.Sample
{
    public class ChatMessage
    {
        public string UserName { get; private set; }

        public string Message { get; private set; }

        public DateTimeOffset Timestamp { get; private set; }

        private ChatMessageEventModel chatMessageEvent;

        public ChatMessage(ChatMessageEventModel chatMessageEvent)
        {
            this.chatMessageEvent = chatMessageEvent;

            this.UserName = this.chatMessageEvent.user_name;
            this.Timestamp = DateTimeOffset.Now;
            this.Message = string.Empty;
            foreach (ChatMessageDataModel message in this.chatMessageEvent.message.message)
            {
                this.Message += message.text;
            }
        }

        public override string ToString() { return string.Format("[{0}] {1}: {2}", this.Timestamp.ToString("HH:mm"), this.UserName, this.Message); }
    }
}
