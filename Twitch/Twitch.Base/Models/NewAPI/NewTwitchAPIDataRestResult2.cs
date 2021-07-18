using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Twitch.Base.Models.NewAPI
{
    /// <summary>
    /// A wrapper result used for the New Twitch APIs where results is a single object for the data node
    /// Used for APIs such as schedule
    /// </summary>
    /// <typeparam name="T">The type that the result contains</typeparam>
    public class NewTwitchAPIDataRestResult2<T>
    {
        /// <summary>
        /// The data of the result.
        /// </summary>
        public T data { get; set; } 

        /// <summary>
        /// Pagination information.
        /// </summary>
        public JObject pagination { get; set; }

        /// <summary>
        /// The pagination cursor.
        /// </summary>
        public string Cursor { get { return (this.pagination != null && this.pagination.ContainsKey("cursor")) ? this.pagination["cursor"].ToString() : null; } }
    }
}
