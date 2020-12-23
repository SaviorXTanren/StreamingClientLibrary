namespace Glimesh.Base.Models.Users
{
    /// <summary>
    /// Information about a user subscription
    /// </summary>
    public class UserSubscriptionModel
    {
        /// <summary>
        /// All fields for a GraphQL query.
        /// </summary>
        public const string AllFields = "id, productName, price, isActive, insertedAt, updatedAt, endedAt";

        /// <summary>
        /// All fields for a GraphQL query.
        /// </summary>
        public static readonly string AllFieldsWithStreamerAndUser = $"{UserSubscriptionModel.AllFields}, streamer {{ {UserModel.AllFields} }}, user {{ {UserModel.AllFields} }}";

        /// <summary>
        /// The ID of the subscription.
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// The name of the subscription product.
        /// </summary>
        public string productName { get; set; }

        /// <summary>
        /// The price of the subscription.
        /// </summary>
        public string price { get; set; }

        /// <summary>
        /// Whether the subscription is currently active.
        /// </summary>
        public bool isActive { get; set; }

        /// <summary>
        /// The datetime of the subscription.
        /// </summary>
        public string insertedAt { get; set; }

        /// <summary>
        /// The datetime of the subscription update.
        /// </summary>
        public string updatedAt { get; set; }

        /// <summary>
        /// The datetime of the subscription end.
        /// </summary>
        public string endedAt { get; set; }

        /// <summary>
        /// The streamer who was subscribed.
        /// </summary>
        public UserModel streamer { get; set; }

        /// <summary>
        /// The user who subscribed.
        /// </summary>
        public UserModel user { get; set; }
    }
}
