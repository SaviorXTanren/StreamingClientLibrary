namespace Mixer.Base.Model.User
{
    /// <summary>
    /// Social information for a model.
    /// </summary>
    public class SocialInfoModel
    {
        /// <summary>
        /// The Twitter URL.
        /// </summary>
        public string twitter { get; set; }
        /// <summary>
        /// The Facebook URL.
        /// </summary>
        public string facebook { get; set; }
        /// <summary>
        /// The YouTube URL.
        /// </summary>
        public string youtube { get; set; }
        /// <summary>
        /// The Player.me URL.
        /// </summary>
        public string player { get; set; }
        /// <summary>
        /// The Discord account.
        /// </summary>
        public string discord { get; set; }
        /// <summary>
        /// Additional verified accounts.
        /// </summary>
        public string[] verified { get; set; }
    }
}
