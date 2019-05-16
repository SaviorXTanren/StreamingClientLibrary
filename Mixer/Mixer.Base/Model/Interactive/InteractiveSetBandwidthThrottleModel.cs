using Newtonsoft.Json.Linq;

namespace Mixer.Base.Model.Interactive
{
    public class MethodSetBandwidthThrottleModel
    {
        public long capacity { get; set; }
        public long drainRate { get; set; }
    }

    public class InteractiveSetBandwidthThrottleModel : JObject
    {
        public void AddThrottle(string methodName, long capacity, long drainRate)
        {
            MethodSetBandwidthThrottleModel method = new MethodSetBandwidthThrottleModel();
            method.capacity = capacity;
            method.drainRate = drainRate;
            this.Add(methodName, JObject.FromObject(method));
        }
    }
}
