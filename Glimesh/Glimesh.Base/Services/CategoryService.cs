using Glimesh.Base.Models.Channels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Glimesh.Base.Services
{
    /// <summary>
    /// The APIs for Category-based services.
    /// </summary>
    public class CategoryService : GlimeshServiceBase
    {
        /// <summary>
        /// Creates an instance of the CategoryService.
        /// </summary>
        /// <param name="connection">The Glimesh connection to use</param>
        public CategoryService(GlimeshConnection connection) : base(connection) { }

        /// <summary>
        /// Gets all of the available categories
        /// </summary>
        /// <returns>All available categories</returns>
        public async Task<IEnumerable<CategoryModel>> GetCategories() { return await this.QueryAsync<IEnumerable<CategoryModel>>($"{{ categories {{ {CategoryModel.AllFields} }} }}", "categories"); }

        /// <summary>
        /// Gets the category with the specified slug.
        /// </summary>
        /// <param name="slug">The slug of the categrory</param>
        /// <returns>The category</returns>
        public async Task<CategoryModel> GetCategoryBySlug(string slug) { return await this.QueryAsync<CategoryModel>($"{{ category(slug: \"{slug}\") {{ {CategoryModel.AllFields} }} }}", "category"); }
    }
}