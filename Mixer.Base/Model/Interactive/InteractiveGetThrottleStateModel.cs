using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Mixer.Base.Model.Interactive
{
    public class MethodGetThrottleStateModel
    {
        public MethodGetThrottleStateModel() { }

        public MethodGetThrottleStateModel(JToken values)
        {
            this.inserted = long.Parse(values["inserted"].ToString());
            this.rejected = long.Parse(values["rejected"].ToString());
        }

        public long inserted { get; set; }
        public long rejected { get; set; }
    }

    public class InteractiveGetThrottleStateModel
    {
        public Dictionary<string, MethodGetThrottleStateModel> MethodThrottles { get; set; }

        public InteractiveGetThrottleStateModel() { this.MethodThrottles = new Dictionary<string, MethodGetThrottleStateModel>(); }

        public InteractiveGetThrottleStateModel(JObject result)
            : this()
        {
            if (result.HasValues)
            {
                foreach (JProperty property in result.Properties())
                {
                    this.MethodThrottles.Add(property.Name, new MethodGetThrottleStateModel(property.Value));
                }
            }
        }
    }
}
