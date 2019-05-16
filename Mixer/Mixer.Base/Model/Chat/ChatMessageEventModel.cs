using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Mixer.Base.Model.Chat
{
    /// <summary>
    /// The base class for all chat messages from users.
    /// </summary>
    public class ChatMessageUserModel
    {
        /// <summary>
        /// The source user ID.
        /// </summary>
        public uint user_id { get; set; }

        /// <summary>
        /// The source user name.
        /// </summary>
        public string user_name { get; set; }

        /// <summary>
        /// The source user roles.
        /// </summary>
        public string[] user_roles { get; set; }

        /// <summary>
        /// The source user level.
        /// </summary>
        public uint user_level { get; set; }

        /// <summary>
        /// The source user avatar.
        /// </summary>
        public string user_avatar { get; set; }
    }

    public class CoordinatesModel
    {
        public uint x { get; set; }
        public uint y { get; set; }
        public uint width { get; set; }
        public uint height { get; set; }
    }

    public class ChatMessageDataModel
    {
        public string type { get; set; }
        public object data { get; set; }
        public string text { get; set; }
        public uint id { get; set; }
        public string username { get; set; }
        public string source { get; set; }
        public string pack { get; set; }
        public string url { get; set; }
        public CoordinatesModel coords { get; set; }
    }

    /// <summary>
    /// The contents of a chat message in a channel.
    /// </summary>
    public class ChatMessageContentsModel
    {
        /// <summary>
        /// An array consisting of all elements of the chat message.
        /// </summary>
        public ChatMessageDataModel[] message { get; set; }

        /// <summary>
        /// Metadata properties associated with the chat message.
        /// </summary>
        public JObject meta { get; set; }

        /// <summary>
        /// Identifies whether the message contains a skill or not.
        /// </summary>
        [JsonIgnore]
        public bool ContainsSkill { get { return this.meta != null && this.meta["is_skill"] != null && (bool)this.meta["is_skill"]; } }

        /// <summary>
        /// The skill that was used with the chat message, if it exists.
        /// </summary>
        [JsonIgnore]
        public ChatSkillModel Skill
        {
            get
            {
                if (this.ContainsSkill && this.meta["skill"] != null)
                {
                    return this.meta["skill"].ToObject<ChatSkillModel>();
                }
                return null;
            }
        }
    }

    /// <summary>
    /// This model is used to represent a chat message event from the chat service.
    /// </summary>
    public class ChatMessageEventModel : ChatMessageUserModel
    {
        /// <summary>
        /// The UUID of this message.
        /// </summary>
        public Guid id { get; set; }

        /// <summary>
        /// The channel ID this message is associated with.
        /// </summary>
        public uint channel { get; set; }

        /// <summary>
        /// The message deatils.
        /// </summary>
        public ChatMessageContentsModel message { get; set; }

        /// <summary>
        /// The target of this message if it was a whisper.
        /// </summary>
        public string target { get; set; }
    }
}
