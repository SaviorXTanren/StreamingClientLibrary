using Newtonsoft.Json.Linq;

namespace Mixer.Base.Model.Chat
{
    /// <summary>
    /// Information about a poll.
    /// </summary>
    public class ChatPollEventModel
    {
        /// <summary>
        /// The channel that started the poll.
        /// </summary>
        public uint originatingChannel { get; set; }
        /// <summary>
        /// The question of the poll.
        /// </summary>
        public string q { get; set; }
        /// <summary>
        /// The selectable answer of the poll.
        /// </summary>
        public string[] answers { get; set; }
        /// <summary>
        /// The creator of the poll.
        /// </summary>
        public ChatUserEventModel author { get; set; }
        /// <summary>
        /// The length of the poll in milliseconds.
        /// </summary>
        public uint duration { get; set; }
        /// <summary>
        /// The timestamp when the poll ends at.
        /// </summary>
        public long endsAt { get; set; }
        /// <summary>
        /// The total number of voters.
        /// </summary>
        public uint voters { get; set; }
        /// <summary>
        /// The responses of the poll with their total number of votes.
        /// </summary>
        public JObject responses { get; set; }
    }
}
