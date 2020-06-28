using Newtonsoft.Json.Linq;
using StreamingClient.Base.Util;
using StreamingClient.Base.Web;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Twitch.Base.Models.NewAPI;
using Twitch.Base.Models.NewAPI.Users;

namespace Twitch.Base.Services.NewAPI
{
    /// <summary>
    /// The APIs for User-based services.
    /// </summary>
    public class UsersService : NewTwitchAPIServiceBase
    {
        /// <summary>
        /// Creates an instance of the UsersService.
        /// </summary>
        /// <param name="connection">The Twitch connection to use</param>
        public UsersService(TwitchConnection connection) : base(connection) { }

        /// <summary>
        /// Gets the current user.
        /// </summary>
        /// <returns>The resulting user</returns>
        public async Task<UserModel> GetCurrentUser()
        {
            IEnumerable<UserModel> users = await this.GetDataResultAsync<UserModel>("users");
            return (users != null) ? users.FirstOrDefault() : null;
        }

        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <param name="user">The user to get</param>
        /// <returns>The resulting user</returns>
        public async Task<UserModel> GetUser(UserModel user) { return await this.GetUserByID(user.id); }

        /// <summary>
        /// Gets a user by their user ID.
        /// </summary>
        /// <param name="userID">The ID of the user</param>
        /// <returns>The user associated with the ID</returns>
        public async Task<UserModel> GetUserByID(string userID)
        {
            IEnumerable<UserModel> users = await this.GetUsersByID(new List<string>() { userID });
            return users.FirstOrDefault();
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
        /// Gets the users by their user IDs.
        /// </summary>
        /// <param name="userIDs">The IDs of the users</param>
        /// <returns>The users associated with the IDs</returns>
        public async Task<IEnumerable<UserModel>> GetUsersByID(IEnumerable<string> userIDs) { return await this.GetUsers(userIDs, new List<string>()); }

        /// <summary>
        /// Gets the users by their logins.
        /// </summary>
        /// <param name="logins">The logins of the users</param>
        /// <returns>The users associated with the logins</returns>
        public async Task<IEnumerable<UserModel>> GetUsersByLogin(IEnumerable<string> logins) { return await this.GetUsers(new List<string>(), logins); }

        /// <summary>
        /// Gets the users by their user IDs &amp; logins.
        /// </summary>
        /// <param name="userIDs">The IDs of the users</param>
        /// <param name="logins">The logins of the users</param>
        /// <returns>The users associated with the IDs &amp; logins</returns>
        public async Task<IEnumerable<UserModel>> GetUsers(IEnumerable<string> userIDs, IEnumerable<string> logins)
        {
            Validator.ValidateVariable(userIDs, "userIDs");
            Validator.ValidateVariable(logins, "logins");
            Validator.Validate((userIDs.Count() > 0 || logins.Count() > 0), "At least one userID or login must be specified");

            List<string> parameters = new List<string>();
            foreach (string userID in userIDs)
            {
                parameters.Add("id=" + userID);
            }
            foreach (string login in logins)
            {
                parameters.Add("login=" + login);
            }

            return await this.GetDataResultAsync<UserModel>("users?" + string.Join("&", parameters));
        }

        /// <summary>
        /// Gets follower information for and/or to a user.
        /// </summary>
        /// <param name="from">The user to search for who they follow</param>
        /// <param name="to">The user to search for who follows them</param>
        /// <param name="maxResults">The maximum number of results. Will be either that amount or slightly more</param>
        /// <returns>The follow information</returns>
        public async Task<IEnumerable<UserFollowModel>> GetFollows(UserModel from = null, UserModel to = null, int maxResults = 1) { return await this.GetFollows(from?.id, to?.id, maxResults); }

        /// <summary>
        /// Gets follower information for and/or to a user.
        /// </summary>
        /// <param name="fromID">The user to search for who they follow</param>
        /// <param name="toID">The user to search for who follows them</param>
        /// <param name="maxResults">The maximum number of results. Will be either that amount or slightly more</param>
        /// <returns>The follow information</returns>
        public async Task<IEnumerable<UserFollowModel>> GetFollows(string fromID = null, string toID = null, int maxResults = 1)
        {
            Validator.Validate(!string.IsNullOrEmpty(fromID) || !string.IsNullOrEmpty(toID), "At least either fromID or toID must be specified");

            Dictionary<string, string> queryParameters = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(fromID))
            {
                queryParameters.Add("from_id", fromID);
            }
            if (!string.IsNullOrEmpty(toID))
            {
                queryParameters.Add("to_id", toID);
            }

            return await this.GetPagedDataResultAsync<UserFollowModel>("users/follows?" + string.Join("&", queryParameters.Select(kvp => kvp.Key + "=" + kvp.Value)), maxResults);
        }

        /// <summary>
        /// Adds the specified "to" user to the followers of the specified "from" user.
        /// </summary>
        /// <param name="from">The user to perform the follow</param>
        /// <param name="to">The user to be followed</param>
        /// <returns>Whether the follow was successful</returns>
        public async Task<bool> FollowUser(UserModel from, UserModel to) { return await this.FollowUser(from?.id, to?.id); }

        /// <summary>
        /// Adds the specified "to" user ID to the followers of the specified "from" user ID.
        /// </summary>
        /// <param name="from">The user ID to perform the follow</param>
        /// <param name="to">The user ID to be followed</param>
        /// <returns>Whether the follow was successful</returns>
        public async Task<bool> FollowUser(string from, string to)
        {
            Validator.ValidateVariable(from, "from");
            Validator.ValidateVariable(to, "to");

            JObject jobj = new JObject();
            jobj["from_id"] = from;
            jobj["to_id"] = to;
            HttpResponseMessage response = await this.PostAsync("users/follows", AdvancedHttpClient.CreateContentFromObject(jobj));
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Removes the specified "to" user from the followers of the specified "from" user.
        /// </summary>
        /// <param name="from">The user to perform the unfollow</param>
        /// <param name="to">The user to be unfollowed</param>
        /// <returns>Whether the unfollow was successful</returns>
        public async Task<bool> UnfollowUser(UserModel from, UserModel to) { return await this.UnfollowUser(from?.id, to?.id); }

        /// <summary>
        /// Removes the specified "to" user ID from the followers of the specified "from" user ID.
        /// </summary>
        /// <param name="from">The user ID to perform the unfollow</param>
        /// <param name="to">The user ID to be unfollowed</param>
        /// <returns>Whether the unfollow was successful</returns>
        public async Task<bool> UnfollowUser(string from, string to)
        {
            Validator.ValidateVariable(from, "from");
            Validator.ValidateVariable(to, "to");
            return await this.DeleteAsync($"users/follows?from_id={from}&to_id={to}");
        }

        /// <summary>
        /// Updates the description of the current user.
        /// </summary>
        /// <param name="description">The description to set</param>
        /// <returns>The updated current user</returns>
        public async Task<UserModel> UpdateCurrentUserDescription(string description)
        {
            NewTwitchAPIDataRestResult<UserModel> result = await this.PutAsync<NewTwitchAPIDataRestResult<UserModel>>("users?description=" + AdvancedHttpClient.EncodeString(description));
            if (result != null && result.data != null)
            {
                return result.data.FirstOrDefault();
            }
            return null;
        }
    }
}
