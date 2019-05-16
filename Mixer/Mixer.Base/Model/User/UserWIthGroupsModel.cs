using System;
using System.Linq;

namespace Mixer.Base.Model.User
{
    /// <summary>
    /// A user and the groups they are part of.
    /// </summary>
    public class UserWithGroupsModel : UserModel
    {
        /// <summary>
        /// The groups that the user is a part of.
        /// </summary>
        public UserGroupModel[] groups { get; set; }

        /// <summary>
        /// Gets the created date for a group if the user has the group and the user is still active in that group
        /// </summary>
        /// <param name="groupName">The name of the group</param>
        /// <returns>The created date for the group or null</returns>
        public DateTimeOffset? GetCreatedDateForGroupIfCurrent(string groupName)
        {
            if (this.groups != null)
            {
                UserGroupModel subscriberGroup = this.groups.FirstOrDefault(g => g.name.Equals(groupName) && g.deletedAt == null);
                if (subscriberGroup != null)
                {
                    return subscriberGroup.createdAt;
                }
            }
            return null;
        }
    }
}
