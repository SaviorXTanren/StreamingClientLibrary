namespace Twitch.Base.Services.V5API
{
    /// <summary>
    /// API wrapper for all Twitch API v5 services
    /// </summary>
    public class V5APIServices
    {
        /// <summary>
        /// APIs for Bits interaction.
        /// </summary>
        public BitsService Bits { get; private set; }

        /// <summary>
        /// APIs for Channel interaction.
        /// </summary>
        public ChannelsService Channels { get; private set; }

        /// <summary>
        /// APIs for Chat interaction.
        /// </summary>
        public ChatService Chat { get; private set; }

        /// <summary>
        /// APIs for Search interaction.
        /// </summary>
        public SearchService Search { get; private set; }

        /// <summary>
        /// APIs for Streams interaction.
        /// </summary>
        public StreamsService Streams { get; private set; }

        /// <summary>
        /// APIs for Teams interaction.
        /// </summary>
        public TeamsService Teams { get; private set; }

        /// <summary>
        /// APIs for User interaction.
        /// </summary>
        public UsersService Users { get; private set; }

        /// <summary>
        /// Creates a new instance of the V5APIServices class with the specified connection.
        /// </summary>
        /// <param name="connection">The Twitch connection</param>
        public V5APIServices(TwitchConnection connection)
        {
            this.Bits = new BitsService(connection);
            this.Channels = new ChannelsService(connection);
            this.Chat = new ChatService(connection);
            this.Search = new SearchService(connection);
            this.Streams = new StreamsService(connection);
            this.Teams = new TeamsService(connection);
            this.Users = new UsersService(connection);
        }
    }
}
