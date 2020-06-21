using Newtonsoft.Json.Linq;
using StreamingClient.Base.Util;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Twitch.Base.Models.V5.Channel;
using Twitch.Base.Models.V5.Emotes;
using Twitch.Base.Models.V5.Users;

namespace Twitch.Base.Services.V5API
{
    /// <summary>
    /// The APIs for User-based services.
    /// </summary>
    public class UsersService : V5APIServiceBase
    {
        /// <summary>
        /// Creates an instance of the UsersService.
        /// </summary>
        /// <param name="connection">The Twitch connection to use</param>
        public UsersService(TwitchConnection connection) : base(connection) { }

        /// <summary>
        /// Gets the currently authenticated user.
        /// </summary>
        /// <returns>The currently authenticated user</returns>
        public async Task<UserModel> GetCurrentUser() { return await this.GetAsync<UserModel>("user"); }

        /// <summary>
        /// Gets a user by their user ID.
        /// </summary>
        /// <param name="userID">The ID of the user</param>
        /// <returns>The user associated with the ID</returns>
        public async Task<UserModel> GetUserByID(string userID)
        {
            Validator.ValidateString(userID, "userID");
            return await this.GetAsync<UserModel>("users/" + userID);
        }

        /// <summary>
        /// Gets a user by their login.
        /// </summary>
        /// <param name="login">The login of the user</param>
        /// <returns>The user associated with the login</returns>
        public async Task<UserModel> GetUserByLogin(string login)
        {
            IEnumerable<UserModel> users = await this.GetUsersByLogin(new List<string>() { login });
            return users.FirstOrDefault();
        }

        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <param name="user">The user to get</param>
        /// <returns>The resulting user</returns>
        public async Task<UserModel> GetUser(UserModel user) { return await this.GetUserByID(user.id); }

        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <param name="channel">The channel to get the user for</param>
        /// <returns>The resulting user</returns>
        public async Task<UserModel> GetUser(ChannelModel channel) { return await this.GetUserByID(channel.id); }

        /// <summary>
        /// Gets the users by their logins.
        /// </summary>
        /// <param name="logins">The logins of the users</param>
        /// <returns>The users associated with the logins</returns>
        public async Task<IEnumerable<UserModel>> GetUsersByLogin(IEnumerable<string> logins)
        {
            Validator.ValidateList(logins, "logins");
            return await this.GetNamedArrayAsync<UserModel>("users?login=" + string.Join(",", logins), "users");
        }

        /// <summary>
        /// Gets the emotes available to the user.
        /// </summary>
        /// <param name="user">The user to get emotes for</param>
        /// <returns>The available emotes</returns>
        public async Task<IEnumerable<EmoteModel>> GetUserEmotes(UserModel user)
        {
            Validator.ValidateVariable(user, "user");

            JObject jobj = await this.GetJObjectAsync("users/" + user.id + "/emotes");
            List<EmoteModel> results = new List<EmoteModel>();
            if (jobj != null && jobj.ContainsKey("emoticon_sets"))
            {
                JObject emoticonSets = (JObject)jobj["emoticon_sets"];
                foreach (var kvp in emoticonSets)
                {
                    string setID = kvp.Key;
                    foreach (JToken token in (JArray)kvp.Value)
                    {
                        EmoteModel emote = token.ToObject<EmoteModel>();
                        emote.setID = setID;
                        results.Add(emote);
                    }
                }
            }
            return results;
        }

        /// <summary>
        /// Gets subscription information for a user to a channel.
        /// </summary>
        /// <param name="user">The user to check for.</param>
        /// <param name="channel">The channel to check for</param>
        /// <returns>The subscription information, if it exists</returns>
        public async Task<UserSubscriptionModel> CheckUserSubscription(UserModel user, ChannelModel channel)
        {
            Validator.ValidateVariable(channel, "channel");
            Validator.ValidateVariable(user, "user");
            return await this.GetAsync<UserSubscriptionModel>("users/" + user.id + "/subscriptions/" + channel.id);
        }

        /// <summary>
        /// Gets the channels that the user follows
        /// </summary>
        /// <param name="user">The user to get follows for</param>
        /// <param name="maxResults">The maximum number of results. Will be either that amount or slightly more</param>
        /// <returns>The channels the user follows</returns>
        public async Task<IEnumerable<UserFollowModel>> GetUserFollows(UserModel user, int maxResults = 1)
        {
            Validator.ValidateVariable(user, "user");
            return await this.GetOffsetPagedResultAsync<UserFollowModel>("users/" + user.id + "/follows/channels", "follows", maxResults);
        }

        /// <summary>
        /// Gets follow information for a user to a channel.
        /// </summary>
        /// <param name="user">The user to check for.</param>
        /// <param name="channel">The channel to check for</param>
        /// <returns>The follow information, if it exists</returns>
        public async Task<UserFollowModel> CheckUserFollow(UserModel user, ChannelModel channel)
        {
            Validator.ValidateVariable(user, "user");
            Validator.ValidateVariable(channel, "channel");
            return await this.GetAsync<UserFollowModel>("users/" + user.id + "/follows/channels/" + channel.id);
        }

        /// <summary>
        /// Has the specified user follow the specified channel.
        /// </summary>
        /// <param name="user">The user to follow for</param>
        /// <param name="channel">The channel to follow</param>
        /// <returns>The follow information, if it exists</returns>
        public async Task<UserFollowModel> FollowChannel(UserModel user, ChannelModel channel)
        {
            Validator.ValidateVariable(user, "user");
            Validator.ValidateVariable(channel, "channel");
            return await this.PutAsync<UserFollowModel>("users/" + user.id + "/follows/channels/" + channel.id);
        }

        /// <summary>
        /// Has the specified user unfollow the specified channel.
        /// </summary>
        /// <param name="user">The user to unfollow for</param>
        /// <param name="channel">The channel to unfollow</param>
        /// <returns>Whether the deletion was successful</returns>
        public async Task<bool> UnfollowChannel(UserModel user, ChannelModel channel)
        {
            Validator.ValidateVariable(user, "user");
            Validator.ValidateVariable(channel, "channel");
            return await this.DeleteAsync("users/" + user.id + "/follows/channels/" + channel.id);
        }

        /// <summary>
        /// Gets the users that have been blocked
        /// </summary>
        /// <param name="user">The user to get blocked users for</param>
        /// <param name="maxResults">The maximum number of results. Will be either that amount or slightly more</param>
        /// <returns>The blocked users</returns>
        public async Task<IEnumerable<UserModel>> GetUserBlockList(UserModel user, int maxResults = 1)
        {
            Validator.ValidateVariable(user, "user");
            return await this.GetOffsetPagedResultAsync<UserModel>("users/" + user.id + "/blocks", "blocks", maxResults);
        }

        /// <summary>
        /// Has the specified user block the specified block user.
        /// </summary>
        /// <param name="user">The user to block for</param>
        /// <param name="blockUser">The user to block</param>
        /// <returns>The user information</returns>
        public async Task<UserModel> BlockUser(UserModel user, UserModel blockUser)
        {
            Validator.ValidateVariable(user, "user");
            Validator.ValidateVariable(blockUser, "blockUser");
            return await this.PutAsync<UserModel>("users/" + user.id + "/blocks/" + blockUser.id);
        }

        /// <summary>
        /// Has the specified user unblock the specified block user.
        /// </summary>
        /// <param name="user">The user to unblock for</param>
        /// <param name="blockUser">The user to unblock</param>
        /// <returns>Whether the deletion was successful</returns>
        public async Task<bool> UnblockUser(UserModel user, UserModel blockUser)
        {
            Validator.ValidateVariable(user, "user");
            Validator.ValidateVariable(blockUser, "blockUser");
            return await this.DeleteAsync("users/" + user.id + "/blocks/" + blockUser.id);
        }
    }
}
