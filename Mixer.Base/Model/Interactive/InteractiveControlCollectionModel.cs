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
        public IEnumerable<InteractiveConnectedButtonControlModel> buttons
        {
            get { return this.ConvertJArrayToTypedArray<InteractiveConnectedButtonControlModel>(this.controlsUnstructured).Where(c => c.kind.Equals(InteractiveButtonControlModel.ButtonControlKind)); }
        }

        [JsonIgnore]
        public IEnumerable<InteractiveConnectedJoystickControlModel> joysticks
        {
            get { return this.ConvertJArrayToTypedArray<InteractiveConnectedJoystickControlModel>(this.controlsUnstructured).Where(c => c.kind.Equals(InteractiveJoystickControlModel.JoystickControlKind)); }
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