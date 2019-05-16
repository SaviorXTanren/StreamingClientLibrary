using Mixer.Base.Model.Channel;
using StreamingClient.Base.ViewModel.Abstract;

namespace AbstractStreamingClient.Base.ViewModel.Abstract.Mixer
{
    public class AbstractMixerChannelViewModel : AbstractChannelViewModel
    {
        /// <summary>
        /// The underlying Mixer channel.
        /// </summary>
        public ChannelModel Channel { get; private set; }

        /// <summary>
        /// Creates a new instance of the AbstractMixerChannelViewModel class.
        /// </summary>
        /// <param name="channel">The underlying Mixer channel.</param>
        public AbstractMixerChannelViewModel(ChannelModel channel)
        {
            this.Channel = channel;
        }

        /// <summary>
        /// The platform of the channel.
        /// </summary>
        public override StreamingPlatformType Platform { get { return StreamingPlatformType.Mixer; } }
        /// <summary>
        /// The ID of the channel.
        /// </summary>
        public override string ID { get { return this.Channel.id.ToString(); } }
        /// <summary>
        /// The name of the channel.
        /// </summary>
        public override string Name { get { return this.Channel.token.ToString(); } }
        /// <summary>
        /// The title of the channel.
        /// </summary>
        public override string Title { get { return this.Channel.name; } set { this.Channel.name = value; } }
        /// <summary>
        /// The URL of the channel.
        /// </summary>
        public override string URL { get { return "https://www.mixer.com/" + this.Channel.token; } }
    }
}
