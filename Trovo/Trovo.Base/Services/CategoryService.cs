using StreamingClient.Base.Util;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trovo.Base.Models.Category;

namespace Trovo.Base.Services
{
    /// <summary>
    /// The APIs for category-based services.
    /// </summary>
    public class CategoryService : TrovoServiceBase
    {
        private class CategoryWrapperModel
        {
            public List<CategoryModel> category_info { get; set; }
        }

        /// <summary>
        /// Creates an instance of the CategoryService.
        /// </summary>
        /// <param name="connection">The Trovo connection to use</param>
        public CategoryService(TrovoConnection connection) : base(connection) { }

        /// <summary>
        /// Gets the top categories.
        /// </summary>
        /// <returns>The top categories</returns>
        public async Task<IEnumerable<CategoryModel>> GetTopCategories()
        {
            CategoryWrapperModel categories = await this.GetAsync<CategoryWrapperModel>("categorys/top");
            if (categories != null)
            {
                return categories.category_info;
            }
            return null;
        }

        /// <summary>
        /// Searches for categories matching the specified query.
        /// </summary>
        /// <param name="query">The query to search for</param>
        /// <param name="maxResults">The maximum number of results. Will be either that amount or slightly more</param>
        /// <returns>The matching categories</returns>
        public async Task<IEnumerable<CategoryModel>> SearchCategories(string query, int maxResults = 1)
        {
            Validator.ValidateString(query, "query");

            CategoryWrapperModel categories = await this.GetAsync<CategoryWrapperModel>($"categorys/searchcategory?query={query}&limit={maxResults}");
            if (categories != null)
            {
                return categories.category_info;
            }
            return null;
        }
    }
}
