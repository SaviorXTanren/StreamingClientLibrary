using Mixer.Base.Model.Channel;
using Mixer.Base.Util;
using Mixer.Base.Web;
using System;
using System.Threading.Tasks;

namespace Mixer.Base
{
    public class InteractiveClient : WebSocketClientBase
    {
        public ChannelModel Channel { get; private set; }

        public static InteractiveClient CreateFromChannel(MixerClient client, ChannelModel channel)
        {
            Validator.ValidateVariable(channel, "channel");

            return new InteractiveClient(channel);
        }

        private InteractiveClient(ChannelModel channel)
        {
            Validator.ValidateVariable(channel, "channel");

            this.Channel = channel;
        }

        public async Task<bool> Ready()
        {
            return this.connectSuccessful;
        }

        protected override void Receive(string jsonBuffer)
        {
            throw new NotImplementedException();
        }
    }
}
