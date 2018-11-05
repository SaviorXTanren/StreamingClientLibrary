using System;

namespace Mixer.Base.Model.Chat
{
    /// <summary>
    /// A skill that has been in a channel's chat.
    /// </summary>
    public class ChatSkillModel
    {
        /// <summary>
        /// The unique ID of the skill.
        /// </summary>
        public Guid skill_id { get; set; }
        /// <summary>
        /// The name of the skill.
        /// </summary>
        public string skill_name { get; set; }
        /// <summary>
        /// The unique execution ID of the instance of the skill.
        /// </summary>
        public Guid execution_id { get; set; }
        /// <summary>
        /// The URL for the icon of the skill.
        /// </summary>
        public string icon_url { get; set; }
        /// <summary>
        /// The cost of the skill.
        /// </summary>
        public uint cost { get; set; }
        /// <summary>
        /// The type of currency used for the skill.
        /// </summary>
        public string currency { get; set; }
    }

    /// <summary>
    /// The event for a skill used in a channel's chat.
    /// </summary>
    public class ChatSkillAttributionEventModel : ChatMessageUserModel
    {
        /// <summary>
        /// The skill used.
        /// </summary>
        public ChatSkillModel skill { get; set; }
    }
}
