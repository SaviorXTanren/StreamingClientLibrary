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
            this.labels = new List<InteractiveConnectedLabelControlModel>();
            this.textBoxes = new List<InteractiveConnectedTextBoxControlModel>();
        }

        [JsonProperty("controls")]
        public new JArray controlsUnstructured
        {
            get
            {
                JArray array = new JArray();
                array.Merge(JArray.FromObject(this.buttons));
                array.Merge(JArray.FromObject(this.joysticks));
                array.Merge(JArray.FromObject(this.labels));
                array.Merge(JArray.FromObject(this.textBoxes));
                return array;
            }
            set
            {
                this.buttons = JsonHelper.ConvertJArrayToTypedArray<InteractiveConnectedButtonControlModel>(value).
                    Where(c => c.kind.Equals(InteractiveButtonControlModel.ButtonControlKind)).ToList();
                this.joysticks = JsonHelper.ConvertJArrayToTypedArray<InteractiveConnectedJoystickControlModel>(value).
                    Where(c => c.kind.Equals(InteractiveJoystickControlModel.JoystickControlKind)).ToList();
                this.labels = JsonHelper.ConvertJArrayToTypedArray<InteractiveConnectedLabelControlModel>(value).
                    Where(c => c.kind.Equals(InteractiveConnectedLabelControlModel.LabelControlKind)).ToList();
                this.textBoxes = JsonHelper.ConvertJArrayToTypedArray<InteractiveConnectedTextBoxControlModel>(value).
                    Where(c => c.kind.Equals(InteractiveConnectedTextBoxControlModel.TextBoxControlKind)).ToList();
            }
        }

        [JsonIgnore]
        public new List<InteractiveConnectedButtonControlModel> buttons { get; set; }

        [JsonIgnore]
        public new List<InteractiveConnectedJoystickControlModel> joysticks { get; set; }

        [JsonIgnore]
        public new List<InteractiveConnectedLabelControlModel> labels { get; set; }

        [JsonIgnore]
        public new List<InteractiveConnectedTextBoxControlModel> textBoxes { get; set; }

        [JsonIgnore]
        public new IEnumerable<InteractiveControlModel> allControls
        {
            get
            {
                List<InteractiveControlModel> controls = new List<InteractiveControlModel>();
                controls.AddRange(this.buttons);
                controls.AddRange(this.joysticks);
                controls.AddRange(this.labels);
                controls.AddRange(this.textBoxes);
                return controls;
            }
        }
    }

    public class InteractiveControlCollectionModel
    {
        public InteractiveControlCollectionModel()
        {
            this.buttons = new List<InteractiveButtonControlModel>();
            this.joysticks = new List<InteractiveJoystickControlModel>();
            this.labels = new List<InteractiveLabelControlModel>();
            this.textBoxes = new List<InteractiveTextBoxControlModel>();
        }

        [JsonProperty("controls")]
        public JArray controlsUnstructured
        {
            get
            {
                JArray array = new JArray();
                array.Merge(JArray.FromObject(this.buttons));
                array.Merge(JArray.FromObject(this.joysticks));
                array.Merge(JArray.FromObject(this.labels));
                array.Merge(JArray.FromObject(this.textBoxes));
                return array;
            }
            set
            {
                this.buttons = JsonHelper.ConvertJArrayToTypedArray<InteractiveButtonControlModel>(value).
                    Where(c => c.kind.Equals(InteractiveButtonControlModel.ButtonControlKind)).ToList();
                this.joysticks = JsonHelper.ConvertJArrayToTypedArray<InteractiveJoystickControlModel>(value).
                    Where(c => c.kind.Equals(InteractiveJoystickControlModel.JoystickControlKind)).ToList();
                this.labels = JsonHelper.ConvertJArrayToTypedArray<InteractiveLabelControlModel>(value).
                    Where(c => c.kind.Equals(InteractiveLabelControlModel.LabelControlKind)).ToList();
                this.textBoxes = JsonHelper.ConvertJArrayToTypedArray<InteractiveTextBoxControlModel>(value).
                    Where(c => c.kind.Equals(InteractiveTextBoxControlModel.TextBoxControlKind)).ToList();
            }
        }

        [JsonIgnore]
        public List<InteractiveButtonControlModel> buttons { get; set; }

        [JsonIgnore]
        public List<InteractiveJoystickControlModel> joysticks { get; set; }

        [JsonIgnore]
        public List<InteractiveLabelControlModel> labels { get; set; }

        [JsonIgnore]
        public List<InteractiveTextBoxControlModel> textBoxes { get; set; }

        [JsonIgnore]
        public IEnumerable<InteractiveControlModel> allControls
        {
            get
            {
                List<InteractiveControlModel> controls = new List<InteractiveControlModel>();
                controls.AddRange(this.buttons);
                controls.AddRange(this.joysticks);
                controls.AddRange(this.labels);
                controls.AddRange(this.textBoxes);
                return controls;
            }
        }
    }
}