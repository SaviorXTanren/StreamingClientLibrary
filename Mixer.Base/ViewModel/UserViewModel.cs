using Mixer.Base.Model.User;
using System;

namespace Mixer.Base.ViewModel
{
    public class UserViewModel : IEquatable<UserViewModel>
    {
        public uint ID { get; private set; }

        public string UserName { get; private set; }

        public UserViewModel(uint id, string username)
        {
            this.ID = id;
            this.UserName = username;
        }

        public UserModel GetModel()
        {
            return new UserModel()
            {
                id = this.ID,
                username = this.UserName,
            };
        }

        public override bool Equals(object obj)
        {
            if (obj is UserViewModel)
            {
                return this.Equals((UserViewModel)obj);
            }
            return false;
        }

        public bool Equals(UserViewModel other) { return this.ID.Equals(other.ID); }

        public override int GetHashCode() { return this.ID.GetHashCode(); }

        public override string ToString() { return this.UserName; }
    }
}
