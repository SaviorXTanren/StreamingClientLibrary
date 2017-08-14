using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Mixer.Base.Model.Interactive
{
    public class InteractiveControlCollectionModel
    {
        [JsonProperty("controls")]
        public JArray controlsUnstructured { get; set; }

        [JsonIgnore]
        public List<InteractiveControlModel> controls
        {
            get { return this.ConvertJArrayToTypedArray<InteractiveControlModel>(this.controlsUnstructured); }
            set { JArray.FromObject(value); }
        }

        [JsonIgnore]
        public IEnumerable<InteractiveButtonControlModel> buttons
        {
            get { return this.ConvertJArrayToTypedArray<InteractiveButtonControlModel>(this.controlsUnstructured).Where(c => c.kind.Equals(InteractiveButtonControlModel.ButtonControlKind)); }
        }

        [JsonIgnore]
        public IEnumerable<InteractiveJoystickControlModel> joysticks
        {
            get { return this.ConvertJArrayToTypedArray<InteractiveJoystickControlModel>(this.controlsUnstructured).Where(c => c.kind.Equals(InteractiveJoystickControlModel.JoystickControlKind)); }
        }

        private List<T> ConvertJArrayToTypedArray<T>(JArray array)
        {
            List<T> results = new List<T>();
            foreach (JToken token in array)
            {
                results.Add(token.ToObject<T>());
            }
            return results;
        }
    }
}