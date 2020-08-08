namespace Trovo.Base.Models.Channels
{
    public class PrivateChannelModel : ChannelModel
    {
        public string uid { get; set; }
        public string stream_key { get; set; }
        public string created_at { get; set; }
    }
}
