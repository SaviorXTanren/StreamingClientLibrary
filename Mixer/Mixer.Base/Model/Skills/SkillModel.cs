using System;

namespace Mixer.Base.Model.Skills
{
    /// <summary>
    /// The full details of a skill.
    /// </summary>
    public class SkillModel
    {
        /// <summary>
        /// The ID of a skill.
        /// </summary>
        public Guid id { get; set; }
        /// <summary>
        /// The name of the skill.
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// The description of the skill.
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// Whether the skill is locked.
        /// </summary>
        public bool locked { get; set; }
        /// <summary>
        /// The reason for the skill being locked.
        /// </summary>
        public int lockReason { get; set; }
        /// <summary>
        /// The category of the skill.
        /// </summary>
        public string category { get; set; }
        /// <summary>
        /// The URL for the icon of the skill.
        /// </summary>
        public string iconUrl { get; set; }
        /// <summary>
        /// The URL of a preview of the skill.
        /// </summary>
        public string previewUrl { get; set; }
        /// <summary>
        /// The level required to unlock this skill.
        /// </summary>
        public uint globalLevelUnlock { get; set; }
        /// <summary>
        /// The price to use the skill.
        /// </summary>
        public uint price { get; set; }
        /// <summary>
        /// The currency type required for the skill.
        /// </summary>
        public int currency { get; set; }
        /// <summary>
        /// The URL for any setup involved for the skill.
        /// </summary>
        public string skillSetupUrl { get; set; }
        /// <summary>
        /// The URL for the execution of the skill.
        /// </summary>
        public string skillExecutionUrl { get; set; }
        /// <summary>
        /// The URL of the icon to show for attribution of the skill.
        /// </summary>
        public string attributionIconUrl { get; set; }
        /// <summary>
        /// The URL for the assets for the skill.
        /// </summary>
        public string skillAssetsUrl { get; set; }
    }
}
