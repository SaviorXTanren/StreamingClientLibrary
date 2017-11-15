using Mixer.Base.Model.Chat;
using System;
using System.Windows;

namespace Mixer.ChatSample.WPF
{
    public class ChatMessage
    {
        public string UserName { get; private set; }

        public string Message { get; private set; }

        public DateTimeOffset Timestamp { get; private set; }

        public ChatUser User { get; private set; }

        private ChatMessageEventModel chatMessageEvent;

        public ChatMessage(ChatMessageEventModel chatMessageEvent, ChatUser user)
            : this(chatMessageEvent)
        {
            this.User = user;
        }

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

        public ChatMessage(string userName, string message)
        {
            this.UserName = userName;
            this.Timestamp = DateTimeOffset.Now;
            this.Message = message;
        }

        public UserRole Role { get { return (this.User != null) ? this.User.Role : UserRole.User; } }

        public Visibility UserMessageVisibility { get { return (this.Role == UserRole.User) ? Visibility.Visible : Visibility.Collapsed; } }
        public Visibility ProMessageVisibility { get { return (this.Role == UserRole.Pro) ? Visibility.Visible : Visibility.Collapsed; } }
        public Visibility SubscriberMessageVisibility { get { return (this.Role == UserRole.Subscriber) ? Visibility.Visible : Visibility.Collapsed; } }
        public Visibility ModMessageVisibility { get { return (this.Role == UserRole.Mod) ? Visibility.Visible : Visibility.Collapsed; } }
        public Visibility StreamerMessageVisibility { get { return (this.Role == UserRole.Streamer) ? Visibility.Visible : Visibility.Collapsed; } }

        public override string ToString() { return string.Format("{0}: {1}", this.UserName, this.Message); }
    }
}
