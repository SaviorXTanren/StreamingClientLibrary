using Mixer.Base;
using Mixer.Base.Clients;
using Mixer.Base.Model.Channel;
using Mixer.Base.Model.Chat;
using Mixer.Base.Model.User;
using Mixer.ChatSample.WPF;
using StreamingClient.Base.Model.OAuth;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Mixer.LargeChat.WPF
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
            new List<OAuthClientScopeEnum>()
            {
                OAuthClientScopeEnum.chat__chat,
                OAuthClientScopeEnum.chat__connect,
                OAuthClientScopeEnum.channel__details__self,
                OAuthClientScopeEnum.channel__update__self,
                OAuthClientScopeEnum.user__details__self,
                OAuthClientScopeEnum.user__log__self,
                OAuthClientScopeEnum.user__notification__self,
                OAuthClientScopeEnum.user__update__self,
            },
            (OAuthShortCodeModel code) =>
            {
                this.ShortCodeTextBox.Text = code.code;
                Process.Start("https://mixer.com/oauth/shortcode");
            });

            if (this.connection != null)
            {
                this.user = await this.connection.Users.GetCurrentUser();
                this.channel = await this.connection.Channels.GetChannel(this.user.username);

                this.chatClient = await ChatClient.CreateFromChannel(this.connection, channel);

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
