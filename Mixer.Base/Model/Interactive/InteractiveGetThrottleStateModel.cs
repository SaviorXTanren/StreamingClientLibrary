using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Mixer.Base.Model.Interactive
{
    public class MethodGetThrottleStateModel
    {
        public long inserted { get; set; }
        public long rejected { get; set; }
    }

    public class InteractiveGetThrottleStateModel : JObject
    {
        public Dictionary<string, MethodGetThrottleStateModel> GetAllThrottles()
        {
            Dictionary<string, MethodGetThrottleStateModel> throttles = new Dictionary<string, MethodGetThrottleStateModel>();
            if (this.HasValues)
            {
                foreach (JProperty property in this.Properties())
                {
                    throttles.Add(property.Name, property.Value.ToObject<MethodGetThrottleStateModel>());
                }
            }
            return throttles;
        }
    }
}
