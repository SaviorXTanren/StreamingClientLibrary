using Mixer.Base;
using Mixer.Base.Clients;
using Mixer.Base.Model.Channel;
using Mixer.Base.Model.Chat;
using Mixer.Base.Model.User;
using Mixer.Sample;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Mixer.LargeChat
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
        private List<TextBlock> chatTextBlocks = new List<TextBlock>();
        private int viewerCount = 0;

        private SolidColorBrush backgroundColor = new SolidColorBrush(Color.FromRgb(16, 21, 33));

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

            this.connection = await MixerConnection.ConnectViaShortCode(clientID,
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

            if (this.connection != null)
            {
                this.user = await this.connection.Users.GetCurrentUser();
                this.channel = await this.connection.Channels.GetChannel(this.user.username);

                this.chatClient = await ChatClient.CreateFromChannel(this.connection, channel);

                this.chatClient.DisconnectOccurred += ChatClient_DisconnectOccurred;
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

                IEnumerable<ChatUserModel> users = await this.connection.Chats.GetUsers(this.channel);
                this.viewerCount = users.Count();
                this.CurrentViewersTextBlock.Text = this.viewerCount.ToString();

                if (await this.chatClient.Connect() && await this.chatClient.Authenticate())
                {
                    this.LoginGrid.Visibility = Visibility.Collapsed;
                    this.ChatGrid.Visibility = Visibility.Visible;
                }
            }

            this.LoginButton.IsEnabled = true;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.LoginGrid.Visibility = Visibility.Visible;
            this.ChatGrid.Visibility = Visibility.Collapsed;

            this.MainGrid.Background = this.backgroundColor;
        }

        private async void MainWindow_Closed(object sender, EventArgs e)
        {
            if (this.chatClient != null)
            {
                await this.chatClient.Disconnect();
            }
        }

        private void AddChatMessage(ChatMessage message)
        {
            this.chatMessages.Insert(0, message);

            TextBlock textBlock = new TextBlock();
            textBlock.Margin = new Thickness(0, 2, 0, 2);
            textBlock.FontSize = 24;
            textBlock.FontWeight = FontWeights.Bold;
            textBlock.TextWrapping = TextWrapping.Wrap;
            switch (message.Role)
            {
                case UserRole.Streamer:
                case UserRole.Mod:
                    textBlock.Foreground = Brushes.Green;
                    break;
                case UserRole.Subscriber:
                case UserRole.Pro:
                    textBlock.Foreground = Brushes.Purple;
                    break;
                case UserRole.User:
                    textBlock.Foreground = Brushes.White;
                    break;
            }
            textBlock.Text = message.ToString();
            this.chatTextBlocks.Insert(0, textBlock);

            this.ChatListStackPanel.Children.Insert(0, textBlock);
        }

        #region Chat Event Handler

        private void ChatClient_DisconnectOccurred(object sender, System.Net.WebSockets.WebSocketCloseStatus e)
        {
            this.AddChatMessage(new ChatMessage("ERROR", "DISCONNECTED FROM CHAT"));
        }

        private void ChatClient_MessageOccurred(object sender, ChatMessageEventModel e)
        {
            this.AddChatMessage(new ChatMessage(e, new ChatUser(e)));
        }

        private void ChatClient_UserJoinOccurred(object sender, ChatUserEventModel e)
        {
            this.viewerCount++;
            this.CurrentViewersTextBlock.Text = this.viewerCount.ToString();
        }

        private void ChatClient_UserLeaveOccurred(object sender, ChatUserEventModel e)
        {
            this.viewerCount--;
            this.CurrentViewersTextBlock.Text = this.viewerCount.ToString();
        }

        private void ChatClient_UserTimeoutOccurred(object sender, ChatUserEventModel e)
        {
            this.viewerCount--;
            this.CurrentViewersTextBlock.Text = this.viewerCount.ToString();
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
