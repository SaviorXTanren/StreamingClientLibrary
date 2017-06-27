using Mixer.Base;
using Mixer.Base.Model.Channel;
using Mixer.Base.Model.Interactive;
using Mixer.Base.Model.User;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
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
        private uint versionID;

        private ObservableCollection<Reply> replies = new ObservableCollection<Reply>();
        private ObservableCollection<Participant> participants = new ObservableCollection<Participant>();

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            this.Closed += MainWindow_Closed;
        }


        #region UI Events 

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.LoginGrid.Visibility = Visibility.Visible;
            this.MainGrid.Visibility = Visibility.Collapsed;

            this.ParticipantList.ItemsSource = participants;
            this.Replies.ItemsSource = replies;
        }

        private async void MainWindow_Closed(object sender, EventArgs e)
        {
            if (this.interactiveClient != null)
            {
                await this.interactiveClient.Disconnect();
            }
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            this.LoginButton.IsEnabled = false;

            versionID = Convert.ToUInt32(VersionTextBox.Text);

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
                    versions = new InteractiveVersionModel[] { new InteractiveVersionModel() { id = versionID } }
                };

                this.interactiveClient = await InteractiveClient.CreateFromChannel(this.client, this.channel, game); //ToDo - Prompt For Version Number

                this.interactiveClient.OnReply += InteractiveClient_OnReply;
                this.interactiveClient.OnMethod += InteractiveClient_OnMethod;
                this.interactiveClient.OnError += InteractiveClient_OnError;

                this.interactiveClient.OnIssueMemoryWarning += InteractiveClient_OnIssueMemoryWarning;
                this.interactiveClient.OnReplyGetScenes += InteractiveClient_OnReplyGetScenes;

                this.interactiveClient.OnParticipantLeave += InteractiveClient_OnParticipantLeave;
                this.interactiveClient.OnParticipantJoin += InteractiveClient_OnParticipantJoin;
                this.interactiveClient.OnParticipantUpdate += InteractiveClient_OnParticipantUpdate;
                this.interactiveClient.OnReplyGetAllParticipants += InteractiveClient_OnReplyGetAllParticipants;

                if (await this.interactiveClient.Connect() && await this.interactiveClient.Ready())
                {
                    this.LoginGrid.Visibility = Visibility.Collapsed;
                    this.MainGrid.Visibility = Visibility.Visible;
                }
            }

            this.LoginButton.IsEnabled = true;
        }

        private void GetScenes_Click(object sender, RoutedEventArgs e)
        {
            interactiveClient.GetScenes().Wait(200);
        }

        private void GetAllParticipants_Click(object sender, RoutedEventArgs e)
        {
            interactiveClient.GetAllParticipants().Wait(200);
        }

        #endregion

        #region Interactive Event Handler

        private void InteractiveClient_OnReply(object sender, InteractiveReplyPacket e)
        {
            replies.Insert(0, new Reply() { id = e.id, message = e.result.ToString() });
        }

        private void InteractiveClient_OnMethod(object sender, InteractiveMethodPacket e)
        {
            replies.Insert(0, new Reply() { id = e.id, message = e.method.ToString() });
        }

        private void InteractiveClient_OnError(object sender, InteractiveErrorModel e)
        {
            InteractiveErrorModel error = e;
        }

        private void InteractiveClient_OnIssueMemoryWarning(object sender, InteractiveIssueMemoryWarningModel e)
        {
        }

        private void InteractiveClient_OnReplyGetAllParticipants(object sender, InteractiveGetAllParticipants e)
        {
            InteractiveGetAllParticipants participants = e;
        }

        private void InteractiveClient_OnParticipantLeave(object sender, InteractiveParticipantChangedModel e)
        {
            List<uint> userIDs = e.participants.Select(x => x.userID).ToList();
            List<Participant> foundParticipants = participants.Where(x => userIDs.Contains(x.userID)).ToList();
            foreach (Participant p in foundParticipants)
            {
                participants.Remove(p);
            }
        }

        private void InteractiveClient_OnParticipantJoin(object sender, InteractiveParticipantChangedModel e)
        {
            foreach (InteractiveParticipantModel p in e.participants)
            {
                if (participants.SingleOrDefault(x => x.userID == p.userID) == null)
                {
                    participants.Add(new Participant()
                    {
                        userID = p.userID,
                        username = p.username,
                        connectedAt = p.connectedAt,
                        lastInputAt = p.lastInputAt,
                        level = p.level,
                    });
                }
            }
        }

        private void InteractiveClient_OnParticipantUpdate(object sender, InteractiveParticipantChangedModel e)
        {
            foreach (InteractiveParticipantModel p in e.participants)
            {
                Participant found = participants.SingleOrDefault(x => x.userID == p.userID);
                if (found != null)
                {
                    found.userID = p.userID;
                    found.username = p.username;
                    found.connectedAt = p.connectedAt;
                    found.lastInputAt = p.lastInputAt;
                    found.level = p.level;
                }
            }
        }

        private void InteractiveClient_OnReplyGetScenes(object sender, InteractiveGetScenes e)
        {
            InteractiveGetScenes scenes = e;
        }

        #endregion Interactive Event Handler



    }
}