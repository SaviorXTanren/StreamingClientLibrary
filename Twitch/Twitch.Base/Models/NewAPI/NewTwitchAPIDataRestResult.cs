using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Twitch.Base.Models.NewAPI
{
    /// <summary>
    /// A wrapper result used forthe New Twitch APIs
    /// </summary>
    /// <typeparam name="T">The type that the result contains</typeparam>
    public class NewTwitchAPIDataRestResult<T>
    {
        /// <summary>
        /// The data of the result.
        /// </summary>
        public List<T> data { get; set; } = new List<T>();

        /// <summary>
        /// The total number of results.
        /// </summary>
        public long total { get; set; }

        /// <summary>
        /// Pagination information.
        /// </summary>
        public JObject pagination { get; set; }

        /// <summary>
        /// The pagination cursor.
        /// </summary>
        public string Cursor { get { return (this.pagination!=null && this.pagination.ContainsKey("cursor")) ? this.pagination["cursor"].ToString() : null; } }
    }
}
