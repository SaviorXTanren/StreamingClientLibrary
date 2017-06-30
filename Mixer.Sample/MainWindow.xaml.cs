using Mixer.Base;
using Mixer.Base.Clients;
using Mixer.Base.Model.Channel;
using Mixer.Base.Model.Chat;
using Mixer.Base.Model.User;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace Mixer.Sample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MixerConnection client;
        private ExpandedChannelModel channel;
        private PrivatePopulatedUserModel user;
        private ChatClient chatClient;

        private ObservableCollection<ChatMessage> chatMessages = new ObservableCollection<ChatMessage>();
        private ObservableCollection<ChatUser> chatUsers = new ObservableCollection<ChatUser>();

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            this.Closed += MainWindow_Closed;
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            this.LoginButton.IsEnabled = false;

            string clientID = ConfigurationManager.AppSettings["ClientID"];
            if (string.IsNullOrEmpty(clientID))
            {
                throw new ArgumentException("ClientID value isn't set in application configuration");
            }

            this.client = await MixerConnection.ConnectViaShortCode(clientID,
            new List<ClientScopeEnum>()
            {
                ClientScopeEnum.chat__chat,
                ClientScopeEnum.chat__connect,
                ClientScopeEnum.channel__details__self,
                ClientScopeEnum.channel__update__self,
                ClientScopeEnum.user__details__self,
                ClientScopeEnum.user__log__self,
                ClientScopeEnum.user__notification__self,
                ClientScopeEnum.user__update__self,
            },
            (string code) =>
            {
                this.ShortCodeTextBox.Text = code;
                Process.Start("https://mixer.com/oauth/shortcode");
            });

            if (this.client != null)
            {
                this.user = await this.client.Users.GetCurrentUser();
                this.channel = await this.client.Channels.GetChannel(this.user.username);

                this.chatClient = await ChatClient.CreateFromChannel(this.client, this.user.channel);

                this.chatClient.MessageOccurred += ChatClient_MessageOccurred;
                this.chatClient.UserJoinOccurred += ChatClient_UserJoinOccurred;
                this.chatClient.UserLeaveOccurred += ChatClient_UserLeaveOccurred;
                this.chatClient.UserTimeoutOccurred += ChatClient_UserTimeoutOccurred;
                this.chatClient.UserUpdateOccurred += ChatClient_UserUpdateOccurred;
                this.chatClient.PollStartOccurred += ChatClient_PollStartOccurred;
                this.chatClient.PollEndOccurred += ChatClient_PollEndOccurred;
                this.chatClient.PurgeMessageOccurred += ChatClient_PurgeMessageOccurred;
                this.chatClient.DeleteMessageOccurred += ChatClient_DeleteMessageOccurred;
                this.chatClient.ClearMessagesOccurred += ChatClient_ClearMessagesOccurred;

                if (await this.chatClient.Connect() && await this.chatClient.Authenticate())
                {
                    this.ChannelUserTextBlock.Text = this.user.username;
                    this.StreamTitleTextBox.Text = this.channel.name;
                    this.GameTitleTextBlock.Text = this.channel.type.name;

                    foreach (ChatUserModel user in await this.client.Chats.GetUsers(this.chatClient.Channel))
                    {
                        this.chatUsers.Add(new ChatUser(user));
                    }

                    this.LoginGrid.Visibility = Visibility.Collapsed;

                    this.MainGrid.Visibility = Visibility.Visible;
                }
            }

            this.LoginButton.IsEnabled = true;
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

                await this.client.Channels.UpdateChannel(this.channel);
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

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.LoginGrid.Visibility = Visibility.Visible;

            this.MainGrid.Visibility = Visibility.Collapsed;

            this.ChatListView.ItemsSource = this.chatMessages;
            this.UsersListView.ItemsSource = this.chatUsers;
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

        private void ChatClient_PurgeMessageOccurred(object sender, uint e)
        {

        }

        private void ChatClient_DeleteMessageOccurred(object sender, Guid e)
        {

        }

        private void ChatClient_ClearMessagesOccurred(object sender, EventArgs e)
        {

        }

        #endregion Chat Event Handlers
    }
}
