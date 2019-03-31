namespace Mixer.Base.Model.Channel
{
    /// <summary>
    /// Analytic information of a viewer.
    /// </summary>
    public class ViewerAnalyticModel : ChannelAnalyticModel
    {
        /// <summary>
        /// The country of the viewer.
        /// </summary>
        public string country { get; set; }
        /// <summary>
        /// The browser of the viewer.
        /// </summary>
        public string browser { get; set; }
        /// <summary>
        /// The platform of the version.
        /// </summary>
        public string platform { get; set; }
    }
}
