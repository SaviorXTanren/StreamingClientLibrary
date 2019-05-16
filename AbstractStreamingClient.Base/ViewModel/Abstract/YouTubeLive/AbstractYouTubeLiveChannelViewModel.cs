using Google.Apis.YouTube.v3.Data;
using StreamingClient.Base.ViewModel.Abstract;

namespace AbstractStreamingClient.Base.ViewModel.Abstract.YouTubeLive
{
    public class AbstractYouTubeLiveChannelViewModel : AbstractChannelViewModel
    {
        /// <summary>
        /// The underlying YouTube Live channel.
        /// </summary>
        public Channel Channel { get; private set; }
        /// <summary>
        /// The underlying YouTube Live broadcast.
        /// </summary>
        public LiveBroadcast Broadcast { get; private set; }

        /// <summary>
        /// Creates a new instance of the AbstractYouTubeLiveChannelViewModel class.
        /// </summary>
        /// <param name="channel">The underlying YouTube Live channel.</param>
        public AbstractYouTubeLiveChannelViewModel(Channel channel, LiveBroadcast broadcast)
        {
            this.Channel = channel;
            this.Broadcast = broadcast;
        }

        /// <summary>
        /// The platform of the channel.
        /// </summary>
        public override StreamingPlatformType Platform { get { return StreamingPlatformType.YouTubeLive; } }
        /// <summary>
        /// The ID of the channel.
        /// </summary>
        public override string ID { get { return this.Channel.Id.ToString(); } }
        /// <summary>
        /// The name of the channel.
        /// </summary>
        public override string Name { get { return this.Channel.Snippet.Title.ToString(); } }
        /// <summary>
        /// The title of the channel.
        /// </summary>
        public override string Title
        {
            get { return (this.Broadcast != null) ? this.Broadcast.Snippet.Title : null; }
            set
            {
                if (this.Broadcast != null)
                {
                    this.Broadcast.Snippet.Title = value;
                }
            }
        }
        /// <summary>
        /// The URL of the channel.
        /// </summary>
        public override string URL { get { return "https://www.youtube.com/channel/" + this.ID; } }
    }
}
