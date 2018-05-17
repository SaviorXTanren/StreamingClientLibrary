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

namespace Mixer.InteractiveSample.Console
{
    public class Program
    {
        private static InteractiveClient interactiveClient;
        private static List<InteractiveConnectedSceneModel> scenes = new List<InteractiveConnectedSceneModel>();
        private static List<InteractiveConnectedButtonControlModel> buttons = new List<InteractiveConnectedButtonControlModel>();
        private static List<InteractiveConnectedJoystickControlModel> joysticks = new List<InteractiveConnectedJoystickControlModel>();
        private static List<InteractiveConnectedLabelControlModel> labels = new List<InteractiveConnectedLabelControlModel>();
        private static List<InteractiveConnectedTextBoxControlModel> textBoxes = new List<InteractiveConnectedTextBoxControlModel>();

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

                List<InteractiveGameListingModel> games = new List<InteractiveGameListingModel>(connection.Interactive.GetOwnedInteractiveGames(channel).Result);
                InteractiveGameListingModel game = games.FirstOrDefault();
                if (game != null)
                {
                    System.Console.WriteLine();
                    System.Console.WriteLine(string.Format("Connecting to channel interactive using game {0}...", game.name));

                    Program.interactiveClient = InteractiveClient.CreateFromChannel(connection, channel, game).Result;

                    if (Program.interactiveClient.Connect().Result && Program.interactiveClient.Ready().Result)
                    {
                        InteractiveConnectedSceneGroupCollectionModel scenes = Program.interactiveClient.GetScenes().Result;
                        if (scenes != null)
                        {
                            Program.scenes.AddRange(scenes.scenes);

                            foreach (InteractiveConnectedSceneModel scene in Program.scenes)
                            {
                                foreach (InteractiveConnectedButtonControlModel button in scene.buttons)
                                {
                                    Program.buttons.Add(button);
                                }

                                foreach (InteractiveConnectedJoystickControlModel joystick in scene.joysticks)
                                {
                                    Program.joysticks.Add(joystick);
                                }

                                foreach (InteractiveConnectedLabelControlModel label in scene.labels)
                                {
                                    Program.labels.Add(label);
                                }

                                foreach (InteractiveConnectedTextBoxControlModel textBox in scene.textBoxes)
                                {
                                    Program.textBoxes.Add(textBox);
                                }

                                foreach (InteractiveControlModel control in scene.allControls)
                                {
                                    control.disabled = false;
                                }

                                Program.interactiveClient.UpdateControls(scene, scene.allControls).Wait();
                            }

                            Program.interactiveClient.OnDisconnectOccurred += InteractiveClient_OnDisconnectOccurred;
                            Program.interactiveClient.OnParticipantJoin += InteractiveClient_OnParticipantJoin;
                            Program.interactiveClient.OnParticipantLeave += InteractiveClient_OnParticipantLeave;
                            Program.interactiveClient.OnGiveInput += InteractiveClient_OnGiveInput;

                            while (true) { }
                        }
                    }
                }
            }
        }

        private static async void InteractiveClient_OnDisconnectOccurred(object sender, System.Net.WebSockets.WebSocketCloseStatus e)
        {
            System.Console.WriteLine("Disconnection Occurred, attempting reconnection...");

            do
            {
                await InteractiveClient.Reconnect(Program.interactiveClient);
            } while (!await Program.interactiveClient.Ready());

            System.Console.WriteLine("Reconnection successful");
        }

        private static void InteractiveClient_OnParticipantJoin(object sender, InteractiveParticipantCollectionModel e)
        {
            if (e.participants != null)
            {
                foreach (InteractiveParticipantModel participant in e.participants)
                {
                    System.Console.WriteLine("Participant Joined: " + participant.username);
                }
            }
        }

        private static void InteractiveClient_OnParticipantLeave(object sender, InteractiveParticipantCollectionModel e)
        {
            if (e.participants != null)
            {
                foreach (InteractiveParticipantModel participant in e.participants)
                {
                    System.Console.WriteLine("Participant Left: " + participant.username);
                }
            }
        }

        private static void InteractiveClient_OnGiveInput(object sender, InteractiveGiveInputModel e)
        {
            System.Console.WriteLine("Input Received: " + e.participantID + " - " + e.input.eventType + " - " + e.input.controlID);

            if (e.input.eventType.Equals("mousedown") && e.transactionID != null)
            {
                InteractiveConnectedButtonControlModel button = Program.buttons.FirstOrDefault(b => b.controlID.Equals(e.input.controlID));
                if (button != null)
                {
                    InteractiveConnectedSceneModel scene = Program.scenes.FirstOrDefault(s => s.buttons.Contains(button));
                    if (scene != null)
                    {
                        button.cooldown = DateTimeHelper.DateTimeOffsetToUnixTimestamp(DateTimeOffset.Now.AddSeconds(10));

                        Program.interactiveClient.UpdateControls(scene, new List<InteractiveConnectedButtonControlModel>() { button }).Wait();
                        System.Console.WriteLine("Sent 10 second cooldown to button: " + e.input.controlID);
                    }
                }

                Program.interactiveClient.CaptureSparkTransaction(e.transactionID).Wait();
                System.Console.WriteLine("Spark Transaction Captured: " + e.participantID + " - " + e.input.eventType + " - " + e.input.controlID);
            }
        }
    }
}
