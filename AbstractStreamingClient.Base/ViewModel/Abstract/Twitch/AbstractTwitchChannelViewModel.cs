using StreamingClient.Base.ViewModel.Abstract;
using Twitch.Base.Models.V5.Channel;

namespace AbstractStreamingClient.Base.ViewModel.Abstract.Twitch
{
    public class AbstractTwitchChannelViewModel : AbstractChannelViewModel
    {
        /// <summary>
        /// The underlying Twitch channel.
        /// </summary>
        public ChannelModel Channel { get; private set; }

        /// <summary>
        /// Creates a new instance of the AbstractTwitchChannelViewModel class.
        /// </summary>
        /// <param name="channel">The underlying Twitch channel.</param>
        public AbstractTwitchChannelViewModel(ChannelModel channel)
        {
            this.Channel = channel;
        }

        /// <summary>
        /// The platform of the channel.
        /// </summary>
        public override StreamingPlatformType Platform { get { return StreamingPlatformType.Twitch; } }
        /// <summary>
        /// The ID of the channel.
        /// </summary>
        public override string ID { get { return this.Channel.id.ToString(); } }
        /// <summary>
        /// The name of the channel.
        /// </summary>
        public override string Name { get { return this.Channel.name.ToString(); } }
        /// <summary>
        /// The title of the channel.
        /// </summary>
        public override string Title { get { return this.Channel.status; } set { this.Channel.status = value; } }
        /// <summary>
        /// The URL of the channel.
        /// </summary>
        public override string URL { get { return this.Channel.url; } }
    }
}
