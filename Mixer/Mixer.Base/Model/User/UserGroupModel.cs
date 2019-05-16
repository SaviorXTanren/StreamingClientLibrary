namespace Mixer.Base.Model.User
{
    /// <summary>
    /// A group of users.
    /// </summary>
    public class UserGroupModel : TimeStampedModel
    {
        /// <summary>
        /// The ID of the group.
        /// </summary>
        public uint id { get; set; }
        /// <summary>
        /// The name of the group.
        /// </summary>
        public string name { get; set; }
    }
}
