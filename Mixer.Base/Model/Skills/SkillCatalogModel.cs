using System;

namespace Mixer.Base.Model.Skills
{
    /// <summary>
    /// The catalog of skills available.
    /// </summary>
    public class SkillCatalogModel
    {
        /// <summary>
        /// All skills that are available.
        /// </summary>
        public SkillModel[] skills { get; set; }
        /// <summary>
        /// The order of categories for displaying.
        /// </summary>
        public string[] categoryOrder { get; set; }
        /// <summary>
        /// Specific skills that are being promoted.
        /// </summary>
        public Guid[] promotedSkills { get; set; }
    }
}
