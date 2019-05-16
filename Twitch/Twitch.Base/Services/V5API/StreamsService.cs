using StreamingClient.Base.Util;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Twitch.Base.Models.V5.Channel;
using Twitch.Base.Models.V5.Streams;

namespace Twitch.Base.Services.V5API
{
    /// <summary>
    /// The type of stream that is playing.
    /// </summary>
    public enum StreamType
    {
        /// <summary>
        /// A live stream.
        /// </summary>
        Live,
        /// <summary>
        /// A playlist of videos.
        /// </summary>
        Playlist,
        /// <summary>
        /// All stream types
        /// </summary>
        All
    }

    /// <summary>
    /// The APIs for Streams-based services.
    /// </summary>
    public class StreamsService : V5APIServiceBase
    {
        /// <summary>
        /// Creates an instance of the StreamsService.
        /// </summary>
        /// <param name="connection">The Twitch connection to use</param>
        public StreamsService(TwitchConnection connection) : base(connection) { }

        /// <summary>
        /// Gets the stream information for the specified channel.
        /// </summary>
        /// <param name="channel">The channel to get stream information for</param>
        /// <param name="streamType">The type of stream to search for</param>
        /// <returns>The stream information</returns>
        public async Task<StreamModel> GetChannelStream(ChannelModel channel, StreamType streamType = StreamType.Live)
        {
            Validator.ValidateVariable(channel, "channel");
            return await this.GetAsync<StreamModel>("streams/" + channel.id + "?stream_type=" + streamType.ToString().ToLower());
        }

        /// <summary>
        /// Gets a set of stream.
        /// </summary>
        /// <param name="channelIDs">The channels to get streams for</param>
        /// <param name="game">The name of the game to get streams for</param>
        /// <param name="language">The language to get streams for</param>
        /// <param name="streamType">The type of stream to search for</param>
        /// <param name="maxResults">The maximum number of results. Will be either that amount or slightly more</param>
        /// <returns>The followers for the channel</returns>
        public async Task<IEnumerable<StreamModel>> GetStreams(IEnumerable<string> channelIDs = null, string game = null, string language = null, StreamType streamType = StreamType.Live, int maxResults = 1)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            if (channelIDs != null)
            {
                parameters.Add("channel", string.Join(",", channelIDs));
            }
            if (game != null)
            {
                parameters.Add("game", game);
            }
            if (language != null)
            {
                parameters.Add("language", language);
            }
            if (channelIDs != null)
            {
                parameters.Add("stream_type", streamType.ToString().ToLower());
            }

            string parameterString = string.Join("&", parameters.Select(kvp => kvp.Key + "=" + kvp.Value));
            return await this.GetOffsetPagedResultAsync<StreamModel>("streams?" + parameterString, "streams", maxResults);
        }

        /// <summary>
        /// Gets a summary of the streams for a specified game.
        /// </summary>
        /// <param name="game">The game to get a streams summary for</param>
        /// <returns>The streams summary</returns>
        public async Task<StreamsSummaryModel> GetStreamsSummary(string game)
        {
            Validator.ValidateVariable(game, "game");
            return await this.GetAsync<StreamsSummaryModel>("streams/summary?game=" + game);
        }

        /// <summary>
        /// Gets the featured streams
        /// </summary>
        /// <param name="maxResults">The maximum number of results. Will be either that amount or slightly more</param>
        /// <returns>The streams summary</returns>
        public async Task<IEnumerable<FeaturedStreamModel>> GetFeaturedStreams(int maxResults = 1)
        {
            return await this.GetNamedArrayAsync<FeaturedStreamModel>("streams/featured?limit" + maxResults, "featured");
        }

        /// <summary>
        /// Gets a list of streams that the current user is following.
        /// </summary>
        /// <param name="streamType">The type of stream to search for</param>
        /// <param name="maxResults">The maximum number of results. Will be either that amount or slightly more</param>
        /// <returns>The followers for the channel</returns>
        public async Task<IEnumerable<StreamModel>> GetFollowedStreams(StreamType streamType = StreamType.Live, int maxResults = 1)
        {
            return await this.GetOffsetPagedResultAsync<StreamModel>("streams/followed?stream_type=" + streamType.ToString().ToLower(), "streams", maxResults);
        }
    }
}
