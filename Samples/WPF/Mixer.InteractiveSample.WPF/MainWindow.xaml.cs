using Mixer.Base;
using Mixer.Base.Clients;
using Mixer.Base.Model.Channel;
using Mixer.Base.Model.Interactive;
using Mixer.Base.Model.User;
using Mixer.Base.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows;

namespace Mixer.InteractiveSample.WPF
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

        private List<InteractiveConnectedSceneModel> scenes = new List<InteractiveConnectedSceneModel>();

        private List<InteractiveConnectedButtonControlModel> buttons = new List<InteractiveConnectedButtonControlModel>();
        private List<InteractiveConnectedJoystickControlModel> joysticks = new List<InteractiveConnectedJoystickControlModel>();
        private List<InteractiveConnectedLabelControlModel> labels = new List<InteractiveConnectedLabelControlModel>();
        private List<InteractiveConnectedTextBoxControlModel> textBoxes = new List<InteractiveConnectedTextBoxControlModel>();

        public MainWindow()
        {
            InitializeComponent();
            this.Closed += MainWindow_Closed;
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            this.LoginGrid.Visibility = Visibility.Visible;

            this.MainGrid.Visibility = Visibility.Collapsed;

            List<OAuthClientScopeEnum> scopes = new List<OAuthClientScopeEnum>()
            {
                OAuthClientScopeEnum.channel__details__self,
                OAuthClientScopeEnum.channel__update__self,

                OAuthClientScopeEnum.interactive__manage__self,
                OAuthClientScopeEnum.interactive__robot__self,

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
                    this.interactiveClient.OnDisconnectOccurred += InteractiveClient_OnDisconnectOccurred;
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

                    InteractiveConnectedSceneGroupCollectionModel scenes = await this.interactiveClient.GetScenes();
                    if (scenes != null)
                    {
                        this.scenes = new List<InteractiveConnectedSceneModel>(scenes.scenes);

                        foreach (InteractiveConnectedSceneModel scene in this.scenes)
                        {
                            if (scene.allControls.Count() > 0)
                            {
                                foreach (InteractiveConnectedButtonControlModel button in scene.buttons)
                                {
                                    this.buttons.Add(button);
                                }

                                foreach (InteractiveConnectedJoystickControlModel joystick in scene.joysticks)
                                {
                                    this.joysticks.Add(joystick);
                                }

                                foreach (InteractiveConnectedLabelControlModel label in scene.labels)
                                {
                                    this.labels.Add(label);
                                }

                                foreach (InteractiveConnectedTextBoxControlModel textBox in scene.textBoxes)
                                {
                                    this.textBoxes.Add(textBox);
                                }

                                foreach (InteractiveControlModel control in scene.allControls)
                                {
                                    control.disabled = false;
                                }

                                await this.interactiveClient.UpdateControls(scene, scene.allControls);
                            }
                        }
                    }

                    DateTimeOffset? dateTime = await this.interactiveClient.GetTime();
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

        private async void InteractiveClient_OnDisconnectOccurred(object sender, System.Net.WebSockets.WebSocketCloseStatus e)
        {
            this.InteractiveDataTextBlock.Text += "Disconnection Occurred, attempting reconnection..." + Environment.NewLine;

            do
            {
                await InteractiveClient.Reconnect(this.interactiveClient);
            } while (!await this.interactiveClient.Ready());

            this.InteractiveDataTextBlock.Text += "Reconnection successful" + Environment.NewLine;
        }

        private async void InteractiveClient_OnGiveInput(object sender, InteractiveGiveInputModel e)
        {
            this.InteractiveDataTextBlock.Text += "Input Received: " + e.participantID + " - " + e.input.eventType + " - " + e.input.controlID + Environment.NewLine;

            if (e.input.eventType.Equals("mousedown") && e.transactionID != null)
            {
                InteractiveConnectedButtonControlModel button = this.buttons.FirstOrDefault(b => b.controlID.Equals(e.input.controlID));
                if (button != null)
                {
                    InteractiveConnectedSceneModel scene = this.scenes.FirstOrDefault(s => s.buttons.Contains(button));
                    if (scene != null)
                    {
                        button.cooldown = DateTimeHelper.DateTimeOffsetToUnixTimestamp(DateTimeOffset.Now.AddSeconds(10));
                        await this.interactiveClient.UpdateControls(scene, new List<InteractiveConnectedButtonControlModel>() { button });
                        this.InteractiveDataTextBlock.Text += "Sent 10 second cooldown to button: " + e.input.controlID + Environment.NewLine;
                    }
                }

                await this.interactiveClient.CaptureSparkTransaction(e.transactionID);
                this.InteractiveDataTextBlock.Text += "Spark Transaction Captured: " + e.participantID + " - " + e.input.eventType + " - " + e.input.controlID + Environment.NewLine;
            }
        }

        private void InteractiveClient_OnGroupCreate(object sender, InteractiveGroupCollectionModel e)
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

        private void InteractiveClient_OnGroupUpdate(object sender, InteractiveGroupCollectionModel e)
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

        private void InteractiveClient_OnParticipantJoin(object sender, InteractiveParticipantCollectionModel e)
        {
            if (e.participants != null)
            {
                foreach (InteractiveParticipantModel participant in e.participants)
                {
                    this.InteractiveDataTextBlock.Text += "Participant Joined: " + participant.username + Environment.NewLine;
                }
            }
        }

        private void InteractiveClient_OnParticipantLeave(object sender, InteractiveParticipantCollectionModel e)
        {
            if (e.participants != null)
            {
                foreach (InteractiveParticipantModel participant in e.participants)
                {
                    this.InteractiveDataTextBlock.Text += "Participant Left: " + participant.username + Environment.NewLine;
                }
            }
        }

        private void InteractiveClient_OnParticipantUpdate(object sender, InteractiveParticipantCollectionModel e)
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
