using System;

namespace Mixer.Base.Model.Chat
{
    /// <summary>
    /// The event for the deletion of a skill in a channel's chat.
    /// </summary>
    public class ChatDeleteSkillAttributionEventModel
    {
        /// <summary>
        /// The moderator user who deleted the skill.
        /// </summary>
        public ChatMessageUserModel moderator { get; set; }
        /// <summary>
        /// The unique execution ID of the instance of the skill.
        /// </summary>
        public Guid execution_id { get; set; }
    }
}
