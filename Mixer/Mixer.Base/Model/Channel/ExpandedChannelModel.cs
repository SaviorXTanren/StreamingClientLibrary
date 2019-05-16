namespace Mixer.Base.Model.Channel
{
    public class ExpandedChannelModel : ChannelAdvancedModel
    {
        public ResourceModel thumbnail { get; set; }
        public ResourceModel cover { get; set; }
        public ResourceModel badge { get; set; }
        public ChannelPreferencesModel preferences { get; set; }
    }
}
