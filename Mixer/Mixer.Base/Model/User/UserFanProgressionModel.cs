using Newtonsoft.Json;

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
        private const string AssetURLVariantReplacementText = "{variant}";

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
        public long? minXp { get; set; }
        /// <summary>
        /// The current XP for the rank.
        /// </summary>
        public long? currentXp { get; set; }
        /// <summary>
        /// The required XP for the next rank.
        /// </summary>
        public long? nextLevelXp { get; set; }

        /// <summary>
        /// The small version of the asset URL for the rank.
        /// </summary>
        [JsonIgnore]
        public string SmallAssetURL { get { return this.assetsUrl.Replace(AssetURLVariantReplacementText, "small.png"); } }
        /// <summary>
        /// The large version of the asset URL for the rank.
        /// </summary>
        [JsonIgnore]
        public string LargeAssetURL { get { return this.assetsUrl.Replace(AssetURLVariantReplacementText, "large.png"); } }
        /// <summary>
        /// The large GIF version of the asset URL for the rank.
        /// </summary>
        [JsonIgnore]
        public string LargeGIFAssetURL { get { return this.assetsUrl.Replace(AssetURLVariantReplacementText, "large.gif"); } }
    }
}
