using System.Collections.Generic;

namespace Twitch.Base.Models.Clients.Chat
{
    /// <summary>
    /// Information about a chat user state packet.
    /// </summary>
    public class ChatUserStatePacketModel : ChatPacketModelBase
    {
        /// <summary>
        /// The ID of the command for a chat user state.
        /// </summary>
        public const string CommandID = "USERSTATE";

        /// <summary>
        /// The user's display name.
        /// </summary>
        public string UserDisplayName { get; set; }

        /// <summary>
        /// The user's badge information.
        /// </summary>
        public string UserBadgeInfo { get; set; }

        /// <summary>
        /// The user's badges.
        /// </summary>
        public string UserBadges { get; set; }

        /// <summary>
        /// Indicates whether the user is a moderator.
        /// </summary>
        public bool Moderator { get; set; }

        /// <summary>
        /// Hexadecimal RGB color code of the message, if any.
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// Information to the user's emote sets.
        /// </summary>
        public string EmoteSets { get; set; }

        /// <summary>
        /// Creates a new instance of the ChatUserStatePacketModel class.
        /// </summary>
        /// <param name="packet">The Chat packet</param>
        public ChatUserStatePacketModel(ChatRawPacketModel packet)
            : base(packet)
        {
            this.UserDisplayName = packet.GetTagString("display-name");
            this.UserBadgeInfo = packet.GetTagString("badge-info");
            this.UserBadges = packet.GetTagString("badges");
            this.Moderator = packet.GetTagBool("mod");

            this.Color = packet.GetTagString("color");
            this.EmoteSets = packet.GetTagString("emote-sets");
        }

        /// <summary>
        /// A dictionary containing the user's badges and associated versions.
        /// </summary>
        public Dictionary<string, int> BadgeVersions
        {
            get
            {
                Dictionary<string, int> results = new Dictionary<string, int>();
                if (!string.IsNullOrEmpty(this.UserBadges))
                {
                    string[] splits = this.UserBadges.Split(new char[] { ',', '/' });
                    if (splits != null && splits.Length > 0 && splits.Length % 2 == 0)
                    {
                        for (int i = 0; i < splits.Length; i = i + 2)
                        {
                            if (int.TryParse(splits[i + 1], out int version))
                            {
                                results[splits[i]] = version;
                            }
                        }
                    }
                }
                return results;
            }
        }

        /// <summary>
        /// A list containing the user's available emote set IDs.
        /// </summary>
        public List<int> AvailableEmoteSets
        {
            get
            {
                List<int> results = new List<int>();
                if (!string.IsNullOrEmpty(this.EmoteSets))
                {
                    string[] splits = this.EmoteSets.Split(new char[] { ',' });
                    if (splits != null && splits.Length > 0)
                    {
                        foreach (string split in splits)
                        {
                            if (int.TryParse(split, out int emoteSet))
                            {
                                results.Add(emoteSet);
                            }
                        }
                    }
                }
                return results;
            }
        }
    }
}
