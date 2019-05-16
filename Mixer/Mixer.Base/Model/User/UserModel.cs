namespace Mixer.Base.Model.User
{
    /// <summary>
    /// Information about a user.
    /// </summary>
    public class UserModel : TimeStampedModel
    {
        /// <summary>
        /// The ID of the user.
        /// </summary>
        public uint id { get; set; }
        /// <summary>
        /// The level of the user.
        /// </summary>
        public uint level { get; set; }
        /// <summary>
        /// Social information for the user.
        /// </summary>
        public SocialInfoModel social { get; set; }
        /// <summary>
        /// The user's account name.
        /// </summary>
        public string username { get; set; }
        /// <summary>
        /// The user's email.
        /// </summary>
        public string email { get; set; }
        /// <summary>
        /// Whether the user is verified.
        /// </summary>
        public bool verified { get; set; }
        /// <summary>
        /// The user's experience amount.
        /// </summary>
        public uint experience { get; set; }
        /// <summary>
        /// The user's spark total.
        /// </summary>
        public uint sparks { get; set; }
        /// <summary>
        /// The user's avatar URL.
        /// </summary>
        public string avatarUrl { get; set; }
        /// <summary>
        /// The user's bio info.
        /// </summary>
        public string bio { get; set; }
        /// <summary>
        /// The ID of the primary team for the user.
        /// </summary>
        public uint? primaryTeam { get; set; }
        /// <summary>
        /// The user's stream transcoding profile ID.
        /// </summary>
        public uint? transcodingProfileId { get; set; }
        /// <summary>
        /// Whether the user has transcoding enabled.
        /// </summary>
        public bool? hasTranscodes { get; set; }
    }
}
