namespace Trovo.Base.Models
{
    /// <summary>
    /// A paged response.
    /// </summary>
    public class PageDataResponseModel
    {
        /// <summary>
        /// The page instance token.
        /// </summary>
        public string token { get; set; }
        /// <summary>
        /// The total number of pages.
        /// </summary>
        public int total_page { get; set; }
        /// <summary>
        /// The current page number.
        /// </summary>
        public int cursor { get; set; }
    }
}
