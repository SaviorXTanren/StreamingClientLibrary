namespace Twitch.Base.Models.V5.Channel
{
    /// <summary>
    /// Information about a channel commercial.
    /// </summary>
    public class ChannelCommercialModel
    {
        /// <summary>
        /// The length of the commercial.
        /// </summary>
        public int Length { get; set; }
        /// <summary>
        /// The message of the commercial
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// The amount of minutes to retry to show a commercial.
        /// </summary>
        public int RetryAfter { get; set; }
    }
}
