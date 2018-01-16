using Mixer.Base.Model.Channel;
using Mixer.Base.Model.User;
using Mixer.Base.Util;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Mixer.Base.Services
{
    /// <summary>
    /// The APIs for Channel-based services.
    /// </summary>
    public class ChannelsService : ServiceBase
    {
        public ChannelsService(MixerConnection connection) : base(connection) { }

        /// <summary>
        /// Gets the channel with the specified channel name.
        /// </summary>
        /// <param name="channelName">The name of the channel (also the name of the user account)</param>
        /// <returns>An expanded channel</returns>
        public async Task<ExpandedChannelModel> GetChannel(string channelName)
        {
            Validator.ValidateString(channelName, "channelName");
            return await this.GetAsync<ExpandedChannelModel>("channels/" + channelName);
        }

        /// <summary>
        /// Gets a list of currently online channels. The search can be limited to a maximum number of results to speed up
        /// the operation as it can take a long time on large channels. This maximum number is a lower threshold and slightly
        /// more than the maximum number may be returned.
        /// </summary>
        /// <param name="maxResults">The maximum number of results. Will be either that amount or slightly more</param>
        /// <returns>A list of currently online channels</returns>
        public async Task<IEnumerable<ExpandedChannelModel>> GetChannels(uint maxResults = 1)
        {
            return await this.GetPagedAsync<ExpandedChannelModel>("channels", maxResults, linkPagesAvailable: false);
        }

        /// <summary>
        /// Updates the channel with the information supplied. Only limited information can be updated
        /// via this API. See the ChannelUpdateableModel for the fields that are updateable.
        /// </summary>
        /// <param name="channel">The channel to update</param>
        /// <returns>The updated channel</returns>
        public async Task<ChannelModel> UpdateChannel(ChannelModel channel)
        {
            Validator.ValidateVariable(channel, "channel");

            // Need to strip out all of the non-updateable fields in order for the API to not return a 403 error
            ChannelUpdateableModel updateableChannel = JsonHelper.ConvertToDifferentType<ChannelUpdateableModel>(channel);

            return await this.PatchAsync<ChannelModel>("channels/" + channel.id, this.CreateContentFromObject(updateableChannel));
        }

        /// <summary>
        /// Gets all the viewers from the specified channel from the specified start and end dates
        /// </summary>
        /// <param name="channel">The channel to get viewers for</param>
        /// <param name="startDate">The start date for viewers</param>
        /// <param name="endDate">The end date for viewers</param>
        /// <returns>All viewers during the timespan</returns>
        public async Task<IEnumerable<ViewerAnalyticModel>> GetViewerAnalytics(ChannelModel channel, DateTimeOffset startDate, DateTimeOffset? endDate = null)
        {
            Validator.ValidateVariable(channel, "channel");
            Validator.ValidateVariable(startDate, "startDate");

            return await this.GetPagedAsync<ViewerAnalyticModel>("channels/" + channel.id + "/analytics/tsdb/viewersMetrics?" + this.ConstructToAndFromQueryString(startDate, endDate));
        }

        /// <summary>
        /// Gets all the stream sessions from the specified channel from the specified start and end dates
        /// </summary>
        /// <param name="channel">The channel to get stream sessions for</param>
        /// <param name="startDate">The start date for stream sessions</param>
        /// <param name="endDate">The end date for stream sessions</param>
        /// <returns>All stream sessions during the timespan</returns>
        public async Task<IEnumerable<StreamSessionsAnalyticModel>> GetStreamSessions(ChannelModel channel, DateTimeOffset startDate, DateTimeOffset? endDate = null)
        {
            Validator.ValidateVariable(channel, "channel");
            Validator.ValidateVariable(startDate, "startDate");

            return await this.GetPagedAsync<StreamSessionsAnalyticModel>("channels/" + channel.id + "/analytics/tsdb/streamSessions?" + this.ConstructToAndFromQueryString(startDate, endDate));
        }

        /// <summary>
        /// Gets all the followers from the specified channel from the specified start and end dates
        /// </summary>
        /// <param name="channel">The channel to get followers for</param>
        /// <param name="startDate">The start date for followers</param>
        /// <param name="endDate">The end date for followers</param>
        /// <returns>All followers during the timespan</returns>
        public async Task<IEnumerable<FollowersAnalyticModel>> GetFollowerAnalytics(ChannelModel channel, DateTimeOffset startDate, DateTimeOffset? endDate = null)
        {
            Validator.ValidateVariable(channel, "channel");
            Validator.ValidateVariable(startDate, "startDate");

            return await this.GetPagedAsync<FollowersAnalyticModel>("channels/" + channel.id + "/analytics/tsdb/followers?" + this.ConstructToAndFromQueryString(startDate, endDate));
        }

        /// <summary>
        /// Gets all the subscribers from the specified channel from the specified start and end dates
        /// </summary>
        /// <param name="channel">The channel to get subscribers for</param>
        /// <param name="startDate">The start date for subscribers</param>
        /// <param name="endDate">The end date for subscribers</param>
        /// <returns>All subscribers during the timespan</returns>
        public async Task<IEnumerable<SubscriberAnalyticModel>> GetSubscriberAnalytics(ChannelModel channel, DateTimeOffset startDate, DateTimeOffset? endDate = null)
        {
            Validator.ValidateVariable(channel, "channel");
            Validator.ValidateVariable(startDate, "stateDate");

            return await this.GetPagedAsync<SubscriberAnalyticModel>("channels/" + channel.id + "/analytics/tsdb/subscriptions?" + this.ConstructToAndFromQueryString(startDate, endDate));
        }

        /// <summary>
        /// Gets the current followers of the channel. The search can be limited to a maximum number of results to speed up
        /// the operation as it can take a long time on large channels. This maximum number is a lower threshold and slightly
        /// more than the maximum number may be returned.
        /// </summary>
        /// <param name="channel">The channel to get followers for</param>
        /// <param name="maxResults">The maximum number of results. Will be either that amount or slightly more</param>
        /// <returns>The users who followed the channel</returns>
        public async Task<IEnumerable<FollowerUserModel>> GetFollowers(ChannelModel channel, uint maxResults = 1)
        {
            Validator.ValidateVariable(channel, "channel");

            return await this.GetPagedAsync<FollowerUserModel>("channels/" + channel.id + "/follow", maxResults);
        }

        /// <summary>
        /// Checks if the specified user follows the specified channel by getting their follow date.
        /// </summary>
        /// <param name="channel">The channel to get follows against</param>
        /// <param name="user">The user to check if they follow</param>
        /// <returns>The follow date of the user if they follow the channel</returns>
        public async Task<DateTimeOffset?> CheckIfFollows(ChannelModel channel, UserModel user)
        {
            Validator.ValidateVariable(channel, "channel");
            Validator.ValidateVariable(user, "user");

            Dictionary<UserModel, DateTimeOffset?> results = await this.CheckIfFollows(channel, new List<UserModel>() { user });
            return results[user];
        }

        /// <summary>
        /// Checks if the specified users follows the specified channel
        /// </summary>
        /// <param name="channel">The channel to get follows against</param>
        /// <param name="users">The users to check if they follow</param>
        /// <returns>All users checked and whether they follow or not</returns>
        public async Task<Dictionary<UserModel, DateTimeOffset?>> CheckIfFollows(ChannelModel channel, IEnumerable<UserModel> users)
        {
            Validator.ValidateVariable(channel, "channel");
            Validator.ValidateList(users, "users");

            Dictionary<UserModel, DateTimeOffset?> results = new Dictionary<UserModel, DateTimeOffset?>();
            for (int i = 0; i < users.Count(); i += 25)
            {
                IEnumerable<UserModel> userSubset = users.Skip(i).Take(25);

                string followQuery = "";
                foreach (UserModel user in userSubset)
                {
                    followQuery += "where=id:eq:" + user.id + ",";
                }
                followQuery = followQuery.Remove(followQuery.Length - 1);

                IEnumerable<FollowerUserModel> followUsers = await this.GetPagedAsync<FollowerUserModel>("channels/" + channel.id + "/follow?fields=id,followed&" + followQuery);
                IEnumerable<uint> followUserIDs = followUsers.Select(u => u.id);
                foreach (UserModel user in userSubset)
                {
                    DateTimeOffset? followDate = null;
                    FollowerUserModel follow = followUsers.FirstOrDefault(u => u.id.Equals(user.id));
                    if (follow != null)
                    {
                        followDate = follow.followed.createdAt;
                    }
                    results.Add(user, followDate);
                }
            }
            return results;
        }

        /// <summary>
        /// Gets the channel that the specified channel is hosting, if any.
        /// </summary>
        /// <param name="channel">The channel that is hosting</param>
        /// <returns>The hosted channel, if any</returns>
        public async Task<ChannelAdvancedModel> GetHostedChannel(ChannelModel channel)
        {
            Validator.ValidateVariable(channel, "channel");
            return await this.GetAsync<ChannelAdvancedModel>("channels/" + channel.id + "/hostee");
        }

        /// <summary>
        /// Sets the channel to host for the specified channel.
        /// </summary>
        /// <param name="channel">The channel hosting</param>
        /// <param name="hosteeChannel">The channelt to be hosted</param>
        /// <returns>The updated hosting channel</returns>
        public async Task<ChannelModel> SetHostChannel(ChannelModel channel, ChannelModel hosteeChannel)
        {
            Validator.ValidateVariable(channel, "channel");
            Validator.ValidateVariable(hosteeChannel, "hosteeChannel");
            return await this.PutAsync<ChannelModel>("channels/" + channel.id + "/hostee", this.CreateContentFromObject(hosteeChannel));
        }

        /// <summary>
        /// Unhosts whatever channel the specified channel may be hosting, if any.
        /// </summary>
        /// <param name="channel">The channel that is hosting</param>
        /// <returns>Whether the operation succeeded</returns>
        public async Task<bool> UnhostChannel(ChannelModel channel)
        {
            Validator.ValidateVariable(channel, "channel");
            return await this.DeleteAsync("channels/" + channel.id + "/hostee");
        }

        /// <summary>
        /// Unhosts whatever channel the specified channel may be hosting, if any. The search can be limited to a maximum number
        /// of results to speed up the operation as it can take a long time on large channels. This maximum number is a lower
        /// threshold and slightly more than the maximum number may be returned.
        /// </summary>
        /// <param name="channel">The channel to get hosters for</param>
        /// <param name="maxResults">The maximum number of results. Will be either that amount or slightly more</param>
        /// <returns>The hosting channels</returns>
        public async Task<IEnumerable<ChannelAdvancedModel>> GetHosters(ChannelModel channel, uint maxResults = 1)
        {
            Validator.ValidateVariable(channel, "channel");
            return await this.GetPagedAsync<ChannelAdvancedModel>("channels/" + channel.id + "/hosters", maxResults);
        }

        /// <summary>
        /// Gets the preferences for the specified channel.
        /// </summary>
        /// <param name="channel">The channel to get preferences for</param>
        /// <returns>The preferences for the channel</returns>
        public async Task<ChannelPreferencesModel> GetPreferences(ChannelModel channel)
        {
            Validator.ValidateVariable(channel, "channel");
            return await this.GetAsync<ChannelPreferencesModel>("channels/" + channel.id + "/preferences");
        }

        /// <summary>
        /// Updates the preferences for the specified channel.
        /// </summary>
        /// <param name="channel">The channel to update preferences for</param>
        /// <param name="preferences">The updated preferences for the channel</param>
        /// <returns></returns>
        public async Task<ChannelPreferencesModel> UpdatePreferences(ChannelModel channel, ChannelPreferencesModel preferences)
        {
            Validator.ValidateVariable(channel, "channel");
            Validator.ValidateVariable(preferences, "preferences");
            return await this.PostAsync<ChannelPreferencesModel>("channels/" + channel.id + "/preferences", this.CreateContentFromObject(preferences));
        }

        /// <summary>
        /// Gets all of the users who have roles for the specified channel. The search can be limited to a maximum number
        /// of results to speed up the operation as it can take a long time on large channels. This maximum number is a lower
        /// threshold and slightly more than the maximum number may be returned.
        /// </summary>
        /// <param name="channel">The channel to get users for</param>
        /// <param name="maxResults">The maximum number of results. Will be either that amount or slightly more</param>
        /// <returns>The users with roles for the channel</returns>
        public async Task<IEnumerable<UserWithGroupsModel>> GetUsersWithRoles(ChannelModel channel, uint maxResults = 1)
        {
            Validator.ValidateVariable(channel, "channel");
            return await this.GetPagedAsync<UserWithGroupsModel>("channels/" + channel.id + "/users", maxResults);
        }

        /// <summary>
        /// Gets the specified user and any roles they may have for the specified channel.
        /// </summary>
        /// <param name="channel">The channel to get users for</param>
        /// <param name="userID">The ID of the user to search for</param>
        /// <returns>The user with roles for the channel</returns>
        public async Task<UserWithGroupsModel> GetUser(ChannelModel channel, uint userID)
        {
            Validator.ValidateVariable(channel, "channel");
            IEnumerable<UserWithGroupsModel> users = await this.GetPagedAsync<UserWithGroupsModel>("channels/" + channel.id + "/users?where=id:eq:" + userID);
            return users.FirstOrDefault();
        }

        /// <summary>
        /// Gets all of the users who have the specified role for the specified channel. The search can be limited to a maximum number
        /// of results to speed up the operation as it can take a long time on large channels. This maximum number is a lower
        /// threshold and slightly more than the maximum number may be returned.
        /// </summary>
        /// <param name="channel">The channel to get users for</param>
        /// <param name="role">The role to search for</param>
        /// <param name="maxResults">The maximum number of results. Will be either that amount or slightly more</param>
        /// <returns>The users with roles for the channel</returns>
        public async Task<IEnumerable<UserWithGroupsModel>> GetUsersWithRoles(ChannelModel channel, string role, uint maxResults = 1)
        {
            Validator.ValidateVariable(channel, "channel");
            Validator.ValidateString(role, "role");
            return await this.GetPagedAsync<UserWithGroupsModel>("channels/" + channel.id + "/users/" + role, maxResults);
        }

        /// <summary>
        /// Adds and removes the specified roles for the specified user.
        /// </summary>
        /// <param name="channel">The channel to update the user for</param>
        /// <param name="user">The user to update</param>
        /// <param name="rolesToAdd">The roles to add</param>
        /// <param name="rolesToRemove">The roles to remove</param>
        /// <returns>The updated user with their roles</returns>
        public async Task<bool> UpdateUserRoles(ChannelModel channel, UserModel user, IEnumerable<string> rolesToAdd = null, IEnumerable<string> rolesToRemove = null)
        {
            Validator.ValidateVariable(channel, "channel");
            Validator.ValidateVariable(user, "user");

            if (rolesToAdd == null && rolesToRemove == null)
            {
                throw new ArgumentException("rolesToAdd or rolesToRemove must not be null");
            }

            JObject obj = new JObject();
            if (rolesToAdd != null) { obj.Add("add", JToken.FromObject(rolesToAdd)); };
            if (rolesToRemove != null) { obj.Add("remove", JToken.FromObject(rolesToRemove)); }

            HttpResponseMessage response = await this.PatchAsync("channels/" + channel.id + "/users/" + user.id, this.CreateContentFromObject(obj));
            return (response.StatusCode == System.Net.HttpStatusCode.OK);
        }

        /// <summary>
        /// Gets the Discord bot information for the specified channel.
        /// </summary>
        /// <param name="channel">The channel to get Discord bot information for</param>
        /// <returns>The Discord bot information for the channel</returns>
        public async Task<DiscordBotModel> GetDiscord(ChannelModel channel)
        {
            Validator.ValidateVariable(channel, "channel");
            return await this.GetAsync<DiscordBotModel>("channels/" + channel.id + "/discord");
        }

        /// <summary>
        /// Updates the Discrd bot information for the specified channel.
        /// </summary>
        /// <param name="channel">The channel to update discord bot information for</param>
        /// <param name="discord">The Discord bot information</param>
        /// <returns>The updated Discord bot information</returns>
        public async Task<DiscordBotModel> UpdateDiscord(ChannelModel channel, DiscordBotModel discord)
        {
            Validator.ValidateVariable(channel, "channel");
            Validator.ValidateVariable(discord, "discord");
            return await this.PutAsync<DiscordBotModel>("channels/" + channel.id + "/discord", this.CreateContentFromObject(discord));
        }

        /// <summary>
        /// Gets the Discord channels associated with this channel.
        /// </summary>
        /// <param name="channel">The channel to get Discord channels for</param>
        /// <returns>The Discord channels</returns>
        public async Task<IEnumerable<DiscordChannelModel>> GetDiscordChannels(ChannelModel channel)
        {
            Validator.ValidateVariable(channel, "channel");
            return await this.GetAsync<IEnumerable<DiscordChannelModel>>("channels/" + channel.id + "/discord/channels");
        }

        /// <summary>
        /// Gets the Discord roles associated with this channel.
        /// </summary>
        /// <param name="channel">The channel to get Discord roles for</param>
        /// <returns>The Discord roles</returns>
        public async Task<IEnumerable<DiscordRoleModel>> GetDiscordRoles(ChannelModel channel)
        {
            Validator.ValidateVariable(channel, "channel");
            return await this.GetAsync<IEnumerable<DiscordRoleModel>>("channels/" + channel.id + "/discord/roles");
        }

        /// <summary>
        /// Verifys that a Discord invite can be generated for the specified channel and user.
        /// </summary>
        /// <param name="channel">The channel to generate a Discord invite for</param>
        /// <param name="user">The user to generate a Discord invite for</param>
        /// <returns>Whether an invite could be generated</returns>
        public async Task<bool> CanUserGetDiscordInvite(ChannelModel channel, UserModel user)
        {
            Validator.ValidateVariable(channel, "channel");
            HttpResponseMessage response = await this.GetAsync("channels/" + channel.id + "/discord/roles?user=" + user.id);
            return (response.StatusCode == System.Net.HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Creates a Discord invite for the specified channel and user.
        /// </summary>
        /// <param name="channel">The channel to generate a Discord invite for</param>
        /// <param name="user">The user to generate a Discord invite for</param>
        /// <returns>The generated invite</returns>
        public async Task<string> GetUserDiscordInvite(ChannelModel channel, UserModel user)
        {
            Validator.ValidateVariable(channel, "channel");
            JObject jobj = await this.GetJObjectAsync("channels/" + channel.id + "/discord/roles?user=" + user.id);
            JToken invite;
            if (jobj.TryGetValue("redirectUri", out invite))
            {
                return invite.ToString();
            }
            return null;
        }

        private string ConstructToAndFromQueryString(DateTimeOffset startDate, DateTimeOffset? endDate = null)
        {
            string endDateString = "";
            if (endDate != null)
            {
                endDateString = "&to=" + DateTimeHelper.DateTimeOffsetToISO8601String(endDate.GetValueOrDefault());
            }
            return "from=" + DateTimeHelper.DateTimeOffsetToISO8601String(startDate) + endDateString;
        }
    }
}
