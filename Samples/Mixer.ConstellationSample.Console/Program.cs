using Mixer.Base;
using Mixer.Base.Clients;
using Mixer.Base.Model.Channel;
using Mixer.Base.Model.User;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mixer.ConstellationSample.Console
{
    public class Program
    {
        private static ConstellationClient constellationClient;

        public static void Main(string[] args)
        {
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

            System.Console.WriteLine("Connecting to Mixer...");

            MixerConnection connection = MixerConnection.ConnectViaLocalhostOAuthBrowser(ConfigurationManager.AppSettings["ClientID"], scopes).Result;

            if (connection != null)
            {
                System.Console.WriteLine("Mixer connection successful!");

                UserModel user = connection.Users.GetCurrentUser().Result;
                ExpandedChannelModel channel = connection.Channels.GetChannel(user.username).Result;
                System.Console.WriteLine(string.Format("Logged in as: {0}", user.username));

                System.Console.WriteLine();
                System.Console.WriteLine("Connecting to constellation...");

                constellationClient = ConstellationClient.Create(connection).Result;

                constellationClient.OnDisconnectOccurred += ConstellationClient_OnDisconnectOccurred;
                constellationClient.OnSubscribedEventOccurred += ConstellationClient_OnSubscribedEventOccurred;

                if (constellationClient.Connect().Result)
                {
                    System.Console.WriteLine("Constellation connection successful!");

                    List<ConstellationEventTypeEnum> eventsToSubscribeTo = new List<ConstellationEventTypeEnum>()
                    {
                        ConstellationEventTypeEnum.channel__id__followed, ConstellationEventTypeEnum.channel__id__hosted, ConstellationEventTypeEnum.channel__id__subscribed,
                        ConstellationEventTypeEnum.channel__id__resubscribed, ConstellationEventTypeEnum.channel__id__resubShared, ConstellationEventTypeEnum.channel__id__subscriptionGifted,
                        ConstellationEventTypeEnum.channel__id__update, ConstellationEventTypeEnum.channel__id__skill, ConstellationEventTypeEnum.channel__id__patronageUpdate,
                        ConstellationEventTypeEnum.progression__id__levelup,
                    };

                    List<ConstellationEventType> events = eventsToSubscribeTo.Select(e => new ConstellationEventType(e, channel.id)).ToList();

                    constellationClient.SubscribeToEvents(events).Wait();

                    System.Console.WriteLine("Subscribed to events successful!");
                    System.Console.WriteLine();

                    while (true) { }
                }
            }
        }

        private static async void ConstellationClient_OnDisconnectOccurred(object sender, System.Net.WebSockets.WebSocketCloseStatus e)
        {
            System.Console.WriteLine("Disconnection Occurred, attempting reconnection...");

            do
            {
                await Task.Delay(2500);
            }
            while (!await Program.constellationClient.Connect());

            System.Console.WriteLine("Reconnection successful");
            System.Console.WriteLine();
        }

        private static void ConstellationClient_OnSubscribedEventOccurred(object sender, Base.Model.Constellation.ConstellationLiveEventModel liveEvent)
        {
            System.Console.WriteLine(string.Format("Event Received: {0} - {1}", liveEvent.channel, liveEvent.payload));
            System.Console.WriteLine();
        }
    }
}
