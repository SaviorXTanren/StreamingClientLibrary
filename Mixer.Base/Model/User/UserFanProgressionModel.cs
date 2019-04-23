namespace Mixer.Base.Model.User
{
    /// <summary>
    /// Information about a user's fan progression in a channel.
    /// </summary>
    public class UserFanProgressionModel
    {
        /// <summary>
        /// The user's ID.
        /// </summary>
        public uint userId { get; set; }
        /// <summary>
        /// The user's fan progression level.
        /// </summary>
        public UserFanProgressionLevelModel level { get; set; }
        /// <summary>
        /// The timestamp when this was retrieved.
        /// </summary>
        public string timestamp { get; set; }
        /// <summary>
        /// The total number of hearts the user has in the channel.
        /// </summary>
        public uint total { get; set; }
    }

    /// <summary>
    /// Information about the user's current fan progression level in a channel.
    /// </summary>
    public class UserFanProgressionLevelModel
    {
        /// <summary>
        /// The name of the rank.
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// The rank number.
        /// </summary>
        public uint level { get; set; }
        /// <summary>
        /// The color of the rank.
        /// </summary>
        public string color { get; set; }
        /// <summary>
        /// The color complement of the rank.
        /// </summary>
        public string complement { get; set; }
        /// <summary>
        /// The asset URL for the rank.
        /// </summary>
        public string assetsUrl { get; set; }
        /// <summary>
        /// The minimum XP required for the rank.
        /// </summary>
        public uint minXp { get; set; }
        /// <summary>
        /// The current XP for the rank.
        /// </summary>
        public uint currentXp { get; set; }
        /// <summary>
        /// The required XP for the next rank.
        /// </summary>
        public uint nextLevelXp { get; set; }
    }
}
