using Mixer.Base;
using Mixer.Base.Clients;
using Mixer.Base.Model.Channel;
using Mixer.Base.Model.Chat;
using Mixer.Base.Model.User;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Windows;

namespace Mixer.Sample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MixerConnection connection;
        private ExpandedChannelModel channel;
        private PrivatePopulatedUserModel user;
        private ChatClient chatClient;

        private ObservableCollection<ChatMessage> chatMessages = new ObservableCollection<ChatMessage>();
        private ObservableCollection<ChatUser> chatUsers = new ObservableCollection<ChatUser>();

        public MainWindow()
        {
            InitializeComponent();
            this.Closed += MainWindow_Closed;
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            this.LoginGrid.Visibility = Visibility.Visible;

            this.MainGrid.Visibility = Visibility.Collapsed;

            this.ChatListView.ItemsSource = this.chatMessages;
            this.UsersListView.ItemsSource = this.chatUsers;

            List<ClientScopeEnum> scopes = new List<ClientScopeEnum>()
            {
                ClientScopeEnum.chat__bypass_links,
                ClientScopeEnum.chat__bypass_slowchat,
                ClientScopeEnum.chat__change_ban,
                ClientScopeEnum.chat__change_role,
                ClientScopeEnum.chat__chat,
                ClientScopeEnum.chat__connect,
                ClientScopeEnum.chat__clear_messages,
                ClientScopeEnum.chat__edit_options,
                ClientScopeEnum.chat__giveaway_start,
                ClientScopeEnum.chat__poll_start,
                ClientScopeEnum.chat__poll_vote,
                ClientScopeEnum.chat__purge,
                ClientScopeEnum.chat__remove_message,
                ClientScopeEnum.chat__timeout,
                ClientScopeEnum.chat__view_deleted,
                ClientScopeEnum.chat__whisper,

                ClientScopeEnum.channel__details__self,
                ClientScopeEnum.channel__update__self,

                ClientScopeEnum.user__details__self,
                ClientScopeEnum.user__log__self,
                ClientScopeEnum.user__notification__self,
                ClientScopeEnum.user__update__self,
            };

            this.connection = await MixerConnection.ConnectViaLocalhostOAuthBrowser(ConfigurationManager.AppSettings["ClientID"], scopes);

            if (this.connection != null)
            {
                this.user = await this.connection.Users.GetCurrentUser();
                this.channel = await this.connection.Channels.GetChannel("ChannelOne");

                this.chatClient = await ChatClient.CreateFromChannel(this.connection, this.channel);

                this.chatClient.OnMessageOccurred += ChatClient_MessageOccurred;
                this.chatClient.OnUserJoinOccurred += ChatClient_UserJoinOccurred;
                this.chatClient.OnUserLeaveOccurred += ChatClient_UserLeaveOccurred;
                this.chatClient.OnUserTimeoutOccurred += ChatClient_UserTimeoutOccurred;
                this.chatClient.OnUserUpdateOccurred += ChatClient_UserUpdateOccurred;
                this.chatClient.OnPollStartOccurred += ChatClient_PollStartOccurred;
                this.chatClient.OnPollEndOccurred += ChatClient_PollEndOccurred;
                this.chatClient.OnPurgeMessageOccurred += ChatClient_PurgeMessageOccurred;
                this.chatClient.OnDeleteMessageOccurred += ChatClient_DeleteMessageOccurred;
                this.chatClient.OnClearMessagesOccurred += ChatClient_ClearMessagesOccurred;

                if (await this.chatClient.Connect() && await this.chatClient.Authenticate())
                {
                    this.ChannelUserTextBlock.Text = this.user.username;
                    this.StreamTitleTextBox.Text = this.channel.name;
                    this.GameTitleTextBlock.Text = this.channel.type.name;

                    foreach (ChatUserModel user in await this.connection.Chats.GetUsers(this.chatClient.Channel))
                    {
                        this.chatUsers.Add(new ChatUser(user));
                    }

                    this.LoginGrid.Visibility = Visibility.Collapsed;

                    this.MainGrid.Visibility = Visibility.Visible;
                }
            }
        }

        private async void UpdateStreamButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.StreamTitleTextBox.Text))
            {
                if (this.StreamTitleTextBox.Text.Length > 80)
                {
                    MessageBox.Show("Stream Title Error", "Stream titles must be 80 characters or less", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                this.channel.name = this.StreamTitleTextBox.Text;

                await this.connection.Channels.UpdateChannel(this.channel);
            }
        }

        private async void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.MessageTextBox.Text))
            {
                await this.chatClient.SendMessage(this.MessageTextBox.Text);
                this.MessageTextBox.Text = string.Empty;
            }
        }

        private async void MainWindow_Closed(object sender, EventArgs e)
        {
            if (this.chatClient != null)
            {
                await this.chatClient.Disconnect();
            }
        }

        #region Chat Event Handler

        private void ChatClient_MessageOccurred(object sender, ChatMessageEventModel e)
        {
            this.chatMessages.Add(new ChatMessage(e));
        }

        private void ChatClient_UserJoinOccurred(object sender, ChatUserEventModel e)
        {
            ChatUser user = this.chatUsers.FirstOrDefault(u => u.ID.Equals(e.id));
            if (user == null)
            {
                this.chatUsers.Add(new ChatUser(e));
            }
        }

        private void ChatClient_UserLeaveOccurred(object sender, ChatUserEventModel e)
        {
            ChatUser user = this.chatUsers.FirstOrDefault(u => u.ID.Equals(e.id));
            if (user != null)
            {
                this.chatUsers.Remove(user);
            }
        }

        private void ChatClient_UserTimeoutOccurred(object sender, ChatUserEventModel e)
        {

        }

        private void ChatClient_UserUpdateOccurred(object sender, ChatUserEventModel e)
        {

        }

        private void ChatClient_PollStartOccurred(object sender, ChatPollEventModel e)
        {

        }

        private void ChatClient_PollEndOccurred(object sender, ChatPollEventModel e)
        {

        }

        private void ChatClient_PurgeMessageOccurred(object sender, ChatPurgeMessageEventModel e)
        {

        }

        private void ChatClient_DeleteMessageOccurred(object sender, ChatDeleteMessageEventModel e)
        {

        }

        private void ChatClient_ClearMessagesOccurred(object sender, ChatClearMessagesEventModel e)
        {

        }

        #endregion Chat Event Handlers
    }
}
