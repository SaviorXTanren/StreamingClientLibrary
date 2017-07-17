using Mixer.Base;
using Mixer.Base.Clients;
using Mixer.Base.Model.Channel;
using Mixer.Base.Model.Interactive;
using Mixer.Base.Model.User;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows;

namespace Mixer.InteractiveSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MixerConnection connection;
        private ExpandedChannelModel channel;
        private PrivatePopulatedUserModel user;
        private InteractiveClient interactiveClient;

        private List<InteractiveGameListingModel> games;
        private InteractiveGameListingModel game;

        public MainWindow()
        {
            InitializeComponent();
            this.Closed += MainWindow_Closed;
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            this.LoginGrid.Visibility = Visibility.Visible;

            this.MainGrid.Visibility = Visibility.Collapsed;

            List<ClientScopeEnum> scopes = new List<ClientScopeEnum>()
            {
                ClientScopeEnum.channel__details__self,
                ClientScopeEnum.channel__update__self,

                ClientScopeEnum.interactive__manage__self,
                ClientScopeEnum.interactive__robot__self,

                ClientScopeEnum.user__details__self,
                ClientScopeEnum.user__log__self,
                ClientScopeEnum.user__notification__self,
                ClientScopeEnum.user__update__self,
            };

            this.connection = await MixerConnection.ConnectViaLocalhostOAuthBrowser(ConfigurationManager.AppSettings["ClientID"], scopes);

            if (this.connection != null)
            {
                this.user = await this.connection.Users.GetCurrentUser();
                this.channel = await this.connection.Channels.GetChannel(this.user.username);

                this.games = new List<InteractiveGameListingModel>(await this.connection.Interactive.GetOwnedInteractiveGames(this.channel));
                this.GameSelectComboBox.ItemsSource = this.games.Select(g => g.name);

                this.LoginGrid.Visibility = Visibility.Collapsed;

                this.GameSelectGrid.Visibility = Visibility.Visible;
            }
        }

        private async void GameSelectButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.GameSelectComboBox.SelectedIndex >= 0)
            {
                string gameName = (string)this.GameSelectComboBox.SelectedItem;
                this.game = this.games.FirstOrDefault(g => g.name.Equals(gameName));

                this.interactiveClient = await InteractiveClient.CreateFromChannel(this.connection, this.channel, this.game);

                if (await this.interactiveClient.Connect() && await this.interactiveClient.Ready())
                {
                    this.interactiveClient.OnGiveInput += InteractiveClient_OnGiveInput;
                    this.interactiveClient.OnGroupCreate += InteractiveClient_OnGroupCreate;
                    this.interactiveClient.OnGroupDelete += InteractiveClient_OnGroupDelete;
                    this.interactiveClient.OnGroupUpdate += InteractiveClient_OnGroupUpdate;
                    this.interactiveClient.OnIssueMemoryWarning += InteractiveClient_OnIssueMemoryWarning;
                    this.interactiveClient.OnParticipantJoin += InteractiveClient_OnParticipantJoin;
                    this.interactiveClient.OnParticipantLeave += InteractiveClient_OnParticipantLeave;
                    this.interactiveClient.OnParticipantUpdate += InteractiveClient_OnParticipantUpdate;

                    this.GameSelectGrid.Visibility = Visibility.Collapsed;

                    this.MainGrid.Visibility = Visibility.Visible;
                }
            }
        }

        private async void MainWindow_Closed(object sender, EventArgs e)
        {
            if (this.interactiveClient != null)
            {
                await this.interactiveClient.Disconnect();
            }
        }

        #region Event Methods

        private async void InteractiveClient_OnGiveInput(object sender, InteractiveGiveInputModel e)
        {
            this.InteractiveDataTextBlock.Text += "Input Received: " + e.participantID + " - " + e.input.@event + " - " + e.input.controlID + Environment.NewLine;
            if (e.input.@event.Equals("mousedown"))
            {
                if (await this.interactiveClient.CaptureSparkTransaction(e.transactionID))
                {
                    this.InteractiveDataTextBlock.Text += "Spark Transaction Captured: " + e.participantID + " - " + e.input.@event + " - " + e.input.controlID + Environment.NewLine;
                }
            }
        }

        private void InteractiveClient_OnGroupCreate(object sender, InteractiveGroupContainerModel e)
        {
            if (e.groups != null)
            {
                foreach (InteractiveGroupModel group in e.groups)
                {
                    this.InteractiveDataTextBlock.Text += "Group Created: " + group.groupID + Environment.NewLine;
                }
            }
        }

        private void InteractiveClient_OnGroupDelete(object sender, Tuple<InteractiveGroupModel, InteractiveGroupModel> e)
        {
            this.InteractiveDataTextBlock.Text += "Group Deleted: " + e.Item1 + " - " + e.Item2 + Environment.NewLine;
        }

        private void InteractiveClient_OnGroupUpdate(object sender, InteractiveGroupContainerModel e)
        {
            if (e.groups != null)
            {
                foreach (InteractiveGroupModel group in e.groups)
                {
                    this.InteractiveDataTextBlock.Text += "Group Updated: " + group.groupID + Environment.NewLine;
                }
            }
        }

        private void InteractiveClient_OnIssueMemoryWarning(object sender, InteractiveIssueMemoryWarningModel e)
        {
            this.InteractiveDataTextBlock.Text += "Memory Warning Issued: " + e.usedBytes + " bytes used out of " + e.totalBytes + " total bytes" + Environment.NewLine;
        }

        private void InteractiveClient_OnParticipantJoin(object sender, InteractiveParticipantChangedModel e)
        {
            if (e.participants != null)
            {
                foreach (InteractiveParticipantModel participant in e.participants)
                {
                    this.InteractiveDataTextBlock.Text += "Participant Joined: " + participant.username + Environment.NewLine;
                }
            }
        }

        private void InteractiveClient_OnParticipantLeave(object sender, InteractiveParticipantChangedModel e)
        {
            if (e.participants != null)
            {
                foreach (InteractiveParticipantModel participant in e.participants)
                {
                    this.InteractiveDataTextBlock.Text += "Participant Left: " + participant.username + Environment.NewLine;
                }
            }
        }

        private void InteractiveClient_OnParticipantUpdate(object sender, InteractiveParticipantChangedModel e)
        {
            if (e.participants != null)
            {
                foreach (InteractiveParticipantModel participant in e.participants)
                {
                    this.InteractiveDataTextBlock.Text += "Participant Updated: " + participant.username + Environment.NewLine;
                }
            }
        }

        #endregion Event Methods
    }
}
