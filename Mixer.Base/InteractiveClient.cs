using Mixer.Base.Model.Channel;
using Mixer.Base.Model.Interactive;
using Mixer.Base.Util;
using Mixer.Base.Web;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mixer.Base
{
    public class InteractiveClient : WebSocketClientBase
    {
        public ChannelModel Channel { get; private set; }

        private IEnumerable<string> interactiveConnections;

        public static async Task<InteractiveClient> CreateFromChannel(MixerClient client, ChannelModel channel)
        {
            Validator.ValidateVariable(client, "client");
            Validator.ValidateVariable(channel, "channel");

            IEnumerable<string> interactiveConnections = await client.Interactive.GetInteractiveHosts();

            return new InteractiveClient(channel, interactiveConnections);
        }

        private InteractiveClient(ChannelModel channel, IEnumerable<string> interactiveConnections)
        {
            Validator.ValidateVariable(channel, "channel");
            Validator.ValidateList(interactiveConnections, "interactiveConnections");

            this.Channel = channel;
            this.interactiveConnections = interactiveConnections;
        }

        public async Task<bool> Ready()
        {
            int totalEndpoints = this.interactiveConnections.Count();
            Random random = new Random();
            int endpointToUse = random.Next() % totalEndpoints;

            await this.ConnectInternal(this.interactiveConnections.ElementAt(endpointToUse));

            return this.connectSuccessful;
        }

        protected override void Receive(string jsonBuffer)
        {
            InteractivePacket packet = JsonConvert.DeserializeObject<InteractivePacket>(jsonBuffer);
            if (packet.type.Equals("reply"))
            {

            }
            else if (packet.type.Equals("method"))
            {

            }
        }
    }
}
