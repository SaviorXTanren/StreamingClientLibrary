using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Mixer.Base.Model.MixPlay
{
    /// <summary>
    /// The throttle state of MixPlay.
    /// </summary>
    public class MethodGetThrottleStateModel
    {
        /// <summary>
        /// Creates a new instance of the MethodGetThrottleStateModel class.
        /// </summary>
        public MethodGetThrottleStateModel() { }

        /// <summary>
        /// Creates a new instance of the MethodGetThrottleStateModel class.
        /// </summary>
        /// <param name="values">The values to set.</param>
        public MethodGetThrottleStateModel(JToken values)
        {
            this.inserted = long.Parse(values["inserted"].ToString());
            this.rejected = long.Parse(values["rejected"].ToString());
        }

        /// <summary>
        /// The number of sent packets.
        /// </summary>
        public long inserted { get; set; }
        /// <summary>
        /// The number of rejected packets.
        /// </summary>
        public long rejected { get; set; }
    }

    /// <summary>
    /// The throttle state of MixPlay.
    /// </summary>
    public class MixPlayGetThrottleStateModel
    {
        /// <summary>
        /// The set of methods and their throttle states.
        /// </summary>
        public Dictionary<string, MethodGetThrottleStateModel> MethodThrottles { get; set; }

        /// <summary>
        /// Creates a new instance of the MixPlayGetThrottleStateModel class.
        /// </summary>
        public MixPlayGetThrottleStateModel() { this.MethodThrottles = new Dictionary<string, MethodGetThrottleStateModel>(); }

        /// <summary>
        /// Creates a new instance of the MixPlayGetThrottleStateModel class.
        /// </summary>
        /// <param name="result">The result to set</param>
        public MixPlayGetThrottleStateModel(JObject result)
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
