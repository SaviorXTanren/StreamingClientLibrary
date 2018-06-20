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
using System.Threading.Tasks;
using System.Windows;

namespace Mixer.ChatSample.WPF
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

            List<OAuthClientScopeEnum> scopes = new List<OAuthClientScopeEnum>()
            {
                OAuthClientScopeEnum.chat__bypass_links,
                OAuthClientScopeEnum.chat__bypass_slowchat,
                OAuthClientScopeEnum.chat__change_ban,
                OAuthClientScopeEnum.chat__change_role,
                OAuthClientScopeEnum.chat__chat,
                OAuthClientScopeEnum.chat__connect,
                OAuthClientScopeEnum.chat__clear_messages,
                OAuthClientScopeEnum.chat__edit_options,
                OAuthClientScopeEnum.chat__giveaway_start,
                OAuthClientScopeEnum.chat__poll_start,
                OAuthClientScopeEnum.chat__poll_vote,
                OAuthClientScopeEnum.chat__purge,
                OAuthClientScopeEnum.chat__remove_message,
                OAuthClientScopeEnum.chat__timeout,
                OAuthClientScopeEnum.chat__view_deleted,
                OAuthClientScopeEnum.chat__whisper,

                OAuthClientScopeEnum.channel__details__self,
                OAuthClientScopeEnum.channel__update__self,

                OAuthClientScopeEnum.user__details__self,
                OAuthClientScopeEnum.user__log__self,
                OAuthClientScopeEnum.user__notification__self,
                OAuthClientScopeEnum.user__update__self,
            };

            this.connection = await MixerConnection.ConnectViaLocalhostOAuthBrowser(ConfigurationManager.AppSettings["ClientID"], scopes);

            if (this.connection != null)
            {
                this.user = await this.connection.Users.GetCurrentUser();
                this.channel = await this.connection.Channels.GetChannel(this.user.username);

                this.chatClient = await ChatClient.CreateFromChannel(this.connection, this.channel);

                this.chatClient.OnDisconnectOccurred += ChatClient_OnDisconnectOccurred;
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

        private async void ChatClient_OnDisconnectOccurred(object sender, System.Net.WebSockets.WebSocketCloseStatus e)
        {
            this.chatMessages.Add(new ChatMessage("SYSTEM", "Disconnection Occurred, attempting reconnection..."));

            System.Console.WriteLine();

            do
            {
                await Task.Delay(2500);
            }
            while (!await this.chatClient.Connect() && !await this.chatClient.Authenticate());

            this.chatMessages.Add(new ChatMessage("SYSTEM", "Reconnection successful"));
        }

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
