namespace Glimesh.Base.Models.Channels
{
    /// <summary>
    /// Information for a category.
    /// </summary>
    public class CategoryModel
    {
        /// <summary>
        /// All fields for a GraphQL query.
        /// </summary>
        public const string AllFields = "id, name, slug";

        /// <summary>
        /// The ID of the category
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// The display name of the category.
        /// </summary>
        public string name { get; set; }
        
        /// <summary>
        /// The unique slug string of the category.
        /// </summary>
        public string slug { get; set; }
    }
}
