using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StreamingClient.Base.Util;
using System.Collections.Generic;
using System.Linq;

namespace Mixer.Base.Model.MixPlay
{
    /// <summary>
    /// A collection of connected MixPlay controls.
    /// </summary>
    public class MixPlayConnectedControlCollectionModel : MixPlayControlCollectionModel
    {
        /// <summary>
        /// Creates a new instance of the MixPlayConnectedControlCollectionModel class.
        /// </summary>
        public MixPlayConnectedControlCollectionModel()
        {
            this.buttons = new List<MixPlayConnectedButtonControlModel>();
            this.joysticks = new List<MixPlayConnectedJoystickControlModel>();
            this.labels = new List<MixPlayConnectedLabelControlModel>();
            this.textBoxes = new List<MixPlayConnectedTextBoxControlModel>();
        }

        /// <summary>
        /// The unstructured JSON of the MixPlay controls.
        /// </summary>
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
                this.buttons = value.ToTypedArray<MixPlayConnectedButtonControlModel>().
                    Where(c => c.kind.Equals(MixPlayButtonControlModel.ButtonControlKind)).ToList();
                this.joysticks = value.ToTypedArray<MixPlayConnectedJoystickControlModel>().
                    Where(c => c.kind.Equals(MixPlayJoystickControlModel.JoystickControlKind)).ToList();
                this.labels = value.ToTypedArray<MixPlayConnectedLabelControlModel>().
                    Where(c => c.kind.Equals(MixPlayConnectedLabelControlModel.LabelControlKind)).ToList();
                this.textBoxes = value.ToTypedArray<MixPlayConnectedTextBoxControlModel>().
                    Where(c => c.kind.Equals(MixPlayConnectedTextBoxControlModel.TextBoxControlKind)).ToList();
            }
        }

        /// <summary>
        /// The set of MixPlay buttons.
        /// </summary>
        [JsonIgnore]
        public new List<MixPlayConnectedButtonControlModel> buttons { get; set; }

        /// <summary>
        /// The set of MixPlay joysticks.
        /// </summary>
        [JsonIgnore]
        public new List<MixPlayConnectedJoystickControlModel> joysticks { get; set; }

        /// <summary>
        /// The set of MixPlay labels.
        /// </summary>
        [JsonIgnore]
        public new List<MixPlayConnectedLabelControlModel> labels { get; set; }

        /// <summary>
        /// The set of MixPlay text boxes.
        /// </summary>
        [JsonIgnore]
        public new List<MixPlayConnectedTextBoxControlModel> textBoxes { get; set; }

        /// <summary>
        /// All MixPlay controls in the set.
        /// </summary>
        [JsonIgnore]
        public new IEnumerable<MixPlayControlModel> allControls
        {
            get
            {
                List<MixPlayControlModel> controls = new List<MixPlayControlModel>();
                controls.AddRange(this.buttons);
                controls.AddRange(this.joysticks);
                controls.AddRange(this.labels);
                controls.AddRange(this.textBoxes);
                return controls;
            }
        }
    }

    /// <summary>
    /// A collection of MixPlay controls.
    /// </summary>
    public class MixPlayControlCollectionModel
    {
        /// <summary>
        /// Creates a new instance of the MixPlayControlCollectionModel class.
        /// </summary>
        public MixPlayControlCollectionModel()
        {
            this.buttons = new List<MixPlayButtonControlModel>();
            this.joysticks = new List<MixPlayJoystickControlModel>();
            this.labels = new List<MixPlayLabelControlModel>();
            this.textBoxes = new List<MixPlayTextBoxControlModel>();
        }

        /// <summary>
        /// The unstructured JSON of the MixPlay controls.
        /// </summary>
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
                this.buttons = value.ToTypedArray<MixPlayButtonControlModel>().
                    Where(c => c.kind.Equals(MixPlayButtonControlModel.ButtonControlKind)).ToList();
                this.joysticks = value.ToTypedArray<MixPlayJoystickControlModel>().
                    Where(c => c.kind.Equals(MixPlayJoystickControlModel.JoystickControlKind)).ToList();
                this.labels = value.ToTypedArray<MixPlayLabelControlModel>().
                    Where(c => c.kind.Equals(MixPlayLabelControlModel.LabelControlKind)).ToList();
                this.textBoxes = value.ToTypedArray<MixPlayTextBoxControlModel>().
                    Where(c => c.kind.Equals(MixPlayTextBoxControlModel.TextBoxControlKind)).ToList();
            }
        }

        /// <summary>
        /// The set of MixPlay buttons.
        /// </summary>
        [JsonIgnore]
        public List<MixPlayButtonControlModel> buttons { get; set; }

        /// <summary>
        /// The set of MixPlay joysticks.
        /// </summary>
        [JsonIgnore]
        public List<MixPlayJoystickControlModel> joysticks { get; set; }

        /// <summary>
        /// The set of MixPlay labels.
        /// </summary>
        [JsonIgnore]
        public List<MixPlayLabelControlModel> labels { get; set; }

        /// <summary>
        /// The set of MixPlay text boxes.
        /// </summary>
        [JsonIgnore]
        public List<MixPlayTextBoxControlModel> textBoxes { get; set; }

        /// <summary>
        /// All MixPlay controls in the set.
        /// </summary>
        [JsonIgnore]
        public IEnumerable<MixPlayControlModel> allControls
        {
            get
            {
                List<MixPlayControlModel> controls = new List<MixPlayControlModel>();
                controls.AddRange(this.buttons);
                controls.AddRange(this.joysticks);
                controls.AddRange(this.labels);
                controls.AddRange(this.textBoxes);
                return controls;
            }
        }
    }
}