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
            this.EmoteSets = packet.GetTagString("emote-sets");
        }

        /// <summary>
        /// A list containing the user's available emote set IDs.
        /// </summary>
        public List<int> EmoteSetsDictionary
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
