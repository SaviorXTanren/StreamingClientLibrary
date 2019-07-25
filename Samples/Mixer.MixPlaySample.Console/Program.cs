using Mixer.Base;
using Mixer.Base.Clients;
using Mixer.Base.Model.Channel;
using Mixer.Base.Model.MixPlay;
using Mixer.Base.Model.User;
using Mixer.Base.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace Mixer.MixPlaySample.Console
{
    public class Program
    {
        private static MixPlayClient mixPlayClient;
        private static List<MixPlayConnectedSceneModel> scenes = new List<MixPlayConnectedSceneModel>();
        private static List<MixPlayConnectedButtonControlModel> buttons = new List<MixPlayConnectedButtonControlModel>();
        private static List<MixPlayConnectedJoystickControlModel> joysticks = new List<MixPlayConnectedJoystickControlModel>();
        private static List<MixPlayConnectedLabelControlModel> labels = new List<MixPlayConnectedLabelControlModel>();
        private static List<MixPlayConnectedTextBoxControlModel> textBoxes = new List<MixPlayConnectedTextBoxControlModel>();

        public static void Main(string[] args)
        {
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

            System.Console.WriteLine("Connecting to Mixer...");

            MixerConnection connection = MixerConnection.ConnectViaLocalhostOAuthBrowser(ConfigurationManager.AppSettings["ClientID"], scopes).Result;

            if (connection != null)
            {
                System.Console.WriteLine("Mixer connection successful!");

                UserModel user = connection.Users.GetCurrentUser().Result;
                ExpandedChannelModel channel = connection.Channels.GetChannel(user.username).Result;
                System.Console.WriteLine(string.Format("Logged in as: {0}", user.username));

                List<MixPlayGameListingModel> games = new List<MixPlayGameListingModel>(connection.MixPlay.GetOwnedMixPlayGames(channel).Result);
                MixPlayGameListingModel game = games.FirstOrDefault();
                if (game != null)
                {
                    System.Console.WriteLine();
                    System.Console.WriteLine(string.Format("Connecting to channel MixPlay using game {0}...", game.name));

                    Program.mixPlayClient = MixPlayClient.CreateFromChannel(connection, channel, game).Result;

                    if (Program.mixPlayClient.Connect().Result && Program.mixPlayClient.Ready().Result)
                    {
                        MixPlayConnectedSceneGroupCollectionModel scenes = Program.mixPlayClient.GetScenes().Result;
                        if (scenes != null)
                        {
                            Program.scenes.AddRange(scenes.scenes);

                            foreach (MixPlayConnectedSceneModel scene in Program.scenes)
                            {
                                foreach (MixPlayConnectedButtonControlModel button in scene.buttons)
                                {
                                    Program.buttons.Add(button);
                                }

                                foreach (MixPlayConnectedJoystickControlModel joystick in scene.joysticks)
                                {
                                    Program.joysticks.Add(joystick);
                                }

                                foreach (MixPlayConnectedLabelControlModel label in scene.labels)
                                {
                                    Program.labels.Add(label);
                                }

                                foreach (MixPlayConnectedTextBoxControlModel textBox in scene.textBoxes)
                                {
                                    Program.textBoxes.Add(textBox);
                                }

                                foreach (MixPlayControlModel control in scene.allControls)
                                {
                                    control.disabled = false;
                                }

                                Program.mixPlayClient.UpdateControls(scene, scene.allControls).Wait();
                            }

                            Program.mixPlayClient.OnDisconnectOccurred += MixPlayClient_OnDisconnectOccurred;
                            Program.mixPlayClient.OnParticipantJoin += MixPlayClient_OnParticipantJoin;
                            Program.mixPlayClient.OnParticipantLeave += MixPlayClient_OnParticipantLeave;
                            Program.mixPlayClient.OnGiveInput += MixPlayClient_OnGiveInput;

                            while (true) { }
                        }
                    }
                }
            }
        }

        private static async void MixPlayClient_OnDisconnectOccurred(object sender, System.Net.WebSockets.WebSocketCloseStatus e)
        {
            System.Console.WriteLine("Disconnection Occurred, attempting reconnection...");

            do
            {
                await Task.Delay(2500);
            }
            while (!await Program.mixPlayClient.Connect() && !await Program.mixPlayClient.Ready());

            System.Console.WriteLine("Reconnection successful");
        }

        private static void MixPlayClient_OnParticipantJoin(object sender, MixPlayParticipantCollectionModel e)
        {
            if (e.participants != null)
            {
                foreach (MixPlayParticipantModel participant in e.participants)
                {
                    System.Console.WriteLine("Participant Joined: " + participant.username);
                }
            }
        }

        private static void MixPlayClient_OnParticipantLeave(object sender, MixPlayParticipantCollectionModel e)
        {
            if (e.participants != null)
            {
                foreach (MixPlayParticipantModel participant in e.participants)
                {
                    System.Console.WriteLine("Participant Left: " + participant.username);
                }
            }
        }

        private static void MixPlayClient_OnGiveInput(object sender, MixPlayGiveInputModel e)
        {
            System.Console.WriteLine("Input Received: " + e.participantID + " - " + e.input.eventType + " - " + e.input.controlID);

            if (e.input.eventType.Equals("mousedown") && e.transactionID != null)
            {
                MixPlayConnectedButtonControlModel button = Program.buttons.FirstOrDefault(b => b.controlID.Equals(e.input.controlID));
                if (button != null)
                {
                    MixPlayConnectedSceneModel scene = Program.scenes.FirstOrDefault(s => s.buttons.Contains(button));
                    if (scene != null)
                    {
                        button.cooldown = DateTimeOffset.Now.AddSeconds(10).ToUnixTimeMilliseconds();

                        Program.mixPlayClient.UpdateControls(scene, new List<MixPlayConnectedButtonControlModel>() { button }).Wait();
                        System.Console.WriteLine("Sent 10 second cooldown to button: " + e.input.controlID);
                    }
                }

                Program.mixPlayClient.CaptureSparkTransaction(e.transactionID).Wait();
                System.Console.WriteLine("Spark Transaction Captured: " + e.participantID + " - " + e.input.eventType + " - " + e.input.controlID);
            }
        }
    }
}
