using Newtonsoft.Json.Linq;
using StreamingClient.Base.Util;
using StreamingClient.Base.Web;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Twitch.Base.Models.V5.Channel;
using Twitch.Base.Models.V5.Communities;
using Twitch.Base.Models.V5.Teams;
using Twitch.Base.Models.V5.Users;
using Twitch.Base.Models.V5.Videos;

namespace Twitch.Base.Services.V5API
{
    /// <summary>
    /// The APIs for User-based services.
    /// </summary>
    public class ChannelsService : V5APIServiceBase
    {
        /// <summary>
        /// Creates an instance of the ChannelsService.
        /// </summary>
        /// <param name="connection">The Twitch connection to use</param>
        public ChannelsService(TwitchConnection connection) : base(connection) { }

        /// <summary>
        /// Gets the channel for the currently authenticated account.
        /// </summary>
        /// <returns>The channel information</returns>
        public async Task<PrivateChannelModel> GetCurrentChannel() { return await this.GetAsync<PrivateChannelModel>("channel"); }

        /// <summary>
        /// Gets the channel for the specified channel.
        /// </summary>
        /// <param name="channel">The channel to search for</param>
        /// <returns>The channel information</returns>
        public async Task<ChannelModel> GetChannel(ChannelModel channel) { return await this.GetChannelByID(channel.id); }

        /// <summary>
        /// Gets the channel for the specified user.
        /// </summary>
        /// <param name="user">The user's channel to search for</param>
        /// <returns>The channel information</returns>
        public async Task<ChannelModel> GetChannel(UserModel user) { return await this.GetChannelByID(user.id); }

        /// <summary>
        /// Gets the channel for the specified ID.
        /// </summary>
        /// <param name="channelID">The channel ID to search for</param>
        /// <returns>The channel information</returns>
        public async Task<ChannelModel> GetChannelByID(string channelID)
        {
            Validator.ValidateString(channelID, "channelID");
            return await this.GetAsync<ChannelModel>("channels/" + channelID);
        }

        /// <summary>
        /// Updates the channel with the specified metadata.
        /// </summary>
        /// <param name="channel">The channel to update</param>
        /// <param name="update">The metadata to update</param>
        /// <returns>The updated channel</returns>
        public async Task<PrivateChannelModel> UpdateChannel(ChannelModel channel, ChannelUpdateModel update)
        {
            Validator.ValidateVariable(update, "update");
            return await this.PutAsync<PrivateChannelModel>("channels/" + channel.id, AdvancedHttpClient.CreateContentFromObject(new { channel = update }));
        }

        /// <summary>
        /// Gets the moderators for a channel.
        /// </summary>
        /// <param name="channel">The channel to get editors for</param>
        /// <returns>The moderators for the channel</returns>
        public async Task<IEnumerable<UserModel>> GetChannelEditors(ChannelModel channel)
        {
            Validator.ValidateVariable(channel, "channel");
            return await this.GetNamedArrayAsync<UserModel>("channels/" + channel.id + "/editors", "users");
        }

        /// <summary>
        /// Gets the followers for a channel.
        /// </summary>
        /// <param name="channel">The channel to get followers for</param>
        /// <param name="maxResults">The maximum number of results. Will be either that amount or slightly more</param>
        /// <returns>The followers for the channel</returns>
        public async Task<IEnumerable<UserFollowModel>> GetChannelFollowers(ChannelModel channel, int maxResults = 1)
        {
            Validator.ValidateVariable(channel, "channel");
            return await this.GetCursorPagedResultAsync<UserFollowModel>("channels/" + channel.id + "/follows", "follows", maxResults);
        }

        /// <summary>
        /// Gets the total number of followers for a channel.
        /// </summary>
        /// <param name="channel">The channel to get the total number of followers for</param>
        /// <returns>The total number of followers for the channel</returns>
        public async Task<long> GetChannelFollowersCount(ChannelModel channel)
        {
            Validator.ValidateVariable(channel, "channel");
            return await this.GetPagedResultTotalCountAsync("channels/" + channel.id + "/follows");
        }

        /// <summary>
        /// Gets the teams for a channel.
        /// </summary>
        /// <param name="channel">The channel to get teams for</param>
        /// <returns>The teams for the channel</returns>
        public async Task<IEnumerable<TeamModel>> GetChannelTeams(ChannelModel channel)
        {
            Validator.ValidateVariable(channel, "channel");
            return await this.GetNamedArrayAsync<TeamModel>("channels/" + channel.id + "/teams", "teams");
        }

        /// <summary>
        /// Gets the subscribers for a channel.
        /// </summary>
        /// <param name="channel">The channel to get subscribers for</param>
        /// <param name="maxResults">The maximum number of results. Will be either that amount or slightly more</param>
        /// <returns>The subscribers for the channel</returns>
        public async Task<IEnumerable<UserSubscriptionModel>> GetChannelSubscribers(ChannelModel channel, int maxResults = 1)
        {
            Validator.ValidateVariable(channel, "channel");
            return await this.GetOffsetPagedResultAsync<UserSubscriptionModel>("channels/" + channel.id + "/subscriptions", "subscriptions", maxResults);
        }

        /// <summary>
        /// Gets the total number of subscribers for a channel.
        /// </summary>
        /// <param name="channel">The channel to get the total number of subscribers for</param>
        /// <returns>The total number of subscribers for the channel</returns>
        public async Task<long> GetChannelSubscribersCount(ChannelModel channel)
        {
            Validator.ValidateVariable(channel, "channel");
            return await this.GetPagedResultTotalCountAsync("channels/" + channel.id + "/subscriptions");
        }

        /// <summary>
        /// Gets the subscription for a user to a specified channel if it exists.
        /// </summary>
        /// <param name="channel">The channel to get the subscription for</param>
        /// <param name="user">The user to check subscription for</param>
        /// <returns>The subscription for the channel</returns>
        public async Task<UserSubscriptionModel> GetChannelUserSubscription(ChannelModel channel, UserModel user)
        {
            Validator.ValidateVariable(channel, "channel");
            Validator.ValidateVariable(user, "user");
            return await this.GetAsync<UserSubscriptionModel>("channels/" + channel.id + "/subscriptions/" + user.id, throwExceptionOnFailure: false);
        }

        /// <summary>
        /// Gets the videos for a channel.
        /// </summary>
        /// <param name="channel">The channel to get videos for</param>
        /// <param name="maxResults">The maximum number of results. Will be either that amount or slightly more</param>
        /// <returns>The videos for the channel</returns>
        public async Task<IEnumerable<VideoModel>> GetChannelVideos(ChannelModel channel, int maxResults = 1)
        {
            Validator.ValidateVariable(channel, "channel");
            return await this.GetOffsetPagedResultAsync<VideoModel>("channels/" + channel.id + "/videos", "videos", maxResults);
        }

        /// <summary>
        /// Shows a commercial on the channel.
        /// </summary>
        /// <param name="channel">The channel to show a commercial for</param>
        /// <param name="length">The length of the commercial</param>
        /// <returns>Information about the commercial</returns>
        public async Task<ChannelCommercialModel> ShowChannelCommercial(ChannelModel channel, int length)
        {
            Validator.ValidateVariable(channel, "channel");
            return await this.PostAsync<ChannelCommercialModel>("channels/" + channel.id + "/commercial", AdvancedHttpClient.CreateContentFromObject(new { length = length }));
        }

        /// <summary>
        /// Resets the stream key on the channel.
        /// </summary>
        /// <param name="channel">The channel to reset the stream key for</param>
        /// <returns>The updated channel</returns>
        public async Task<PrivateChannelModel> ResetChannelStreamKey(ChannelModel channel)
        {
            Validator.ValidateVariable(channel, "channel");
            return await this.DeleteAsync<PrivateChannelModel>("channels/" + channel.id + "/stream_key");
        }

        /// <summary>
        /// Gets the communities for a channel.
        /// </summary>
        /// <param name="channel">The channel to get communities for</param>
        /// <returns>The communities for the channel</returns>
        public async Task<IEnumerable<CommunityModel>> GetChannelCommunities(ChannelModel channel)
        {
            Validator.ValidateVariable(channel, "channel");
            return await this.GetNamedArrayAsync<CommunityModel>("channels/" + channel.id + "/communities", "communities");
        }

        /// <summary>
        /// Sets the communities for a channel.
        /// </summary>
        /// <param name="channel">The channel to set communities for</param>
        /// <param name="communities">The communities to set</param>
        /// <returns>An awaitable Task</returns>
        public async Task SetChannelCommunities(ChannelModel channel, IEnumerable<CommunityModel> communities)
        {
            Validator.ValidateVariable(channel, "channel");
            JObject content = new JObject();
            content["community_ids"] = new JArray(communities.Select(c => c.id).ToList());
            await this.PutAsync("channels/" + channel.id + "/communities", AdvancedHttpClient.CreateContentFromObject(content));
        }

        /// <summary>
        /// Deletes the communities for a channel.
        /// </summary>
        /// <param name="channel">The channel to delete communities for</param>
        /// <returns>An awaitable Task</returns>
        public async Task DeleteChannelCommunities(ChannelModel channel)
        {
            Validator.ValidateVariable(channel, "channel");
            await this.DeleteAsync("channels/" + channel.id + "/community");
        }
    }
}
