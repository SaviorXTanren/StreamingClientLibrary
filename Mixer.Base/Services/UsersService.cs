using Mixer.Base.Model;
using Mixer.Base.Model.Channel;
using Mixer.Base.Model.Chat;
using Mixer.Base.Model.Subscription;
using Mixer.Base.Model.Teams;
using Mixer.Base.Model.User;
using Mixer.Base.Util;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mixer.Base.Services
{
    /// <summary>
    /// The APIs for user-based services.
    /// </summary>
    public class UsersService : MixerServiceBase
    {
        /// <summary>
        /// Creates an instance of the UsersService.
        /// </summary>
        /// <param name="connection">The Mixer connection to use</param>
        public UsersService(MixerConnection connection) : base(connection) { }

        /// <summary>
        /// Gets the currently authenicated user.
        /// </summary>
        /// <returns>The currently authenticated user</returns>
        public async Task<PrivatePopulatedUserModel> GetCurrentUser()
        {
            return await this.GetAsync<PrivatePopulatedUserModel>("users/current");
        }

        /// <summary>
        /// Gets information about the specified user.
        /// </summary>
        /// <param name="user">The user to get</param>
        /// <returns>The user's information</returns>
        public async Task<UserWithChannelModel> GetUser(UserModel user)
        {
            Validator.ValidateVariable(user, "user");
            return await this.GetUser(user.id);
        }

        /// <summary>
        /// Gets information about the specified user.
        /// </summary>
        /// <param name="user">The user to get</param>
        /// <returns>The user's information</returns>
        public async Task<UserWithChannelModel> GetUser(ChatUserEventModel user)
        {
            Validator.ValidateVariable(user, "user");
            return await this.GetUser(user.id);
        }

        /// <summary>
        /// Gets information about the specified user.
        /// </summary>
        /// <param name="user">The user to get</param>
        /// <returns>The user's information</returns>
        public async Task<UserWithChannelModel> GetUser(ChatUserModel user)
        {
            Validator.ValidateVariable(user, "user");
            if (user.userId != null)
            {
                return await this.GetUser(user.userId.GetValueOrDefault());
            }
            return null;
        }

        /// <summary>
        /// Gets information about the user with the specified id.
        /// </summary>
        /// <param name="userID">The id of the user to get</param>
        /// <returns>The user's information</returns>
        public async Task<UserWithChannelModel> GetUser(uint userID)
        {
            Validator.ValidateVariable(userID, "userID");
            return await this.GetAsync<UserWithChannelModel>("users/" + userID);
        }

        /// <summary>
        /// Gets the user with the specified username.
        /// </summary>
        /// <param name="username">The username to search for</param>
        /// <returns>The user with the specified username</returns>
        public async Task<UserModel> GetUser(string username)
        {
            Validator.ValidateString(username, "username");
            IEnumerable<UserModel> users = await this.GetPagedAsync<UserModel>("users/search?query=" + username);
            if (users.Count() > 0)
            {
                return users.First();
            }
            return null;
        }

        /// <summary>
        /// Gets a link to the avatar of the specified user.
        /// </summary>
        /// <param name="user">The user to get the avatar for</param>
        /// <returns>A link to the avatar</returns>
        public async Task<string> GetAvatar(UserModel user)
        {
            Validator.ValidateVariable(user, "user");
            return await this.GetStringAsync("users/" + user.id + "/avatar");
        }

        /// <summary>
        /// Gets all users that the specified user follows. The search can be limited to a maximum number of results to speed up
        /// the operation as it can take a long time on large channels. This maximum number is a lower threshold and slightly
        /// more than the maximum number may be returned.
        /// </summary>
        /// <param name="user">The user to get follows for</param>
        /// <param name="maxResults">The maximum number of results. Will be either that amount or slightly more</param>
        /// <returns>All users that the specified user follows</returns>
        public async Task<IEnumerable<ChannelAdvancedModel>> GetFollows(UserModel user, uint maxResults = 1)
        {
            Validator.ValidateVariable(user, "user");
            return await this.GetPagedAsync<ChannelAdvancedModel>("users/" + user.id + "/follows", maxResults);
        }

        /// <summary>
        /// Gets all channels that the specified user is subscribed to. The search can be limited to a maximum number of results to speed up
        /// the operation as it can take a long time on large channels. This maximum number is a lower threshold and slightly
        /// more than the maximum number may be returned.
        /// </summary>
        /// <param name="user">The user to get subscriptions for</param>
        /// <param name="maxResults">The maximum number of results. Will be either that amount or slightly more</param>
        /// <returns>All users that the specified user is subscribed to</returns>
        public async Task<IEnumerable<SubscriptionWithGroupModel>> GetSubscriptions(UserModel user, uint maxResults = 1)
        {
            Validator.ValidateVariable(user, "user");
            return await this.GetPagedAsync<SubscriptionWithGroupModel>("users/" + user.id + "/subscriptions", maxResults);
        }

        /// <summary>
        /// Gets all logs for the specified user follows. The search can be limited to a maximum number of results to speed up
        /// the operation as it can take a long time on large channels. This maximum number is a lower threshold and slightly
        /// more than the maximum number may be returned.
        /// </summary>
        /// <param name="user">The user to get logs for</param>
        /// <param name="maxResults">The maximum number of results. Will be either that amount or slightly more</param>
        /// <returns>All logs for the specified user</returns>
        public async Task<IEnumerable<UserLogModel>> GetLogs(UserModel user, uint maxResults = 1)
        {
            Validator.ValidateVariable(user, "user");
            return await this.GetPagedAsync<UserLogModel>("users/" + user.id + "/log", maxResults);
        }

        /// <summary>
        /// Gets all notifications for the specified user follows. The search can be limited to a maximum number of results to speed up
        /// the operation as it can take a long time on large channels. This maximum number is a lower threshold and slightly
        /// more than the maximum number may be returned.
        /// </summary>
        /// <param name="user">The user to get notifications for</param>
        /// <param name="maxResults">The maximum number of results. Will be either that amount or slightly more</param>
        /// <returns>All notifications for the specified user</returns>
        public async Task<IEnumerable<NotificationModel>> GetNotifications(UserModel user, uint maxResults = 1)
        {
            Validator.ValidateVariable(user, "user");
            return await this.GetPagedAsync<NotificationModel>("users/" + user.id + "/notifications", maxResults);
        }

        /// <summary>
        /// Gets the preferences for the specified user.
        /// </summary>
        /// <param name="user">The user to get preferences for</param>
        /// <returns>The preferences of the user</returns>
        public async Task<UserPreferencesModel> GetPreferences(UserModel user)
        {
            Validator.ValidateVariable(user, "user");
            return await this.GetAsync<UserPreferencesModel>("users/" + user.id + "/preferences");
        }

        /// <summary>
        /// Updates the preferences of the specified user.
        /// </summary>
        /// <param name="user">The user to update preferences for</param>
        /// <param name="preferences">The preferences to update</param>
        /// <returns>The updated preferences</returns>
        public async Task<UserPreferencesModel> UpdatePreferences(UserModel user, UserPreferencesModel preferences)
        {
            Validator.ValidateVariable(user, "user");
            return await this.PostAsync<UserPreferencesModel>("users/" + user.id + "/preferences", this.CreateContentFromObject(preferences));
        }

        /// <summary>
        /// Gets all of the teams that the specified user is part of.
        /// </summary>
        /// <param name="user">The user to get teams for</param>
        /// <returns>The teams the specified user is part of</returns>
        public async Task<IEnumerable<TeamMembershipExpandedModel>> GetTeams(UserModel user)
        {
            Validator.ValidateVariable(user, "user");
            return await this.GetAsync<List<TeamMembershipExpandedModel>>("users/" + user.id + "/teams");
        }
    }
}
