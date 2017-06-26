using Mixer.Base;
using Mixer.Base.Model.Channel;
using Mixer.Base.Model.Interactive;
using Mixer.Base.Model.User;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Windows;

namespace Mixer.Interactive
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MixerClient client;
        private ExpandedChannelModel channel;
        private PrivatePopulatedUserModel user;
        private InteractiveClient interactiveClient;
        private string versionID;

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            this.Closed += MainWindow_Closed;
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            this.LoginButton.IsEnabled = false;

            versionID = VersionTextBox.Text;

            string clientID = ConfigurationManager.AppSettings["ClientID"];

            if (string.IsNullOrEmpty(clientID))
            {
                throw new ArgumentException("ClientID value isn't set in application configuration");
            }

            this.client = await MixerClient.ConnectViaShortCode(clientID,
            new List<ClientScopeEnum>()
            {
             ClientScopeEnum.interactive__robot__self,
             ClientScopeEnum.interactive__manage__self
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

                InteractiveGameListingModel game = new InteractiveGameListingModel()
                {
                    versions = new InteractiveVersionModel[] { new InteractiveVersionModel() { id = 70104 } }
                };

                this.interactiveClient = await InteractiveClient.CreateFromChannel(this.client, this.channel, game); //ToDo - Prompt For Version Number

                this.interactiveClient.ReplyOccurred += InteractiveClient_ReplyOccurred;
                this.interactiveClient.MethodOccurred += InteractiveClient_MethodOccured;

                this.interactiveClient.IssueMemoryWarningOccurred += InteractiveClient_IssueMemoryWarningOccurred;
                this.interactiveClient.OnParticipantLeaveOccurred += InteractiveClient_OnParticipantLeaveOccurred;
                this.interactiveClient.OnParticipantJoinOccurred += InteractiveClient_OnParticipantJoinOccurred;
                this.interactiveClient.OnParticipantUpdateOccurred += InteractiveClient_OnParticipantUpdateOccurred;
                this.interactiveClient.OnReplyGetScenes += InteractiveClient_OnReplyGetScenes;

                this.interactiveClient.OnError += InteractiveClient_OnError;

                if (await this.interactiveClient.Connect() && await this.interactiveClient.Ready())
                {
                    this.LoginGrid.Visibility = Visibility.Collapsed;

                    this.MainGrid.Visibility = Visibility.Visible;
                }
            }

            this.LoginButton.IsEnabled = true;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.LoginGrid.Visibility = Visibility.Visible;

            this.MainGrid.Visibility = Visibility.Collapsed;
        }

        private async void MainWindow_Closed(object sender, EventArgs e)
        {
            if (this.interactiveClient != null)
            {
                await this.interactiveClient.Disconnect();
            }
        }

        #region Interactive Event Handler

        private void InteractiveClient_ReplyOccurred(object sender, InteractiveReplyPacket e)
        {
        }

        private void InteractiveClient_MethodOccured(object sender, InteractiveMethodPacket e)
        {
        }

        private void InteractiveClient_IssueMemoryWarningOccurred(object sender, InteractiveIssueMemoryWarningModel e)
        {
        }

        private void InteractiveClient_OnParticipantLeaveOccurred(object sender, InteractiveOnParticipantLeaveModel e)
        {
        }

        private void InteractiveClient_OnParticipantJoinOccurred(object sender, InteractiveOnParticipantJoinModel e)
        {
        }

        private void InteractiveClient_OnParticipantUpdateOccurred(object sender, InteractiveOnParticipantUpdateModel e)
        {
        }

        private void InteractiveClient_OnReplyGetScenes(object sender, InteractiveGetScenes e)
        {
            InteractiveGetScenes scenes = e;
        }

        private void InteractiveClient_OnError(object sender, InteractiveError e)
        {
            InteractiveError error = e;
        }

        #endregion Interactive Event Handler

        private void GetScenes_Click(object sender, RoutedEventArgs e)
        {
            interactiveClient.GetScenes().Wait();
        }
    }
}