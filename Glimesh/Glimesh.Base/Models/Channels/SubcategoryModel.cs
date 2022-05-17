namespace Glimesh.Base.Models.Channels
{
    /// <summary>
    /// Information for a subcategory.
    /// </summary>
    public class SubcategoryModel
    {
        /// <summary>
        /// All fields for a GraphQL query.
        /// </summary>
        public const string AllFields = "id, name, slug, backgroundImageUrl";

        /// <summary>
        /// The ID of the subcategory
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// The display name of the subcategory.
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// The unique slug string of the subcategory.
        /// </summary>
        public string slug { get; set; }

        /// <summary>
        /// The background image URL of the subcategory.
        /// </summary>
        public string backgroundImageUrl { get; set; }
    }
}
