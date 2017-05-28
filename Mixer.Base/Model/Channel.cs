namespace Mixer.Base.Model
{
    public class Channel
    {
        public uint id { get; set; }
        public uint userId { get; set; }
        public string name { get; set; }
        public string token { get; set; }

        public bool featured { get; set; }
        public int featureLevel { get; set; }
        public bool partnered { get; set; }
        public bool suspended { get; set; }

        public bool online { get; set; }
        public string audience { get; set; }
        public uint viewersTotal { get; set; }
        public uint viewersCurrent { get; set; }
        public uint numFollowers { get; set; }
    }
}
