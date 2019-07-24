using Newtonsoft.Json.Linq;

namespace Mixer.Base.Model.MixPlay
{
    /// <summary>
    /// The bandwidth throttling to set for MixPlay
    /// </summary>
    public class MethodSetBandwidthThrottleModel
    {
        /// <summary>
        /// The capacity for throttle connection.
        /// </summary>
        public long capacity { get; set; }
        /// <summary>
        /// The drain rate for throttle connections.
        /// </summary>
        public long drainRate { get; set; }
    }

    /// <summary>
    /// The bandwidth throttling to set for MixPlay.
    /// </summary>
    public class MixPlaySetBandwidthThrottleModel : JObject
    {
        /// <summary>
        /// Sets the throttling settings for MixPlay.
        /// </summary>
        /// <param name="methodName">The name of the method</param>
        /// <param name="capacity">The capacity for throttle connection</param>
        /// <param name="drainRate">The drain rate for throttle connections</param>
        public void AddThrottle(string methodName, long capacity, long drainRate)
        {
            MethodSetBandwidthThrottleModel method = new MethodSetBandwidthThrottleModel();
            method.capacity = capacity;
            method.drainRate = drainRate;
            this.Add(methodName, JObject.FromObject(method));
        }
    }
}
