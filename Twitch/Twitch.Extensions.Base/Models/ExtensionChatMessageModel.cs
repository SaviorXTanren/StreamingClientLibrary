using System.Runtime.Serialization;

namespace Twitch.Extensions.Base.Models
{
    /// <summary>
    /// The text of the chat message sent by an extension.
    /// </summary>
    [DataContract]
    public class ExtensionChatMessageModel
    {
        /// <summary>
        /// The text of the chat message.
        /// </summary>
        [DataMember]
        public string text { get; set; }

        /// <summary>
        /// Creates a new instance of the ExtensionChatMessageModel class.
        /// </summary>
        /// <param name="text">The text of the chat message</param>
        public ExtensionChatMessageModel(string text) { this.text = text; }
    }
}
