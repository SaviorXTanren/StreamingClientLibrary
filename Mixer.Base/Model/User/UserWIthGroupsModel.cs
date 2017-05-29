namespace Mixer.Base.Model.User
{
    public class UserWithGroupsModel : UserModel
    {
        public UserGroupModel[] groups { get; set; }
    }
}
