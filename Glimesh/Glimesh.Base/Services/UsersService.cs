using Glimesh.Base.Models.Users;
using System.Threading.Tasks;

namespace Glimesh.Base.Services
{
    /// <summary>
    /// The APIs for user-based services.
    /// </summary>
    public class UsersService : GlimeshServiceBase
    {
        /// <summary>
        /// Creates an instance of the ChatService.
        /// </summary>
        /// <param name="connection">The Trovo connection to use</param>
        public UsersService(GlimeshConnection connection) : base(connection) { }

        /// <summary>
        /// Gets the currently authenticated user.
        /// </summary>
        /// <returns>The currently authenticated user</returns>
        public async Task<UserModel> GetCurrentUser() { return await this.QueryAsync<UserModel>($"{{  myself {{     {UserModel.AllFieldsWithSocials}    }}  }}", "myself"); }

        /// <summary>
        /// Gets the user with the specified id.
        /// </summary>
        /// <param name="id">The id of the user</param>
        /// <returns>The user</returns>
        public async Task<UserModel> GetUserByID(string id) { return await this.QueryAsync<UserModel>($"{{ user(id: {id}) {{ {UserModel.AllFieldsWithSocials} }} }}", "user"); }

        /// <summary>
        /// Gets the user with the specified name.
        /// </summary>
        /// <param name="username">The name of the user</param>
        /// <returns>The user</returns>
        public async Task<UserModel> GetUserByName(string username) { return await this.QueryAsync<UserModel>($"{{  user(username: \"{username}\") {{ {UserModel.AllFieldsWithSocials} }} }}", "user"); }
    }
}