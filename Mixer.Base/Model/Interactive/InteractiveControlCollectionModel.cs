using Mixer.Base.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Mixer.Base.Model.Interactive
{
    public class InteractiveConnectedControlCollectionModel : InteractiveControlCollectionModel
    {
        public InteractiveConnectedControlCollectionModel()
        {
            this.buttons = new List<InteractiveConnectedButtonControlModel>();
            this.joysticks = new List<InteractiveConnectedJoystickControlModel>();
        }

        [JsonProperty("controls")]
        public new JArray controlsUnstructured
        {
            get
            {
                JArray array = new JArray();
                array.Merge(JArray.FromObject(this.buttons));
                array.Merge(JArray.FromObject(this.joysticks));
                return array;
            }
            set
            {
                this.buttons = JsonHelper.ConvertJArrayToTypedArray<InteractiveConnectedButtonControlModel>(value).
                    Where(c => c.kind.Equals(InteractiveButtonControlModel.ButtonControlKind)).ToList();
                this.joysticks = JsonHelper.ConvertJArrayToTypedArray<InteractiveConnectedJoystickControlModel>(value).
                    Where(c => c.kind.Equals(InteractiveJoystickControlModel.JoystickControlKind)).ToList();
            }
        }

        [JsonIgnore]
        public new List<InteractiveConnectedButtonControlModel> buttons { get; set; }

        [JsonIgnore]
        public new List<InteractiveConnectedJoystickControlModel> joysticks { get; set; }
    }

    public class InteractiveControlCollectionModel
    {
        public InteractiveControlCollectionModel()
        {
            this.buttons = new List<InteractiveButtonControlModel>();
            this.joysticks = new List<InteractiveJoystickControlModel>();
        }

        [JsonProperty("controls")]
        public JArray controlsUnstructured
        {
            get
            {
                JArray array = new JArray();
                array.Merge(JArray.FromObject(this.buttons));
                array.Merge(JArray.FromObject(this.joysticks));
                return array;
            }
            set
            {
                this.buttons = JsonHelper.ConvertJArrayToTypedArray<InteractiveButtonControlModel>(value).
                    Where(c => c.kind.Equals(InteractiveButtonControlModel.ButtonControlKind)).ToList();
                this.joysticks = JsonHelper.ConvertJArrayToTypedArray<InteractiveJoystickControlModel>(value).
                    Where(c => c.kind.Equals(InteractiveJoystickControlModel.JoystickControlKind)).ToList();
            }
        }

        [JsonIgnore]
        public List<InteractiveButtonControlModel> buttons { get; set; }

        [JsonIgnore]
        public List<InteractiveJoystickControlModel> joysticks { get; set; }
    }
}