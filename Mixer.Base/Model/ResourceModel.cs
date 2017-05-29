namespace Mixer.Base.Model
{
    public class ResourceModel
    {
        public uint id { get; set; }
        public string type { get; set; }
        public uint? relid { get; set; }
        public string url { get; set; }
        public string store { get; set; }
        public string remotePath { get; set; }
    }
}
