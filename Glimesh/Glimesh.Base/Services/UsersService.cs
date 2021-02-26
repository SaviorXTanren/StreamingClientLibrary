using Glimesh.Base.Models.Users;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Glimesh.Base.Services
{
    /// <summary>
    /// The APIs for User-based services.
    /// </summary>
    public class UsersService : GlimeshServiceBase
    {
        /// <summary>
        /// Creates an instance of the ChatService.
        /// </summary>
        /// <param name="connection">The Glimesh connection to use</param>
        public UsersService(GlimeshConnection connection) : base(connection) { }

        /// <summary>
        /// Gets the currently authenticated user.
        /// </summary>
        /// <returns>The currently authenticated user</returns>
        public async Task<UserModel> GetCurrentUser() { return await this.QueryAsync<UserModel>($"{{ myself {{ {UserModel.AllFields} }} }}", "myself"); }

        /// <summary>
        /// Gets the user with the specified id.
        /// </summary>
        /// <param name="id">The id of the user</param>
        /// <returns>The user</returns>
        public async Task<UserModel> GetUserByID(string id) { return await this.QueryAsync<UserModel>($"{{ user(id: {id}) {{ {UserModel.AllFields} }} }}", "user"); }

        /// <summary>
        /// Gets the user with the specified name.
        /// </summary>
        /// <param name="username">The name of the user</param>
        /// <returns>The user</returns>
        public async Task<UserModel> GetUserByName(string username) { return await this.QueryAsync<UserModel>($"{{  user(username: \"{username}\") {{ {UserModel.AllFields} }} }}", "user"); }

        /// <summary>
        /// Gets the users that the specified user are following.
        /// <param name="username">The user to get follows for</param>
        /// <returns>The set of follows</returns>
        public async Task<IEnumerable<UserFollowModel>> GetUsersFollowed(string username) { return await this.QueryAsync<IEnumerable<UserFollowModel>>($"{{ followers(userUsername: \"{username}\") {{ {UserFollowModel.AllFieldsWithStreamerAndUser} }} }}", "followers"); }

        /// <summary>
        /// Gets the users that are following the specified channel.
        /// <param name="username">The user to get follows for</param>
        /// <returns>The set of follows</returns>
        public async Task<IEnumerable<UserFollowModel>> GetFollowingUsers(string username) { return await this.QueryAsync<IEnumerable<UserFollowModel>>($"{{ followers(streamerUsername: \"{username}\") {{ {UserFollowModel.AllFieldsWithStreamerAndUser} }} }}", "followers"); }

        /// <summary>
        /// Gets the users that the specified user are subscribed to.
        /// <param name="username">The user to get subscriptions for</param>
        /// <returns>The set of subscriptions</returns>
        public async Task<IEnumerable<UserSubscriptionModel>> GetUsersSubscribedTo(string username) { return await this.QueryAsync<IEnumerable<UserSubscriptionModel>>($"{{ subscriptions(userUsername: \"{username}\") {{ {UserSubscriptionModel.AllFields} }} }}", "subscriptions"); }

        /// <summary>
        /// Gets the users that are subscribed to the specified channel.
        /// <param name="username">The user to get subscriptions for</param>
        /// <returns>The set of subscriptions</returns>
        public async Task<IEnumerable<UserSubscriptionModel>> GetSubscribedUsers(string username) { return await this.QueryAsync<IEnumerable<UserSubscriptionModel>>($"{{ subscriptions(streamerUsername: \"{username}\") {{ {UserSubscriptionModel.AllFields} }} }}", "subscriptions"); }
    }
}