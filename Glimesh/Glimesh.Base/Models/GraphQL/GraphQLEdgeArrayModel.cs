using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Glimesh.Base.Models.GraphQL
{
    /// <summary>
    /// An edge array in GraphQL.
    /// </summary>
    public class GraphQLEdgeArrayModel<T>
    {
        /// <summary>
        /// The edge items.
        /// </summary>
        public List<GraphQLEdgeNodeArrayModel<T>> edges { get; set; } = new List<GraphQLEdgeNodeArrayModel<T>>();

        /// <summary>
        /// Gets the items in the array.
        /// </summary>
        [JsonIgnore]
        public IEnumerable<T> Items { get { return this.edges.Select(e => e.node); } }
    }

    /// <summary>
    /// An edge node array in GraphQL.
    /// </summary>
    public class GraphQLEdgeNodeArrayModel<T>
    {
        /// <summary>
        /// The edge cursor.
        /// </summary>
        public string cursor { get; set; }

        /// <summary>
        /// The node items.
        /// </summary>
        public T node { get; set; }
    }
}
