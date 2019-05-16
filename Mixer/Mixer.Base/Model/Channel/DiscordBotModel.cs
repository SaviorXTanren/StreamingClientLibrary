using Newtonsoft.Json.Linq;

namespace Mixer.Base.Model.Channel
{
    public class DiscordBotModel
    {
        public uint id { get; set; }
        public uint channelId { get; set; }
        public string guildId { get; set; }
        public string inviteSettings { get; set; }
        public string inviteChannel { get; set; }
        public string liveUpdateState { get; set; }
        public string liveChatChannel { get; set; }
        public string liveAnnounceChannel { get; set; }
        public uint[] syncEmoteRoles { get; set; }
        public JArray roles { get; set; }
    }
}
