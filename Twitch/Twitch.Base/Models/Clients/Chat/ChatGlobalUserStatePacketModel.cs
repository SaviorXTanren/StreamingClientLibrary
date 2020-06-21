using System.Collections.Generic;

namespace Twitch.Base.Models.Clients.Chat
{
    /// <summary>
    /// Information about a chat global user state packet.
    /// </summary>
    public class ChatGlobalUserStatePacketModel : ChatUserStatePacketModel
    {
        /// <summary>
        /// The ID of the command for a chat global user state.
        /// </summary>
        public new const string CommandID = "GLOBALUSERSTATE";

        /// <summary>
        /// The user’s ID.
        /// </summary>
        public long UserID { get; set; }

        /// <summary>
        /// Creates a new instance of the ChatUserStatePacketModel class.
        /// </summary>
        /// <param name="packet">The Chat packet</param>
        public ChatGlobalUserStatePacketModel(ChatRawPacketModel packet)
            : base(packet)
        {
            this.UserID = packet.GetTagLong("user-id");
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
