namespace Mixer.Base.Model.Interactive
{
    public class InteractiveGameControlsModel
    {
        public InteractiveGameControlsSceneModel[] scenes { get; set; }
    }

    public class InteractiveGameControlsSceneModel
    {
        public string sceneID { get; set; }
        public InteractiveControlModel[] controls { get; set; }
    }
}
