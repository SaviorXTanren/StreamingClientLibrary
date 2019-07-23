namespace Mixer.Base.Model.Channel
{
    /// <summary>
    /// Augmented ChannelAdvanced with additional properties.
    /// </summary>
    public class ExpandedChannelModel : ChannelAdvancedModel
    {
        /// <summary>
        /// A resource object representing the thumbnail.
        /// </summary>
        public ResourceModel thumbnail { get; set; }
        /// <summary>
        /// A resource object representing the cover.
        /// </summary>
        public ResourceModel cover { get; set; }
        /// <summary>
        ///  resource object representing the badge.
        /// </summary>
        public ResourceModel badge { get; set; }
        /// <summary>
        /// The channel preferences.
        /// </summary>
        public ChannelPreferencesModel preferences { get; set; }
    }
}
