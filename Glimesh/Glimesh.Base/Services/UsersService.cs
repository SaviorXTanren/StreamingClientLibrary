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
        /// </summary>
        /// <param name="userId">The ID of the user to get follows for</param>
        /// <returns>The set of follows</returns>
        public async Task<IEnumerable<UserFollowModel>> GetUsersFollowed(string userId) { return await this.QueryAsync<IEnumerable<UserFollowModel>>($"{{ followers(userId: \"{userId}\") {{ {UserFollowModel.AllFields} }} }}", "followers"); }

        /// <summary>
        /// Gets the users that are following the specified channel
        /// </summary>
        /// <param name="streamerId">The ID of the user to get follows for</param>
        /// <returns>The set of follows</returns>
        public async Task<IEnumerable<UserFollowModel>> GetFollowingUsers(string streamerId) { return await this.QueryAsync<IEnumerable<UserFollowModel>>($"{{ followers(streamerId: \"{streamerId}\") {{ {UserFollowModel.AllFields} }} }}", "followers"); }
    }
}