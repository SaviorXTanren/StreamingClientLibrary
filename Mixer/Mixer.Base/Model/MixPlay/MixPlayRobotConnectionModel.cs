namespace Mixer.Base.Model.MixPlay
{
    /// <summary>
    /// MixPlay connection info for an automated connection.
    /// </summary>
    public class MixPlayRobotConnectionModel
    {
        /// <summary>
        /// The address to connect to.
        /// </summary>
        public string address { get; set; }
        /// <summary>
        /// The key to connect with.
        /// </summary>
        public string key { get; set; }
    }
}
