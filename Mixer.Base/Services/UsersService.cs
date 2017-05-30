using Mixer.Base.Model;
using Mixer.Base.Model.Channel;
using Mixer.Base.Model.Chat;
using Mixer.Base.Model.Teams;
using Mixer.Base.Model.User;
using Mixer.Base.Util;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mixer.Base.Services
{
    public class UsersService : ServiceBase
    {
        public UsersService(MixerClient client) : base(client) { }

        public async Task<PrivatePopulatedUserModel> GetCurrentUser()
        {
            return await this.GetAsync<PrivatePopulatedUserModel>("users/current");
        }

        public async Task<UserWithChannelModel> GetUser(UserModel user)
        {
            Validator.ValidateVariable(user, "user");
            return await this.GetUser(user.id);
        }

        public async Task<UserWithChannelModel> GetUser(ChatUserEventModel user)
        {
            Validator.ValidateVariable(user, "user");
            return await this.GetUser(user.id);
        }

        public async Task<UserWithChannelModel> GetUser(ChatUserModel user)
        {
            Validator.ValidateVariable(user, "user");
            if (user.userId != null)
            {
                return await this.GetUser(user.userId.GetValueOrDefault());
            }
            return null;
        }

        public async Task<UserModel> GetUser(string username)
        {
            Validator.ValidateString(username, "username");
            IEnumerable<UserModel> users = await this.GetPagedAsync<UserModel>("users/search?where=username:eq:" + username + "&");
            if (users.Count() > 0)
            {
                return users.First();
            }
            return null;
        }

        public async Task<string> GetAvatar(UserModel user)
        {
            Validator.ValidateVariable(user, "user");
            return await this.GetStringAsync("users/" + user.id + "/avatar");
        }

        public async Task<IEnumerable<ChannelAdvancedModel>> GetFollows(UserModel user)
        {
            Validator.ValidateVariable(user, "user");
            return await this.GetPagedAsync<ChannelAdvancedModel>("users/" + user.id + "/follows");
        }

        public async Task<IEnumerable<UserLogModel>> GetLogs(UserModel user)
        {
            Validator.ValidateVariable(user, "user");
            return await this.GetPagedAsync<UserLogModel>("users/" + user.id + "/log");
        }

        public async Task<IEnumerable<NotificationModel>> GetNotifications(UserModel user)
        {
            Validator.ValidateVariable(user, "user");
            return await this.GetPagedAsync<NotificationModel>("users/" + user.id + "/notifications");
        }

        public async Task<UserPreferencesModel> GetPreferences(UserModel user)
        {
            Validator.ValidateVariable(user, "user");
            return await this.GetAsync<UserPreferencesModel>("users/" + user.id + "/preferences");
        }

        public async Task<UserPreferencesModel> UpdatePreferences(UserModel user, UserPreferencesModel preferences)
        {
            Validator.ValidateVariable(user, "user");
            return await this.PostAsync<UserPreferencesModel>("users/" + user.id + "/preferences", this.CreateContentFromObject(preferences));
        }

        public async Task<IEnumerable<TeamMembershipExpandedModel>> GetTeams(UserModel user)
        {
            Validator.ValidateVariable(user, "user");
            return await this.GetAsync<List<TeamMembershipExpandedModel>>("users/" + user.id + "/teams");
        }

        private async Task<UserWithChannelModel> GetUser(uint userID)
        {
            return await this.GetAsync<UserWithChannelModel>("users/" + userID);
        }
    }
}
